using System;

namespace MobaDSL.Runtime
{
    public enum GameplayEventKind
    {
        DamageApplied,
        Healed,
        ResourceSpent,
        EntityDefeated,
        EntitySpawned,
        EntityRemoved
    }

    [Serializable]
    public abstract class GameplayEvent
    {
        public GameplayEventKind Kind { get; }
        public int Tick { get; }

        protected GameplayEvent(GameplayEventKind kind, int tick)
        {
            Kind = kind;
            Tick = tick;
        }
    }

    [Serializable]
    public class DamageAppliedEvent : GameplayEvent
    {
        public EntityId Attacker { get; }
        public EntityId Target { get; }
        public FixedValue Amount { get; }
        public bool IsFatal { get; }

        public DamageAppliedEvent(int tick, EntityId attacker, EntityId target, FixedValue amount, bool isFatal)
            : base(GameplayEventKind.DamageApplied, tick)
        {
            Attacker = attacker;
            Target = target;
            Amount = amount;
            IsFatal = isFatal;
        }

        public override string ToString() => $"[Tick {Tick}] Damage: {Attacker} -> {Target} for {Amount} (Fatal: {IsFatal})";
    }

    [Serializable]
    public class HealedEvent : GameplayEvent
    {
        public EntityId Source { get; }
        public EntityId Target { get; }
        public FixedValue Amount { get; }

        public HealedEvent(int tick, EntityId source, EntityId target, FixedValue amount)
            : base(GameplayEventKind.Healed, tick)
        {
            Source = source;
            Target = target;
            Amount = amount;
        }

        public override string ToString() => $"[Tick {Tick}] Heal: {Source} -> {Target} for {Amount}";
    }

    [Serializable]
    public class ResourceSpentEvent : GameplayEvent
    {
        public EntityId Entity { get; }
        public StatType Resource { get; }
        public FixedValue Amount { get; }

        public ResourceSpentEvent(int tick, EntityId entity, StatType resource, FixedValue amount)
            : base(GameplayEventKind.ResourceSpent, tick)
        {
            Entity = entity;
            Resource = resource;
            Amount = amount;
        }

        public override string ToString() => $"[Tick {Tick}] Resource: {Entity} spent {Amount} {Resource}";
    }

    [Serializable]
    public class EntityDefeatedEvent : GameplayEvent
    {
        public EntityId Entity { get; }
        public EntityId Killer { get; }

        public EntityDefeatedEvent(int tick, EntityId entity, EntityId killer)
            : base(GameplayEventKind.EntityDefeated, tick)
        {
            Entity = entity;
            Killer = killer;
        }

        public override string ToString() => $"[Tick {Tick}] Defeated: {Entity} killed by {Killer}";
    }

    [Serializable]
    public class EntitySpawnedEvent : GameplayEvent
    {
        public EntityId Entity { get; }
        public TeamId Team { get; }

        public EntitySpawnedEvent(int tick, EntityId entity, TeamId team)
            : base(GameplayEventKind.EntitySpawned, tick)
        {
            Entity = entity;
            Team = team;
        }

        public override string ToString() => $"[Tick {Tick}] Spawned: {Entity} on Team {Team}";
    }

    [Serializable]
    public class EntityRemovedEvent : GameplayEvent
    {
        public EntityId Entity { get; }

        public EntityRemovedEvent(int tick, EntityId entity)
            : base(GameplayEventKind.EntityRemoved, tick)
        {
            Entity = entity;
        }

        public override string ToString() => $"[Tick {Tick}] Removed: {Entity}";
    }
}
