using System;
using System.Collections.Generic;
using System.Net.Http;
using APIMatic.Core.Authentication;
using APIMatic.Core.Test.MockTypes.Authentication;
using NUnit.Framework;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class AuthenticationTest : TestBase
    {
        [Test]
        public void Multiple_Authentication_Success_WithFirstAuth()
        {
            var globalConfiguration = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    {MockServer.Server1, "http://my/path:3000/{one}"},
                }, MockServer.Server1)
                .AuthManagers(new Dictionary<string, AuthManager>()
                {
                    {"basic", new BasicAuthManager("username", "password")},
                    {"header", new HeaderAuthManager("my api key", "test token")},
                    {"query", new QueryAuthManager("my api key", "test token")}
                })
                .HttpConfiguration(_clientConfiguration)
                .Build();

            var request = globalConfiguration.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/auth")
                .WithOrAuth(auth => auth
                    .AddAndGroup(innerGroup => innerGroup
                        .Add("header")
                        .Add("query"))
                    .Add("basic"))
                .Build();

            Assert.False(request.Headers.ContainsKey("Authorization"));
            Assert.AreEqual("my api key", request.Headers["API-KEY"]);
            Assert.AreEqual("test token", request.Headers["TOKEN"]);
            Assert.AreEqual("my api key", request.QueryParameters["API-KEY"]);
            Assert.AreEqual("test token", request.QueryParameters["TOKEN"]);
        }

        [Test]
        public void Multiple_Authentication_Success_WithSecondAuth()
        {
            var basicAuthManager = new BasicAuthManager("username", "password");
            var globalConfiguration = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    {MockServer.Server1, "http://my/path:3000/{one}"},
                }, MockServer.Server1)
                .AuthManagers(new Dictionary<string, AuthManager>()
                {
                    {"basic", basicAuthManager},
                    {"header", new HeaderAuthManager(null, null)},
                    {"query", new QueryAuthManager("my api key", "test token")}
                })
                .HttpConfiguration(_clientConfiguration)
                .Build();

            var request = globalConfiguration.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/auth")
                .WithOrAuth(auth => auth
                    .AddAndGroup(innerGroup => innerGroup
                        .Add("header")
                        .Add("query"))
                    .Add("basic"))
                .Build();

            Assert.AreEqual(basicAuthManager.GetBasicAuthHeader(), request.Headers["Authorization"]);
            Assert.False(request.Headers.ContainsKey("API-KEY"));
            Assert.False(request.Headers.ContainsKey("TOKEN"));
            Assert.False(request.Headers.ContainsKey("API-KEY"));
            Assert.False(request.Headers.ContainsKey("TOKEN"));
        }

        [Test]
        public void Multiple_Authentication_OR_Validation_Failure()
        {
            var globalConfiguration = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    {MockServer.Server1, "http://my/path:3000/{one}"},
                }, MockServer.Server1)
                .AuthManagers(new Dictionary<string, AuthManager>()
                {
                    {"basic", new BasicAuthManager("username", null)},
                    {"header", new HeaderAuthManager("my api key", null)},
                    {"query", new QueryAuthManager("my api key", "test token")}
                })
                .HttpConfiguration(_clientConfiguration)
                .Build();

            var exp = Assert.Throws<ArgumentNullException>(() => globalConfiguration.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/auth")
                .WithOrAuth(auth => auth
                    .Add("basic")
                    .AddAndGroup(innerGroup => innerGroup
                        .Add("header")
                        .Add("query")))
                .Build());

            Assert.AreEqual("Following authentication credentials are required:\n" +
                "-> Missing required header field: Authorization\n" +
                "-> Missing required header field: TOKEN", exp.Message);
        }

        [Test]
        public void Multiple_Authentication_AND_Validation_Failure()
        {
            var globalConfiguration = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    {MockServer.Server1, "http://my/path:3000/{one}"},
                }, MockServer.Server1)
                .AuthManagers(new Dictionary<string, AuthManager>()
                {
                    {"basic", new BasicAuthManager("username", null)},
                    {"header", new HeaderAuthManager("my api key", null)},
                    {"query", new QueryAuthManager("my api key", "test token")}
                })
                .HttpConfiguration(_clientConfiguration)
                .Build();

            var exp = Assert.Throws<ArgumentNullException>(() => globalConfiguration.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/auth")
                .WithAndAuth(auth => auth
                    .Add("query")
                    .Add("header"))
                .Build());

            Assert.AreEqual("Following authentication credentials are required:\n" +
                "-> Missing required header field: TOKEN", exp.Message);
        }

        [Test]
        public void Multiple_Authentication_AND_All_Missing_Validation_Failure()
        {
            var globalConfiguration = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    {MockServer.Server1, "http://my/path:3000/{one}"},
                }, MockServer.Server1)
                .AuthManagers(new Dictionary<string, AuthManager>()
                {
                    {"header", new HeaderAuthManager(null, null)},
                    {"query", new QueryAuthManager(null, null)}
                })
                .HttpConfiguration(_clientConfiguration)
                .Build();

            var exp = Assert.Throws<ArgumentNullException>(() => globalConfiguration.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/auth")
                .WithAndAuth(auth => auth
                    .Add("query")
                    .Add("header"))
                .Build());

            Assert.AreEqual("Following authentication credentials are required:\n" +
                "-> Missing required query field: API-KEY\n" +
                "-> Missing required query field: TOKEN\n" +
                "-> Missing required header field: API-KEY\n" +
                "-> Missing required header field: TOKEN", exp.Message);
        }

        [Test]
        public void Multiple_Authentication_AND_with_nested_OR_Validation_Failure()
        {
            var globalConfiguration = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    {MockServer.Server1, "http://my/path:3000/{one}"},
                }, MockServer.Server1)
                .AuthManagers(new Dictionary<string, AuthManager>()
                {
                    {"basic", new BasicAuthManager("username", null)},
                    {"header", new HeaderAuthManager("my api key", null)},
                    {"query", new QueryAuthManager("my api key", "test token")}
                })
                .HttpConfiguration(_clientConfiguration)
                .Build();

            var exp = Assert.Throws<ArgumentNullException>(() => globalConfiguration.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, "/auth")
                .WithAndAuth(auth => auth
                    .Add("query")
                    .AddOrGroup(innerGroup => innerGroup
                        .Add("basic")
                        .Add("header")))
                .Build());

            Assert.AreEqual("Following authentication credentials are required:\n" +
                "-> Missing required header field: Authorization\n" +
                "-> Missing required header field: TOKEN", exp.Message);
        }
    }
}
