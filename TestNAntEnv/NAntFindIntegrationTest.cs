using Microsoft.VisualStudio.TestTools.UnitTesting;
using NAntEnv;
using TestNAntEnv;

namespace TestNAntEnv
{
    [TestClass]
    public class NAntFindIntegrationTest
    {
        readonly LoadEnvTask reference = new LoadEnvTask();

        [TestMethod]
        public void should_find_visual_studio_110_from_registry()
        {
            NAnt.Assert(@"TestLoadEnvironment.build");
        }
    }
}
