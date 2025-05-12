using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Security;
using System.Threading.Tasks;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Proxy;
using NUnit.Framework;

namespace APIMatic.Core.Test.Http
{
    [TestFixture]
    public class HttpClientWrapperSSLTest : TestBase
    {
        private readonly string expiredSSLCertUrl = "https://expired.badssl.com/";

        // Helper method to create client configuration with SSL handling
        private CoreHttpClientConfiguration CreateClientConfiguration(bool skipSslVerification)
        {
            var handler = new HttpClientHandler
            {
                // If skipSslVerification is true, we bypass the SSL check
                ServerCertificateCustomValidationCallback = skipSslVerification
                    ? (sender, cert, chain, sslPolicyErrors) => true
                    : (sender, cert, chain, sslPolicyErrors) =>
                    {
                        // This will force SSL validation failure to simulate the expired certificate error
                        return sslPolicyErrors == SslPolicyErrors.None;
                    }
            };

            var proxyConfig = new CoreProxyConfiguration(
                address: "http://localhost",
                port: 8080,
                user: "user",
                pass: "pass",
                tunnel: true
            );

            return new CoreHttpClientConfiguration.Builder(proxyConfig)
                .HttpClientInstance(new HttpClient(handler)) // Set custom handler with SSL validation behavior
                .Build();
        }

        [Test]
        public async Task TestHttpClientSSLCertificateVerification_ExceptionResponse()
        {
            var clientConfiguration = CreateClientConfiguration(skipSslVerification: false); // Disable SSL verification

            var config = new GlobalConfiguration.Builder()
                .ServerUrls(new Dictionary<Enum, string>
                {
                    { MockServer.Server1, expiredSSLCertUrl },
                }, MockServer.Server1)
                .HttpConfiguration(clientConfiguration)
                .ApiCallback(ApiCallBack)
                .Build();

            var client = config.HttpClient;

            var request = await config.GlobalRequestBuilder()
                .Setup(HttpMethod.Get, string.Empty)
                .Build();

            // Act
            var ex = Assert.ThrowsAsync<HttpRequestException>(() => client.ExecuteAsync(request));
            Assert.IsTrue(ex.Message.Contains("The SSL connection could not be established"));
        }
    }
}
