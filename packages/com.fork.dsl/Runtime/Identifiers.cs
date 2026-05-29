using System;

namespace MobaDSL.Runtime
{
    [Serializable]
    public readonly struct EntityId : IEquatable<EntityId>
    {
        public readonly int Value;

        public EntityId(int value)
        {
            Value = value;
        }

        public static EntityId Invalid => new EntityId(-1);

        public bool Equals(EntityId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EntityId other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => $"Entity({Value})";

        public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);
        public static bool operator !=(EntityId left, EntityId right) => !left.Equals(right);
    }

    [Serializable]
    public readonly struct HeroId : IEquatable<HeroId>
    {
        public readonly string Value;

        public HeroId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool Equals(HeroId other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        public override bool Equals(object obj) => obj is HeroId other && Equals(other);
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
        public override string ToString() => Value;

        public static bool operator ==(HeroId left, HeroId right) => left.Equals(right);
        public static bool operator !=(HeroId left, HeroId right) => !left.Equals(right);
    }

    [Serializable]
    public readonly struct AbilityId : IEquatable<AbilityId>
    {
        public readonly string Value;

        public AbilityId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool Equals(AbilityId other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        public override bool Equals(object obj) => obj is AbilityId other && Equals(other);
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
        public override string ToString() => Value;

        public static bool operator ==(AbilityId left, AbilityId right) => left.Equals(right);
        public static bool operator !=(AbilityId left, AbilityId right) => !left.Equals(right);
    }

    [Serializable]
    public readonly struct StatusId : IEquatable<StatusId>
    {
        public readonly string Value;

        public StatusId(string value)
        {
            Value = value ?? string.Empty;
        }

        public bool Equals(StatusId other) => string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
        public override bool Equals(object obj) => obj is StatusId other && Equals(other);
        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
        public override string ToString() => Value;

        public static bool operator ==(StatusId left, StatusId right) => left.Equals(right);
        public static bool operator !=(StatusId left, StatusId right) => !left.Equals(right);
    }
}
