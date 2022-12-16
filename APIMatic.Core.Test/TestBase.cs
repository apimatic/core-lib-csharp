using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Authentication;
using APIMatic.Core.Types;
using RichardSzalay.MockHttp;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Test.MockTypes.Authentication;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class TestBase
    {
        protected static readonly string _basicAuthUserName = "ApimaticUserName";
        protected static readonly string _basicAuthPassword = "ApimaticPassword";
        protected static HttpCallBack ApiCallBack = new HttpCallBack();
        protected enum MockServer { Server1, Server2 }
        protected static readonly int numberOfRetries = 1;

        protected Mock<CoreRequest> MockRequest(HttpMethod method = null, string queryUrl = null,
            Dictionary<string, string> headers = null, object body = null,
            List<KeyValuePair<string, object>> formParameters = null,
            Dictionary<string, object> queryParameters = null) =>
            new Mock<CoreRequest>(method, queryUrl, headers, body, formParameters, queryParameters, null, null);

        protected static readonly MockHttpMessageHandler handlerMock = new MockHttpMessageHandler()
        {
            AutoFlush = true
        };

        private static ICoreHttpClientConfiguration _clientConfiguration = new CoreHttpClientConfiguration.Builder()
            .HttpClientInstance(new HttpClient(handlerMock))
            .NumberOfRetries(numberOfRetries)
            .Build();

        private static BasicAuthManager _basicAuthManager = new BasicAuthManager(_basicAuthUserName, _basicAuthPassword);

        private static GlobalConfiguration globalConfiguration;

        protected static Lazy<GlobalConfiguration> LazyGlobalConfiguration => new Lazy<GlobalConfiguration>(() => globalConfiguration ??= new GlobalConfiguration.Builder()
            .ServerUrls(new Dictionary<Enum, string>
            {
                { MockServer.Server1, "http://my/path:3000/{one}"},
                { MockServer.Server2, "https://my/path/{two}"}
            }, MockServer.Server1)
            .AuthManagers(new Dictionary<string, AuthManager> {
                        {"global", _basicAuthManager}
            })
            .HttpConfiguration(_clientConfiguration)
            .Parameters(p => p
                .Header(h => h.Setup("additionalHead1", "headVal1"))
                .Header(h => h.Setup("additionalHead2", "headVal2"))
                .Template(t => t.Setup("one", "v1"))
                .Template(t => t.Setup("two", "v2")))
            .RuntimeParameters(p => p
                .AdditionalHeaders(ah => ah.Setup(new Dictionary<string, object>
                {
                    { "key5", 890.098 }
                })))
            .UserAgent("{language}|{version}|{engine}|{engine-version}|{os-info}", new List<(string placeHolder, string value)>
            {
                ("{language}", "my lang"),
                ("{version}", "1.*.*")
            })
            .ApiCallback(ApiCallBack)
            .Build()
        );

    }
}
