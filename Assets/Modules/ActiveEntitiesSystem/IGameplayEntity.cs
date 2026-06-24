using System;
using UnityEngine;

namespace OctoGamesTest.ActiveEntitiesSystem
{
    public interface IGameplayEntity
    {
        event Action<IGameplayEntity> Enabled;

        event Action<IGameplayEntity> Disabled;

        event Action<IGameplayEntity> Destroyed;

        int EntityId { get; }

        GameObject GameObject { get; }

        Transform Transform { get; }

        GameplayEntityKind Kind { get; }

        bool IsEnabledInHierarchy { get; }
    }
}
