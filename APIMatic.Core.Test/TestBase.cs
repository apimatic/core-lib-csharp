using System;
using System.Collections.Generic;
using System.Net.Http;
using APIMatic.Core.Authentication;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Test.MockTypes.Authentication;
using APIMatic.Core.Types;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test
{
    public class TestBase
    {
        protected static readonly string _basicAuthUserName = "ApimaticUserName";
        protected static readonly string _basicAuthPassword = "ApimaticPassword";
        protected static readonly HttpCallBack ApiCallBack = new();
        protected enum MockServer { Server1, Server2 }
        protected static readonly int numberOfRetries = 1;

        protected static readonly MockHttpMessageHandler handlerMock = new()
        {
            AutoFlush = true
        };

        protected static readonly ICoreHttpClientConfiguration _clientConfiguration = new CoreHttpClientConfiguration.Builder()
            .HttpClientInstance(new HttpClient(handlerMock))
            .NumberOfRetries(numberOfRetries)
            .Build();

        private static GlobalConfiguration globalConfiguration;

        protected static Lazy<GlobalConfiguration> LazyGlobalConfiguration => new(() => globalConfiguration ??= new GlobalConfiguration.Builder()
            .ServerUrls(new Dictionary<Enum, string>
            {
                {MockServer.Server1, "http://my/path:3000/{one}"},
                {MockServer.Server2, "https://my/path/{two}"}
            }, MockServer.Server1)
            .AuthManagers(new Dictionary<string, AuthManager> {
                {"basic", new BasicAuthManager(_basicAuthUserName, _basicAuthPassword)}
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
            .loggingConfig(new SdkLoggingConfiguration.Builder().Logger(NullLogger.Instance).Build())
            .ApiCallback(ApiCallBack)
            .Build()
        );

    }
}
