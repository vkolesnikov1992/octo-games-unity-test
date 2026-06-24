using System;
using UnityEngine;

namespace OctoGamesTest.ActiveEntitiesSystem
{
    public sealed class GameplayEntity : MonoBehaviour, IGameplayEntity
    {
        [SerializeField]
        private int _entityId;

        [SerializeField]
        private GameplayEntityKind _kind;

        private int _resolvedEntityId;

        public event Action<IGameplayEntity> Enabled;

        public event Action<IGameplayEntity> Disabled;

        public event Action<IGameplayEntity> Destroyed;

        public int EntityId
        {
            get
            {
                ResolveEntityId();
                return _resolvedEntityId;
            }
        }

        public GameObject GameObject => gameObject;

        public Transform Transform => transform;

        public GameplayEntityKind Kind => _kind;

        public bool IsEnabledInHierarchy => isActiveAndEnabled;

        private void Awake()
        {
            ResolveEntityId();
        }

        private void OnEnable()
        {
            Enabled?.Invoke(this);
        }

        private void OnDisable()
        {
            Disabled?.Invoke(this);
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }

        private void ResolveEntityId()
        {
            if (_resolvedEntityId != 0)
            {
                return;
            }

            _resolvedEntityId = _entityId == 0 ? GetInstanceID() : _entityId;
        }
    }
}
