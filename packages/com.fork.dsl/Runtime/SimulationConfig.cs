using System;

namespace MobaDSL.Runtime
{
    [Serializable]
    public class SimulationConfig
    {
        public int TicksPerSecond { get; set; } = 20;

        public FixedValue TickDuration => FixedValue.One / FixedValue.FromInt(TicksPerSecond);
    }
}
