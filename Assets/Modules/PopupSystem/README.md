# PopupSystem

Слой содержимого popup на uGUI/TMP. Загрузка prefab, навигация, очередь popup, release/destroy instance и production bootstrap ожидаются из внешнего UI-модуля.

## API

- `PopupParameters` - базовый тип параметров.
- `MessagePopupParameters` задаёт title, body и 1-5 кнопок.
- `PopupButtonParameters` задаёт текст и callback кнопки.
- `PopupViewBase<TParameters>` связывает view с типом параметров.
- `PopupPresenterBase<TView, TParameters>` проверяет типы и управляет lifecycle.
- `IPopupPresenter.ClosedAsync` позволяет внешнему слою дождаться закрытия.

Пример:

```csharp
var parameters = new MessagePopupParameters(
    "Exit",
    "Exit the game?",
    new PopupButtonParameters("Cancel", OnCancel),
    new PopupButtonParameters("Exit", OnExit));
```

## Flow

1. Внешняя фабрика получает `PopupParameters` и выбирает prefab по runtime-типу.
2. Prefab содержит `PopupView`, помеченный `[PresenterBinding(presenter: typeof(PopupPresenter))]`.
3. Внешний слой создаёт presenter через DI и вызывает `Attach(view, parameters)`.
4. `EnterAsync` применяет текст и создаёт кнопки.
5. Кнопки выполняют callbacks. Закрытие можно инициировать во внешнем callback через `CloseAsync`/`ExitAsync`.
6. Внешний слой закрывает popup и уничтожает/release view instance.

## Prefab

Ожидаемые компоненты: `CanvasGroup`, `Image`, `TextMeshProUGUI`, `Button`, `PopupButtonView`, `VerticalLayoutGroup`/`HorizontalLayoutGroup`, `ContentSizeFitter`, `RectTransform` anchors.

`ContentSizeFitter` нужен на контейнерах, которые должны автоматически расширяться под переменный контент, например под высоту body-текста. Template кнопки можно держать выключенным; presenter включает созданные копии.
