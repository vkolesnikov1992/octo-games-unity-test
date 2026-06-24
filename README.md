# Octo Games Unity Test

Unity `6000.3.7f1`. Код тестового задания лежит в `Assets/Modules`.

В проекте реализован только объём из ТЗ: core-код систем и ответы по архитектуре. Сценовая сборка, реальные prefab, UI-навигация, загрузка ресурсов, popup queue, production DI bootstrap, облачные сохранения и VN/game-specific integrations ожидаются как внешний код проекта.

Зависимости: UniTask, Extenject/Zenject, uGUI, TextMeshPro.

## Модули

- `SaveSystem` - generic save/load через JSON и файловое хранилище.
- `PopupSystem` - слой содержимого popup: параметры, view, presenter, кнопки.
- `RefactoringTask` - исправленный `CharactersView` для live UI.
- `ActiveEntitiesSystem` - registry/query для активных gameplay entity.

## 1. Coding Principles

**Инверсия зависимостей.** Gameplay/UI-код должен зависеть от интерфейсов, а не от конкретных файлов, prefab loader, DI-контейнера или сцены. Здесь это видно в `ISaveService`, `ISaveStorage`, `ISaveSerializer`, `IEntityQuery`, `IEntityRegistry`, `IPopupPresenter`.

**Декомпозиция ответственности.** Система должна разделять хранение данных, отображение, lifecycle и gameplay-логику. Это упрощает замену реализации и делает код безопаснее для итераций дизайнеров: popup view не решает, как загружается prefab, save service не знает про конкретный экран, entity registry не сканирует UI.

**ООП через небольшие контракты.** Для Unity-проектов удобнее держать поведение за маленькими интерфейсами и базовыми типами, а настройки отдавать в serialized fields/parameters. Такой подход позволяет расширять popup types, save backends и gameplay entity kinds без переписывания вызывающего кода.

**Событийные и ограниченные UI-обновления.** UI не должен пересчитываться в `FixedUpdate` без причины. В `CharactersView` данные помечают view как dirty через событие, а реальное обновление ограничено кадрами/интервалом.

## 2. Save / Load Utility

Основной API:

```csharp
public interface ISaveService
{
    UniTask SaveAsync<T>(string key, T data, CancellationToken cancellationToken = default)
        where T : class;

    UniTask<LoadResult<T>> LoadAsync<T>(string key, CancellationToken cancellationToken = default)
        where T : class;
}
```

Что сделано:

- `JsonSaveService` сохраняет данные в envelope с версией и типом.
- `FileSaveStorage` пишет JSON в `Application.persistentDataPath/Saves`.
- `UnityJsonSaveSerializer` использует `JsonUtility`.
- `LoadResult<T>` возвращает `FileNotFound`, `ReadFailed`, `InvalidJson`, `TypeMismatch`, `DeserializationFailed` без исключений для обычных ошибок загрузки.

Ограничение: `JsonUtility` работает с Unity-serializable классами и имеет стандартные ограничения Unity JSON. Для Newtonsoft/System.Text.Json достаточно заменить `ISaveSerializer`.

## 3. Popup / UI System

Реализован слой содержимого popup. Внешний UI-модуль должен загрузить prefab, выбрать presenter, вызвать lifecycle и уничтожить/release instance.

Пример параметров:

```csharp
new MessagePopupParameters(
    "Exit",
    "Exit the game?",
    new PopupButtonParameters("Cancel", OnCancel),
    new PopupButtonParameters("Exit", OnExit));
```

Что сделано:

- `MessagePopupParameters` задаёт title, body и 1-5 кнопок.
- `PopupButtonParameters` хранит текст и callback.
- `PopupView` открывает serialized ссылки на uGUI/TMP-компоненты.
- `PopupPresenter` применяет параметры и создаёт runtime-кнопки.
- `PopupPresenterBase` проверяет типы view/parameters и даёт `ClosedAsync`.

Ожидаемые компоненты prefab: `CanvasGroup`, `Image`, `TextMeshProUGUI`, `Button`, `PopupButtonView`, `VerticalLayoutGroup`/`HorizontalLayoutGroup`, `ContentSizeFitter`, `RectTransform` anchors. `ContentSizeFitter` нужен для автоматического расширения контейнеров под переменный контент, например под высоту body-текста.

## 4. UI Performance Refactoring

Исправлено:

- `[SerializedField]` -> `[SerializeField]`.
- `List.Length` -> `Count`.
- `GetComponents<Character>()` и `GetComponent<Text>()` в горячем пути убраны.
- Среднее значение считается как `total / count`.
- Пустой список и `null`-ссылки обработаны безопасно.
- UI больше не обновляется в `FixedUpdate`.
- Обновление ограничено `_updateEveryFrames` и `_updateIntervalSeconds`.
- `Debug.Log` выключен по умолчанию.

## 5. Gameplay / State Logic

`ActiveEntitiesSystem` хранит registered/active entity без сканирования сцены.

- `GameplayEntity` отдаёт `Enabled`, `Disabled`, `Destroyed`.
- `EntityTrackingSystem` регистрирует entity, подписывается на lifecycle и обновляет active set.
- `IEntityQuery` возвращает только активные entity, в том числе по `GameplayEntityKind`.
- `OnDisable` убирает entity из active set, `OnDestroy` снимает с регистрации.

Внешняя фабрика prefab должна вызвать `IEntityRegistry.Register(entity)` после создания объекта.

## External Code

Часть integration-кода намеренно не реализована, потому что она зависит от конкретного проекта, сцен, Addressables/Resources, DI bootstrap и UI-навигации.

## Scaling Notes

Если развивать это в production-системы, я бы сначала закрывал реальные точки роста:

- `SaveSystem`: добавить versioned migrations и tests на совместимость старых сохранений.
- `PopupSystem`: вынести загрузку prefab, очередь, приоритеты и результат popup во внешний UI service.
- `ActiveEntitiesSystem`: добавить индексы только под реальные запросы gameplay-кода, например по сцене, зоне или фракции.
- UI performance: проверять изменения в Unity Profiler по CPU, GC Alloc и UI rebuilds на целевой платформе, а не оптимизировать вслепую.
