using System;

namespace MobaDSL.Runtime
{
    [Serializable]
    public struct Vector2Fixed : IEquatable<Vector2Fixed>
    {
        public FixedValue X;
        public FixedValue Y;

        public Vector2Fixed(FixedValue x, FixedValue y)
        {
            X = x;
            Y = y;
        }

        public static Vector2Fixed Zero => new Vector2Fixed(FixedValue.Zero, FixedValue.Zero);

        public static Vector2Fixed operator +(Vector2Fixed a, Vector2Fixed b)
        {
            return new Vector2Fixed(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2Fixed operator -(Vector2Fixed a, Vector2Fixed b)
        {
            return new Vector2Fixed(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2Fixed operator *(Vector2Fixed a, FixedValue d)
        {
            return new Vector2Fixed(a.X * d, a.Y * d);
        }

        public static Vector2Fixed operator /(Vector2Fixed a, FixedValue d)
        {
            return new Vector2Fixed(a.X / d, a.Y / d);
        }

        public static bool operator ==(Vector2Fixed a, Vector2Fixed b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector2Fixed a, Vector2Fixed b) => a.X != b.X || a.Y != b.Y;

        public bool Equals(Vector2Fixed other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Vector2Fixed other && Equals(other);
        public override int GetHashCode() => (X.GetHashCode() * 397) ^ Y.GetHashCode();

        public FixedValue LengthSquared()
        {
            return (X * X) + (Y * Y);
        }

        public static FixedValue DistanceSquared(Vector2Fixed a, Vector2Fixed b)
        {
            FixedValue dx = a.X - b.X;
            FixedValue dy = a.Y - b.Y;
            return (dx * dx) + (dy * dy);
        }

        public static FixedValue Dot(Vector2Fixed a, Vector2Fixed b)
        {
            return (a.X * b.X) + (a.Y * b.Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}
