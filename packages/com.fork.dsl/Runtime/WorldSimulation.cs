using System;
using System.Collections.Generic;

namespace MobaDSL.Runtime
{
    [Serializable]
    public class WorldSimulation
    {
        public SimulationConfig Config { get; }
        public EntityRegistry Registry { get; }
        public GameplayEventStream EventStream { get; }
        public int CurrentTick { get; private set; }

        public WorldSimulation(SimulationConfig config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            Registry = new EntityRegistry();
            EventStream = new GameplayEventStream();
            CurrentTick = 0;
        }

        public EntityId SpawnEntity(HeroId heroId, TeamId team, StatBlock baseStats, Vector2Fixed position)
        {
            EntityId id = Registry.GenerateUniqueId();
            var state = new CombatEntityState(id, team, baseStats, position);
            Registry.Register(state);

            EventStream.Emit(new EntitySpawnedEvent(CurrentTick, id, team));
            return id;
        }

        public void DespawnEntity(EntityId id)
        {
            if (Registry.Unregister(id))
            {
                EventStream.Emit(new EntityRemovedEvent(CurrentTick, id));
            }
        }

        public void Tick()
        {
            CurrentTick++;
            
            // Execute any tick-based regenerations or updates
            foreach (var entity in Registry.AllEntities)
            {
                if (entity.IsDead) continue;

                // Future extension: Apply tick regenerations, cooldown reductions, or status effect updates
            }
        }

        public void ApplyDamage(EntityId attackerId, EntityId targetId, FixedValue rawAmount, DamageType damageType)
        {
            CombatEntityState target = Registry.GetEntity(targetId);
            if (target == null || target.IsDead) return;

            FixedValue finalAmount = rawAmount;

            if (damageType == DamageType.Physical)
            {
                FixedValue armor = target.Stats.GetVal(StatType.Armor);
                FixedValue hund = FixedValue.FromInt(100);
                FixedValue factor = hund / (hund + armor);
                finalAmount = rawAmount * factor;
            }
            else if (damageType == DamageType.Magical)
            {
                FixedValue mr = target.Stats.GetVal(StatType.MagicResist);
                FixedValue hund = FixedValue.FromInt(100);
                FixedValue factor = hund / (hund + mr);
                finalAmount = rawAmount * factor;
            }

            // Apply damage to health
            target.Health.Modify(FixedValue.Zero - finalAmount, target.Stats);
            bool isFatal = target.IsDead;

            EventStream.Emit(new DamageAppliedEvent(CurrentTick, attackerId, targetId, finalAmount, isFatal));

            if (isFatal)
            {
                EventStream.Emit(new EntityDefeatedEvent(CurrentTick, targetId, attackerId));
                HandleDeath(targetId);
            }
        }

        public void ApplyHeal(EntityId sourceId, EntityId targetId, FixedValue amount)
        {
            CombatEntityState target = Registry.GetEntity(targetId);
            if (target == null || target.IsDead) return;

            target.Health.Modify(amount, target.Stats);
            EventStream.Emit(new HealedEvent(CurrentTick, sourceId, targetId, amount));
        }

        public void SpendMana(EntityId entityId, FixedValue amount)
        {
            CombatEntityState entity = Registry.GetEntity(entityId);
            if (entity == null || entity.IsDead) return;

            if (entity.Mana.TrySpend(amount))
            {
                EventStream.Emit(new ResourceSpentEvent(CurrentTick, entityId, StatType.MaxMana, amount));
            }
        }

        private void HandleDeath(EntityId id)
        {
            // Death handling stub (e.g. trigger respawn timers or schedule despawn)
        }

        public void RespawnEntity(EntityId id, Vector2Fixed respawnPosition)
        {
            CombatEntityState entity = Registry.GetEntity(id);
            if (entity == null || !entity.IsDead) return;

            // Fully restore health/mana and relocate
            entity.Position = respawnPosition;
            FixedValue maxHealth = entity.Stats.GetVal(StatType.MaxHealth);
            FixedValue maxMana = entity.Stats.GetVal(StatType.MaxMana);

            entity.Health.SetCurrentDirect(maxHealth, entity.Stats);
            entity.Mana.SetCurrentDirect(maxMana, entity.Stats);

            EventStream.Emit(new EntitySpawnedEvent(CurrentTick, id, entity.Team));
        }
    }
}
