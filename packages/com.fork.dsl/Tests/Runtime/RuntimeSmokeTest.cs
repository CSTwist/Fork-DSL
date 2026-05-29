using NUnit.Framework;
using MobaDSL.Runtime;

namespace MobaDSL.Tests
{
    [TestFixture]
    public class RuntimeSmokeTest
    {
        [Test]
        public void TestRuntimeVersion_IsCorrect()
        {
            Assert.AreEqual("0.1.0", NamespaceDoc.Version);
        }

        [Test]
        public void TestDeterministicAssumption_IntegerTicks()
        {
            // Simple validation that tick measurements behave predictably
            int startTick = 0;
            int ticksElapsed = 20; // 1 second at 20 ticks/sec
            int currentTick = startTick + ticksElapsed;

            Assert.AreEqual(20, currentTick);
        }
    }
}
