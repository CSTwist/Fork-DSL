using System;
using System.Collections.Generic;

namespace MobaDSL.Runtime
{
    public enum ModifierType
    {
        FlatAdd = 0,
        PercentAdd = 1
    }

    [Serializable]
    public class StatModifier
    {
        public StatType Type { get; }
        public ModifierType ModType { get; }
        public FixedValue Value { get; }
        public object Source { get; }

        public StatModifier(StatType type, ModifierType modType, FixedValue value, object source)
        {
            Type = type;
            ModType = modType;
            Value = value;
            Source = source;
        }
    }

    [Serializable]
    public class StatBlock
    {
        private readonly Dictionary<StatType, FixedValue> _baseValues = new Dictionary<StatType, FixedValue>();

        public void SetBase(StatType type, FixedValue value)
        {
            _baseValues[type] = value;
        }

        public FixedValue GetBase(StatType type)
        {
            return _baseValues.TryGetValue(type, out var val) ? val : FixedValue.Zero;
        }
    }

    [Serializable]
    public class StatCollection
    {
        private readonly StatBlock _baseStats;
        private readonly List<StatModifier> _modifiers = new List<StatModifier>();

        public StatCollection(StatBlock baseStats)
        {
            _baseStats = baseStats ?? throw new ArgumentNullException(nameof(baseStats));
        }

        public void AddModifier(StatModifier modifier)
        {
            if (modifier == null) return;
            _modifiers.Add(modifier);
        }

        public void RemoveModifiersFromSource(object source)
        {
            if (source == null) return;
            _modifiers.RemoveAll(m => m.Source == source);
        }

        public FixedValue GetVal(StatType stat)
        {
            FixedValue baseVal = _baseStats.GetBase(stat);
            long flatAdd = 0;
            long percentAdd = 0; // scaled by 1000, so +10% is 100

            foreach (var mod in _modifiers)
            {
                if (mod.Type != stat) continue;

                if (mod.ModType == ModifierType.FlatAdd)
                {
                    flatAdd += mod.Value.RawValue;
                }
                else if (mod.ModType == ModifierType.PercentAdd)
                {
                    percentAdd += mod.Value.RawValue;
                }
            }

            FixedValue flatSum = FixedValue.FromRaw(baseVal.RawValue + flatAdd);
            FixedValue percentFactor = FixedValue.FromRaw(FixedValue.Scale + percentAdd);

            FixedValue finalVal = flatSum * percentFactor;
            // Floor at zero for sanity (e.g. speed, armor, damage shouldn't generally go negative in base simulation)
            return FixedValue.Max(FixedValue.Zero, finalVal);
        }
    }
}
