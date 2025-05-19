using System;
using System.Collections.Generic;
using System.Net.Http;
using APIMatic.Core.Authentication;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Test.MockTypes.Authentication;
using APIMatic.Core.Types;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test
{
    /// <summary>
    /// Base class for all HTTP-related unit tests.
    /// Sets up shared test data, mock configurations, and reusable client settings.
    /// </summary>
    public class TestBase
    {
        protected static readonly string _basicAuthUserName = "ApimaticUserName";
        protected static readonly string _basicAuthPassword = "ApimaticPassword";

        protected static readonly HttpCallBack ApiCallBack = new();

        protected enum MockServer { Server1, Server2 }

        protected static readonly int numberOfRetries = 1;

        /// <summary>
        /// Shared mocked HTTP message handler used to simulate responses.
        /// </summary>
        protected static readonly MockHttpMessageHandler handlerMock = new()
        {
            AutoFlush = true
        };

        /// <summary>
        /// Shared client configuration for tests, with a mocked HttpClient and retry settings.
        /// </summary>
        protected static readonly ICoreHttpClientConfiguration _clientConfiguration = new CoreHttpClientConfiguration.Builder()
            .HttpClientInstance(new HttpClient(handlerMock))
            .NumberOfRetries(numberOfRetries)
            .Build();

        private static GlobalConfiguration globalConfiguration;

        /// <summary>
        /// Lazily initialized global configuration with server URLs, authentication,
        /// headers, parameters, runtime values, and user agent customization.
        /// </summary>
        protected static Lazy<GlobalConfiguration> LazyGlobalConfiguration => new(() => globalConfiguration ??= new GlobalConfiguration.Builder()
            .ServerUrls(new Dictionary<Enum, string>
            {
            {MockServer.Server1, "http://my/path:3000/{one}" },
            {MockServer.Server2, "https://my/path/{two}"}
            }, MockServer.Server1)
            .AuthManagers(new Dictionary<string, AuthManager>
            {
                { "basic", new BasicAuthManager(_basicAuthUserName, _basicAuthPassword) }
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
            .LoggingConfig(null)
            .ApiCallback(ApiCallBack)
            .Build());
    }
}
