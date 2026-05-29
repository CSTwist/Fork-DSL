using NUnit.Framework;
using MobaDSL.Runtime;
using MobaDSL.Editor;

namespace MobaDSL.Tests
{
    [TestFixture]
    public class EditorSmokeTest
    {
        [Test]
        public void TestEditorNamespace_IsAvailable()
        {
            Assert.Contains("MobaDSL Compiler", EditorNamespaceDoc.Description);
        }

        [Test]
        public void TestEditorCanReferenceRuntime()
        {
            // Verify compiler/editor assembly can see the Runtime assembly
            string runtimeVersion = NamespaceDoc.Version;
            Assert.AreEqual("0.1.0", runtimeVersion);
        }
    }
}
