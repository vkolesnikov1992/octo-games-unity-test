using System;
using System.Collections.Generic;

namespace OctoGamesTest.ActiveEntitiesSystem
{
    public sealed class EntityTrackingSystem : IEntityRegistry, IEntityQuery, IEntityEvents, IDisposable
    {
        private static readonly IGameplayEntity[] EmptyEntities = Array.Empty<IGameplayEntity>();

        private readonly HashSet<IGameplayEntity> _registeredEntities = new();
        private readonly HashSet<IGameplayEntity> _activeEntities = new();
        private readonly Dictionary<int, IGameplayEntity> _entitiesById = new();
        private readonly Dictionary<GameplayEntityKind, HashSet<IGameplayEntity>> _activeByKind = new();

        public event Action<IGameplayEntity> EntityRegistered;
        public event Action<IGameplayEntity> EntityUnregistered;
        public event Action<IGameplayEntity> EntityActivated;
        public event Action<IGameplayEntity> EntityDeactivated;

        public IReadOnlyCollection<IGameplayEntity> RegisteredEntities => _registeredEntities;

        public IReadOnlyCollection<IGameplayEntity> ActiveEntities => _activeEntities;

        public bool Register(IGameplayEntity entity)
        {
            ValidateEntity(entity);

            if (_registeredEntities.Contains(entity))
            {
                return true;
            }

            if (_entitiesById.ContainsKey(entity.EntityId))
            {
                throw new ArgumentException(
                    $"Entity id '{entity.EntityId}' is already registered.",
                    nameof(entity));
            }

            _registeredEntities.Add(entity);
            _entitiesById.Add(entity.EntityId, entity);
            Subscribe(entity);
            EntityRegistered?.Invoke(entity);

            if (entity.IsEnabledInHierarchy)
            {
                AddActive(entity);
            }

            return true;
        }

        public bool Unregister(IGameplayEntity entity)
        {
            ValidateEntity(entity);

            if (!_registeredEntities.Remove(entity))
            {
                return false;
            }

            Unsubscribe(entity);
            RemoveActive(entity);
            _entitiesById.Remove(entity.EntityId);
            EntityUnregistered?.Invoke(entity);
            return true;
        }

        public bool SetActive(IGameplayEntity entity, bool isActive)
        {
            ValidateEntity(entity);

            if (!_registeredEntities.Contains(entity))
            {
                return false;
            }

            return isActive ? AddActive(entity) : RemoveActive(entity);
        }

        public bool TryGetEntity(int entityId, out IGameplayEntity entity)
        {
            return _entitiesById.TryGetValue(entityId, out entity);
        }

        public IReadOnlyCollection<IGameplayEntity> GetActive(GameplayEntityKind kind)
        {
            if (_activeByKind.TryGetValue(kind, out var entities))
            {
                return entities;
            }

            return EmptyEntities;
        }

        public void Dispose()
        {
            foreach (var entity in _registeredEntities)
            {
                Unsubscribe(entity);
            }

            _registeredEntities.Clear();
            _activeEntities.Clear();
            _entitiesById.Clear();
            _activeByKind.Clear();

            EntityRegistered = null;
            EntityUnregistered = null;
            EntityActivated = null;
            EntityDeactivated = null;
        }

        private void Subscribe(IGameplayEntity entity)
        {
            entity.Enabled += OnEntityEnabled;
            entity.Disabled += OnEntityDisabled;
            entity.Destroyed += OnEntityDestroyed;
        }

        private void Unsubscribe(IGameplayEntity entity)
        {
            entity.Enabled -= OnEntityEnabled;
            entity.Disabled -= OnEntityDisabled;
            entity.Destroyed -= OnEntityDestroyed;
        }

        private void OnEntityEnabled(IGameplayEntity entity)
        {
            SetActive(entity, true);
        }

        private void OnEntityDisabled(IGameplayEntity entity)
        {
            SetActive(entity, false);
        }

        private void OnEntityDestroyed(IGameplayEntity entity)
        {
            Unregister(entity);
        }

        private bool AddActive(IGameplayEntity entity)
        {
            if (!_activeEntities.Add(entity))
            {
                return false;
            }

            if (!_activeByKind.TryGetValue(entity.Kind, out var entities))
            {
                entities = new HashSet<IGameplayEntity>();
                _activeByKind.Add(entity.Kind, entities);
            }

            entities.Add(entity);
            EntityActivated?.Invoke(entity);
            return true;
        }

        private bool RemoveActive(IGameplayEntity entity)
        {
            if (!_activeEntities.Remove(entity))
            {
                return false;
            }

            if (_activeByKind.TryGetValue(entity.Kind, out var entities))
            {
                entities.Remove(entity);
            }

            EntityDeactivated?.Invoke(entity);
            return true;
        }

        private static void ValidateEntity(IGameplayEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.EntityId == 0)
            {
                throw new ArgumentException("Entity id cannot be zero.", nameof(entity));
            }
        }
    }
}
