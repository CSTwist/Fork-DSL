using System;

namespace MobaDSL.Runtime
{
    [Serializable]
    public readonly struct FixedValue : IEquatable<FixedValue>, IComparable<FixedValue>
    {
        public readonly long RawValue;
        public const long Scale = 1000;

        public FixedValue(long rawValue)
        {
            RawValue = rawValue;
        }

        public static FixedValue Zero => new FixedValue(0);
        public static FixedValue One => new FixedValue(Scale);

        public static FixedValue FromFloat(float value)
        {
            return new FixedValue((long)Math.Round(value * Scale));
        }

        public static FixedValue FromInt(int value)
        {
            return new FixedValue(value * Scale);
        }

        public static FixedValue FromRaw(long rawValue)
        {
            return new FixedValue(rawValue);
        }

        public float ToFloat()
        {
            return (float)RawValue / Scale;
        }

        public int ToInt()
        {
            return (int)(RawValue / Scale);
        }

        public static FixedValue operator +(FixedValue a, FixedValue b)
        {
            return new FixedValue(a.RawValue + b.RawValue);
        }

        public static FixedValue operator -(FixedValue a, FixedValue b)
        {
            return new FixedValue(a.RawValue - b.RawValue);
        }

        public static FixedValue operator *(FixedValue a, FixedValue b)
        {
            // Avoid overflow using 128-bit math, or cast to double for calculations, or standard integer partitioning.
            // Under 64-bit, casting intermediate multiplication to double or using system decimal is safe but let's do safe 64-bit operations.
            // Since max raw value is ~9x10^18, multiplying two raw values could overflow.
            // Let's use custom integer arithmetic or cast to double if performance permits, or checked blocks.
            // For determinism across platforms, double arithmetic is extremely reliable if rounded, but pure integer is 100% deterministic.
            // Let's do integer math:
            long absA = Math.Abs(a.RawValue);
            long absB = Math.Abs(b.RawValue);

            long aHi = absA / Scale;
            long aLo = absA % Scale;
            long bHi = absB / Scale;
            long bLo = absB % Scale;

            long result = (aHi * bHi * Scale) + (aHi * bLo) + (aLo * bHi) + ((aLo * bLo) / Scale);
            if ((a.RawValue ^ b.RawValue) < 0)
            {
                result = -result;
            }
            return new FixedValue(result);
        }

        public static FixedValue operator /(FixedValue a, FixedValue b)
        {
            if (b.RawValue == 0)
            {
                throw new DivideByZeroException("FixedValue division by zero.");
            }
            // Pure integer division
            long absA = Math.Abs(a.RawValue);
            long absB = Math.Abs(b.RawValue);

            long result = (absA * Scale) / absB;
            if ((a.RawValue ^ b.RawValue) < 0)
            {
                result = -result;
            }
            return new FixedValue(result);
        }

        public static bool operator ==(FixedValue a, FixedValue b) => a.RawValue == b.RawValue;
        public static bool operator !=(FixedValue a, FixedValue b) => a.RawValue != b.RawValue;
        public static bool operator <(FixedValue a, FixedValue b) => a.RawValue < b.RawValue;
        public static bool operator >(FixedValue a, FixedValue b) => a.RawValue > b.RawValue;
        public static bool operator <=(FixedValue a, FixedValue b) => a.RawValue <= b.RawValue;
        public static bool operator >=(FixedValue a, FixedValue b) => a.RawValue >= b.RawValue;

        public bool Equals(FixedValue other) => RawValue == other.RawValue;
        public override bool Equals(object obj) => obj is FixedValue other && Equals(other);
        public override int GetHashCode() => RawValue.GetHashCode();

        public int CompareTo(FixedValue other) => RawValue.CompareTo(other.RawValue);

        public override string ToString()
        {
            return ToFloat().ToString("F3");
        }

        public static FixedValue Min(FixedValue a, FixedValue b) => a.RawValue < b.RawValue ? a : b;
        public static FixedValue Max(FixedValue a, FixedValue b) => a.RawValue > b.RawValue ? a : b;
        public static FixedValue Clamp(FixedValue value, FixedValue min, FixedValue max) => Max(min, Min(value, max));
        public static FixedValue Abs(FixedValue value) => new FixedValue(Math.Abs(value.RawValue));
    }
}
