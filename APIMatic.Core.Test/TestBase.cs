using APIMatic.Core.Types;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class TestBase
    {
        protected enum MockServer { Server1, Server2 }
        protected GlobalConfiguration GlobalConfiguration { get; private set; }

        protected Mock<CoreRequest> MockRequest(HttpMethod method = null, string queryUrl = null,
            Dictionary<string, string> headers = null, object body = null,
            List<KeyValuePair<string, object>> formParameters = null,
            Dictionary<string, object> queryParameters = null) =>
            new Mock<CoreRequest>(method, queryUrl, headers, body, formParameters, null, null, queryParameters);


        /// <summary>
        /// Set up the TestCase.
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            GlobalConfiguration = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    { MockServer.Server1, "http://my/path:3000/{one}"},
                    { MockServer.Server2, "https://my/path/{two}"}
                }, MockServer.Server1)
                .AuthManagers(new Dictionary<string, object>())
                .HeaderParam(p => p.Init("additionalHead1", "headVal1"))
                .HeaderParam(p => p.Init("additionalHead2", "headVal2"))
                .TemplateParam(p => p.Init("one", "v1"))
                .TemplateParam(p => p.Init("two", "v2"))
                .RuntimeHeaderParam(p => p.Init("key5", 890.098))
                .UserAgent("{language}|{version}|{engine}|{engine-version}|{os-info}", new List<(string placeHolder, string value)>
                {
                    ("{language}", "my lang"),
                    ("{version}", "1.*.*")
                })
                .Build();
        }
    }
}
