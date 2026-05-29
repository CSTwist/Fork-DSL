using System;
using System.Collections.Generic;

namespace MobaDSL.Runtime
{
    [Serializable]
    public class EntityRegistry
    {
        private readonly Dictionary<EntityId, CombatEntityState> _entities = new Dictionary<EntityId, CombatEntityState>();
        private int _nextEntityId = 1;

        public EntityId GenerateUniqueId()
        {
            return new EntityId(_nextEntityId++);
        }

        public void Register(CombatEntityState entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (_entities.ContainsKey(entity.Id))
            {
                throw new InvalidOperationException($"Entity with ID {entity.Id} is already registered.");
            }
            _entities[entity.Id] = entity;
        }

        public bool Unregister(EntityId id)
        {
            return _entities.Remove(id);
        }

        public CombatEntityState GetEntity(EntityId id)
        {
            return _entities.TryGetValue(id, out var entity) ? entity : null;
        }

        public IEnumerable<CombatEntityState> AllEntities => _entities.Values;

        public void Clear()
        {
            _entities.Clear();
            _nextEntityId = 1;
        }
    }
}
