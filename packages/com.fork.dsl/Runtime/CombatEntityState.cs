using System;

namespace MobaDSL.Runtime
{
    public enum TeamId
    {
        Neutral = 0,
        Order = 1,
        Chaos = 2
    }

    [Serializable]
    public class CombatEntityState
    {
        public EntityId Id { get; }
        public TeamId Team { get; }
        public Vector2Fixed Position { get; set; }
        public Vector2Fixed Direction { get; set; }
        public StatCollection Stats { get; }
        public ResourcePool Health { get; }
        public ResourcePool Mana { get; }

        public CombatEntityState(EntityId id, TeamId team, StatBlock baseStats, Vector2Fixed spawnPosition)
        {
            Id = id;
            Team = team;
            Position = spawnPosition;
            Direction = new Vector2Fixed(FixedValue.One, FixedValue.Zero); // Face right by default
            Stats = new StatCollection(baseStats);

            // Initialize resource pools from the baseline max values resolved from the base stats
            FixedValue maxHealth = Stats.GetVal(StatType.MaxHealth);
            FixedValue maxMana = Stats.GetVal(StatType.MaxMana);

            Health = new ResourcePool(StatType.MaxHealth, maxHealth);
            Mana = new ResourcePool(StatType.MaxMana, maxMana);
        }

        public bool IsDead => Health.GetCurrentClamped(Stats) == FixedValue.Zero;
    }
}
