using System;

namespace OctoGamesTest.ActiveEntitiesSystem
{
    public interface IEntityEvents
    {
        event Action<IGameplayEntity> EntityRegistered;

        event Action<IGameplayEntity> EntityUnregistered;

        event Action<IGameplayEntity> EntityActivated;

        event Action<IGameplayEntity> EntityDeactivated;
    }
}
