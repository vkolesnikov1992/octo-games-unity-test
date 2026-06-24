# RefactoringTask

Рефакторинг проблемного live UI из ТЗ.

## Что было исправлено

- Неверные Unity/API вызовы: `[SerializedField]`, `List.Length`, `GetComponents<Character>()`.
- Неверная формула среднего значения.
- Деление на ноль и отсутствие обработки пустого списка.
- `GetComponent` и форматирование текста в `FixedUpdate`.
- Постоянный `Debug.Log` в горячем UI-пути.

## Текущий подход

- `Character.Value` вызывает `ValueChanged` только при реальном изменении.
- `CharactersView` получает `Character` и `TextMeshProUGUI` через serialized fields.
- View подписывается в `Awake` и отписывается в `OnDestroy`.
- Событие только ставит dirty-флаг.
- `Update` обновляет UI не чаще `_updateEveryFrames` и `_updateIntervalSeconds`.
- Среднее считается как `totalValue / count`.
- `null`-ссылки в списке пропускаются.
- Логирование включается только через `_logRefreshes`.

В production-коде для такого экрана я бы полностью отказался от `Update` и менял UI только по событию изменения данных. Здесь `Update` оставлен осознанно, потому что в ТЗ отдельно указано ограничить UI-обновления раз в X frames или по fixed interval.

Сканирование сцены и gameplay state management находятся вне этой задачи.
