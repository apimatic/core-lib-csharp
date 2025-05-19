using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Http;
using APIMatic.Core.Http.Configuration;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Http
{
    [TestFixture]
    public class HttpClientWrapperTest : TestBase
    {
        private HttpClientWrapper _client;
        private GlobalConfiguration _config;
        [SetUp]
        public void SetupHttpClient()
        {
            var clientConfiguration = new CoreHttpClientConfiguration.Builder()
                .HttpClientInstance(new HttpClient(handlerMock), false)
                .Build();

            _config = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    { MockServer.Server1, "http://my/path:3000/{one}"},
                }, MockServer.Server1)
                .HttpConfiguration(clientConfiguration)
                .ApiCallback(ApiCallBack)
                .Build();
            _client = _config.HttpClient;
        }
        [Test]
        public async Task HttpClient_GetCall_200Response()
        {
            var request = await _config.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/httpclient/get/200")
                .Parameters(p => p
                    .Body(b => b.Setup("Get Response")))
                .Build();

            var content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");

            handlerMock.When(request.QueryUrl)
                                .Respond(HttpStatusCode.OK, content);
            var actual = await _client.ExecuteAsync(request);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.AreEqual(actual.Body, request.Body);
        }
        [Test]
        public async Task TestHttpClientGetCall_400Response()
        {
            var request = await _config.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/httpclient/get/400")
                .Parameters(p => p
                    .Body(b => b.Setup("Get Bad Request")))
                .Build();

            var content = new StringContent(request.Body.ToString(), Encoding.UTF8, "application/json");

            handlerMock.When(request.QueryUrl)
                                .Respond(HttpStatusCode.BadRequest, content);
            var actual = await _client.ExecuteAsync(request);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, actual.StatusCode);
            Assert.AreEqual(actual.Body, request.Body);
        }
        [Test]
        public async Task TestHttpClientGetCall_200Response()
        {
            var inputString = "Test with combined response headers.";
            var customHeaderKey = "Custom-Headder";
            var customHeaderValue = "customHeader";
            var request = await _config.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/httpclient/get-combined-headers/200")
                .Build();
            var content = JsonContent.Create(inputString);
            content.Headers.Add(customHeaderKey, customHeaderValue);
            handlerMock.When(request.QueryUrl)
                    .Respond(_ =>
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = content
                        };
                        response.Headers.Add(customHeaderKey, customHeaderValue);
                        return response;
                    });
            // Act
            var actual = await _client.ExecuteAsync(request);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.AreEqual(customHeaderValue, actual.Headers[customHeaderKey]);
        }
    }
}
