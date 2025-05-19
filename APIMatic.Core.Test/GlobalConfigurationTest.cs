using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIMatic.Core.Http.Configuration;
using NUnit.Framework;

namespace APIMatic.Core.Test
{
    public class GlobalConfigurationTests : TestBase
    {
        [Test]
        public async Task TestGlobalRequestUrl()
        {
            var request = await LazyGlobalConfiguration.Value.GlobalRequestBuilder().Build();
            Assert.AreEqual("http://my/path:3000/v1", request.QueryUrl);

            var request2 = await LazyGlobalConfiguration.Value.GlobalRequestBuilder(MockServer.Server2).Build();
            Assert.AreEqual("https://my/path/v2", request2.QueryUrl);
        }

        [Test]
        public async Task TestGlobalRequestHeaders()
        {
            var request = await LazyGlobalConfiguration.Value.GlobalRequestBuilder().Build();
            Assert.True(request.Headers.Count == 5);
            Assert.AreEqual("headVal1", request.Headers["additionalHead1"]);
            Assert.AreEqual("headVal2", request.Headers["additionalHead2"]);
            Assert.AreEqual("text/plain; charset=utf-8", request.Headers["content-type"]);
            Assert.AreEqual("890.098", request.Headers["key5"]);
            Assert.True(request.Headers["user-agent"].StartsWith("my lang|1.*.*|"));
        }

        [Test]
        public async Task TestGlobalRequest_NullUserAgent()
        {
            var httpClientConfiguration = new CoreHttpClientConfiguration.Builder().Build();
            var request = await new GlobalConfiguration.Builder()
                .HttpConfiguration(httpClientConfiguration)
                .UserAgent(null)
                .ServerUrls(new Dictionary<Enum, string>
                {
                    { MockServer.Server1, "http://my/path:3000/{one}" },
                    { MockServer.Server2, "https://my/path/{two}" }
                }, MockServer.Server1)
                .Build()
                .GlobalRequestBuilder()
                .Build();

            Assert.IsFalse(request.Headers.ContainsKey("user-agent"));
        }
    }
}
