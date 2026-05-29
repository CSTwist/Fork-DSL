using NUnit.Framework;
using MobaDSL.Runtime;

namespace MobaDSL.Tests
{
    [TestFixture]
    public class RuntimeSmokeTest
    {
        [Test]
        public void TestFixedValueMath_AdditionSubtraction()
        {
            FixedValue a = FixedValue.FromFloat(1.5f);
            FixedValue b = FixedValue.FromFloat(2.25f);

            FixedValue sum = a + b;
            FixedValue diff = b - a;

            Assert.AreEqual(3.75f, sum.ToFloat(), 0.001f);
            Assert.AreEqual(0.75f, diff.ToFloat(), 0.001f);
        }

        [Test]
        public void TestFixedValueMath_Multiplication()
        {
            FixedValue a = FixedValue.FromFloat(2.5f);
            FixedValue b = FixedValue.FromFloat(4.0f);

            FixedValue prod = a * b;

            Assert.AreEqual(10.0f, prod.ToFloat(), 0.001f);
        }

        [Test]
        public void TestFixedValueMath_Division()
        {
            FixedValue a = FixedValue.FromFloat(10.0f);
            FixedValue b = FixedValue.FromFloat(4.0f);

            FixedValue quotient = a / b;

            Assert.AreEqual(2.5f, quotient.ToFloat(), 0.001f);
        }

        [Test]
        public void TestStatsCollection_ResolvesModifiers()
        {
            var baseStats = new StatBlock();
            baseStats.SetBase(StatType.MaxHealth, FixedValue.FromInt(500));
            baseStats.SetBase(StatType.AttackDamage, FixedValue.FromInt(50));

            var stats = new StatCollection(baseStats);

            // Base checks
            Assert.AreEqual(500, stats.GetVal(StatType.MaxHealth).ToInt());
            Assert.AreEqual(50, stats.GetVal(StatType.AttackDamage).ToInt());

            // Add Flat Mod
            object source = new object();
            stats.AddModifier(new StatModifier(StatType.AttackDamage, ModifierType.FlatAdd, FixedValue.FromInt(15), source));
            Assert.AreEqual(65, stats.GetVal(StatType.AttackDamage).ToInt());

            // Add Percent Mod (+10% is 0.10)
            stats.AddModifier(new StatModifier(StatType.AttackDamage, ModifierType.PercentAdd, FixedValue.FromFloat(0.10f), source));
            // Final = (50 + 15) * (1 + 0.10) = 65 * 1.10 = 71.5
            Assert.AreEqual(71.5f, stats.GetVal(StatType.AttackDamage).ToFloat(), 0.001f);

            // Remove modifiers from source
            stats.RemoveModifiersFromSource(source);
            Assert.AreEqual(50, stats.GetVal(StatType.AttackDamage).ToInt());
        }

        [Test]
        public void TestResourcePool_ClampingAndSpending()
        {
            var baseStats = new StatBlock();
            baseStats.SetBase(StatType.MaxMana, FixedValue.FromInt(100));
            var stats = new StatCollection(baseStats);

            var mana = new ResourcePool(StatType.MaxMana, FixedValue.FromInt(100));

            // Spend mana
            bool success = mana.TrySpend(FixedValue.FromInt(40));
            Assert.IsTrue(success);
            Assert.AreEqual(60, mana.GetCurrentClamped(stats).ToInt());

            // Overspend fails
            bool overspend = mana.TrySpend(FixedValue.FromInt(80));
            Assert.IsFalse(overspend);
            Assert.AreEqual(60, mana.GetCurrentClamped(stats).ToInt());

            // Heal mana, clamp at max
            mana.Modify(FixedValue.FromInt(100), stats);
            Assert.AreEqual(100, mana.GetCurrentClamped(stats).ToInt());
        }

        [Test]
        public void TestWorldSimulation_DamageMitigationAndDeath()
        {
            var config = new SimulationConfig { TicksPerSecond = 20 };
            var sim = new WorldSimulation(config);

            var baseAttacker = new StatBlock();
            baseAttacker.SetBase(StatType.MaxHealth, FixedValue.FromInt(100));
            baseAttacker.SetBase(StatType.MaxMana, FixedValue.FromInt(50));

            var baseTarget = new StatBlock();
            baseTarget.SetBase(StatType.MaxHealth, FixedValue.FromInt(200));
            baseTarget.SetBase(StatType.MaxMana, FixedValue.FromInt(50));
            baseTarget.SetBase(StatType.Armor, FixedValue.FromInt(100)); // 100 Armor is 50% mitigation

            EntityId attackerId = sim.SpawnEntity(new HeroId("Mage"), TeamId.Order, baseAttacker, Vector2Fixed.Zero);
            EntityId targetId = sim.SpawnEntity(new HeroId("Tank"), TeamId.Chaos, baseTarget, Vector2Fixed.Zero);

            CombatEntityState target = sim.Registry.GetEntity(targetId);

            // Apply 100 Physical Damage
            sim.ApplyDamage(attackerId, targetId, FixedValue.FromInt(100), DamageType.Physical);

            // Target has 100 Armor, so factor is 100 / (100 + 100) = 0.5. Damaged by 50. Health is 150.
            Assert.AreEqual(150, target.Health.GetCurrentClamped(target.Stats).ToInt());

            // Apply 150 True Damage (bypasses Armor)
            sim.ApplyDamage(attackerId, targetId, FixedValue.FromInt(150), DamageType.True);

            Assert.IsTrue(target.IsDead);
            Assert.AreEqual(0, target.Health.GetCurrentClamped(target.Stats).ToInt());
        }

        [Test]
        public void TestWorldSimulation_Determinism()
        {
            // Verify that running the identical tick sequence yields exact matching states
            var baseStats = new StatBlock();
            baseStats.SetBase(StatType.MaxHealth, FixedValue.FromInt(100));
            baseStats.SetBase(StatType.MaxMana, FixedValue.FromInt(50));

            var config = new SimulationConfig { TicksPerSecond = 20 };
            
            // Simulation 1
            var sim1 = new WorldSimulation(config);
            EntityId id1_A = sim1.SpawnEntity(new HeroId("H1"), TeamId.Order, baseStats, Vector2Fixed.Zero);
            EntityId id1_B = sim1.SpawnEntity(new HeroId("H2"), TeamId.Chaos, baseStats, Vector2Fixed.Zero);
            sim1.ApplyDamage(id1_A, id1_B, FixedValue.FromInt(25), DamageType.True);
            sim1.Tick();

            // Simulation 2
            var sim2 = new WorldSimulation(config);
            EntityId id2_A = sim2.SpawnEntity(new HeroId("H1"), TeamId.Order, baseStats, Vector2Fixed.Zero);
            EntityId id2_B = sim2.SpawnEntity(new HeroId("H2"), TeamId.Chaos, baseStats, Vector2Fixed.Zero);
            sim2.ApplyDamage(id2_A, id2_B, FixedValue.FromInt(25), DamageType.True);
            sim2.Tick();

            CombatEntityState e1 = sim1.Registry.GetEntity(id1_B);
            CombatEntityState e2 = sim2.Registry.GetEntity(id2_B);

            Assert.AreEqual(e1.Health.GetCurrentClamped(e1.Stats).RawValue, e2.Health.GetCurrentClamped(e2.Stats).RawValue);
            Assert.AreEqual(sim1.CurrentTick, sim2.CurrentTick);
        }
    }
}
