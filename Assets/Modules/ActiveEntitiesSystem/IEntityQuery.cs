using System.Collections.Generic;

namespace OctoGamesTest.ActiveEntitiesSystem
{
    public interface IEntityQuery
    {
        IReadOnlyCollection<IGameplayEntity> RegisteredEntities { get; }

        IReadOnlyCollection<IGameplayEntity> ActiveEntities { get; }

        bool TryGetEntity(int entityId, out IGameplayEntity entity);

        IReadOnlyCollection<IGameplayEntity> GetActive(GameplayEntityKind kind);
    }
}
