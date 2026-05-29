using System;

namespace MobaDSL.Runtime
{
    [Serializable]
    public class ResourcePool
    {
        public StatType MaxStat { get; }
        public FixedValue Current { get; private set; }

        public ResourcePool(StatType maxStat, FixedValue initialValue)
        {
            MaxStat = maxStat;
            Current = initialValue;
        }

        public void SetCurrentDirect(FixedValue value, StatCollection stats)
        {
            FixedValue maxVal = stats.GetVal(MaxStat);
            Current = FixedValue.Clamp(value, FixedValue.Zero, maxVal);
        }

        public bool TrySpend(FixedValue amount)
        {
            if (amount.RawValue < 0) return false; // Prevent negative spending which behaves like healing
            if (Current >= amount)
            {
                Current -= amount;
                return true;
            }
            return false;
        }

        public void Modify(FixedValue delta, StatCollection stats)
        {
            FixedValue maxVal = stats.GetVal(MaxStat);
            FixedValue newVal = Current + delta;
            Current = FixedValue.Clamp(newVal, FixedValue.Zero, maxVal);
        }

        public FixedValue GetCurrentClamped(StatCollection stats)
        {
            FixedValue maxVal = stats.GetVal(MaxStat);
            if (Current > maxVal)
            {
                Current = maxVal;
            }
            return Current;
        }
    }
}
