using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APIMatic.Core.Test
{
    [TestClass]
    public class GlobalConfigurationTest
    {
        [TestMethod]
        public void TestHelloWorld()
        {
            var config = new GlobalConfiguration();
            Assert.AreEqual("Hello World", config.HelloWorld());
        }
    }
}
