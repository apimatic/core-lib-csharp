using APIMatic.Core.Request;
using APIMatic.Core.Types;
using Moq;
using NUnit.Framework;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class GlobalConfigurationTest : TestBase
    {
        [Test]
        public void TestGlobalRequestBuilder()
        {
            var mockRequestServer1 = MockRequest(queryUrl: "http://my/path:3000/{one}").Object;
            var actualRequestServer1 = GlobalConfiguration.GlobalRequestBuilder().Build();
            Assert.AreEqual(mockRequestServer1.QueryUrl, actualRequestServer1.QueryUrl);

            var mockRequestServer2 = MockRequest(queryUrl: "https://my/path/{two}").Object;
            var actualRequestServer2 = GlobalConfiguration.GlobalRequestBuilder(MockServer.Server2).Build();
            Assert.AreEqual(mockRequestServer2.QueryUrl, actualRequestServer2.QueryUrl);
        }
    }
}
