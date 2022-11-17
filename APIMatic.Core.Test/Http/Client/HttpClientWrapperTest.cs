using System.Threading.Tasks;
using APIMatic.Core.Http.Client;
using NUnit.Framework;
using System.Net.Http;
using System.Text;
using System.Net;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace APIMatic.Core.Test.Http.Client
{
    [TestFixture]
    public class HttpClientWrapperTest : TestBase
    {
        private IHttpClient _client;

        [SetUp]
        public void SetupHttpClient()
        {
            _client = LazyGlobalConfiguration.Value.HttpClient;
        }

        [Test]
        public async Task HttpClient_GetCall_200Response()
        {
            var request = LazyGlobalConfiguration.Value.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/httpclient/get/200")
                .Parameters(p => p
                    .Body(b => b.Setup("Get Response")))
                .Build();

            var content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");

            handlerMock.When(request.QueryUrl)
                                .Respond(HttpStatusCode.OK, content);

            var response = await _client.ExecuteAsync(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.OK);
            Assert.AreEqual(response.Body, request.Body);
        }

        [Test]
        public async Task TestHttpClientGetCall_400Response()
        {
            var request = LazyGlobalConfiguration.Value.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/httpclient/get/400")
                .Parameters(p => p
                    .Body(b => b.Setup("Get Bad Request")))
                .Build();

            var content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");

            handlerMock.When(request.QueryUrl)
                                .Respond(HttpStatusCode.BadRequest, content);

            var response = await _client.ExecuteAsync(request);

            Assert.AreEqual(response.StatusCode, (int)HttpStatusCode.BadRequest);
            Assert.AreEqual(response.Body, request.Body);
        }
    }
}
