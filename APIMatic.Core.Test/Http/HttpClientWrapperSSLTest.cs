﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using APIMatic.Core.Http.Configuration;
using NUnit.Framework;

namespace APIMatic.Core.Test.Http
{
    [TestFixture]
    public class HttpClientWrapperSSLTest : TestBase
    {
        private readonly string expiredSSLCertUrl = "https://expired.badssl.com/";

        /// <summary>
        /// Verifies that an exception is thrown when the HTTP client attempts to connect
        /// to a server with an expired SSL certificate without skipping SSL verification.
        /// </summary>
        [Test]
        public async Task TestHttpClientSSLCertificateVerification_ExceptionResponse()
        {
            var expectedValue = "The SSL connection could not be established, see inner exception.";
            var clientConfiguration = new CoreHttpClientConfiguration.Builder()
                .Build();

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

            // Assert
            Assert.AreEqual(expectedValue, ex.Message);
        }

        /// <summary>
        /// Verifies that the HTTP client can successfully connect to a server with an expired
        /// SSL certificate when SSL certificate verification is skipped.
        /// </summary>
        [Test]
        public async Task TestHttpClientSkipSSLCertificateVerification_OKResponse()
        {
            var clientConfiguration = new CoreHttpClientConfiguration.Builder()
                .SkipSslCertVerification(true)
                .Build();

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
            var actual = await client.ExecuteAsync(request);

            // Assert
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
        }
    }
}
