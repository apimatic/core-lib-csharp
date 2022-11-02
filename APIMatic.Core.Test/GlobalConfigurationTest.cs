using NUnit.Framework;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class GlobalConfigurationTest : TestBase
    {
        [Test]
        public void TestGlobalRequestBuilder()
        {
            var mockRequestServer1 = MockRequest(queryUrl: "http://my/path:3000/v1").Object;
            var actualRequestServer1 = LazyGlobalConfiguration.Value.GlobalRequestBuilder().Build();
            Assert.AreEqual(mockRequestServer1.QueryUrl, actualRequestServer1.QueryUrl);

            var mockRequestServer2 = MockRequest(queryUrl: "https://my/path/v2").Object;
            var actualRequestServer2 = LazyGlobalConfiguration.Value.GlobalRequestBuilder(MockServer.Server2).Build();
            Assert.AreEqual(mockRequestServer2.QueryUrl, actualRequestServer2.QueryUrl);
        }
    }
}
