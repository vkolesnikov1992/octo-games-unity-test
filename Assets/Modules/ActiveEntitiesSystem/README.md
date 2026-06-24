# ActiveEntitiesSystem

Registry/query для gameplay entity без `FindObjectsOfType` и сканирования сцены.

## API

- `IGameplayEntity` - контракт сценового объекта.
- `IEntityRegistry` регистрирует entity, снимает регистрацию и меняет active state.
- `IEntityQuery` отдаёт registered/active entity и поиск по id/kind.
- `IEntityEvents` публикует события регистрации и active state.

## Flow

1. Внешняя prefab factory создаёт объект и вызывает `IEntityRegistry.Register(entity)`.
2. `EntityTrackingSystem` подписывается на `Enabled`, `Disabled`, `Destroyed`.
3. `OnEnable` добавляет entity в active set.
4. `OnDisable` убирает entity из active set.
5. `OnDestroy` снимает entity с регистрации.

`EntityTrackingSystem` хранит `HashSet` registered/active entity и индекс по `EntityId`. Дубли id считаются ошибкой конфигурации.

## Границы

Автоматический поиск объектов в сцене намеренно не используется. Регистрация, lifetime prefab и интеграция с gameplay factory ожидаются как внешний код проекта.
