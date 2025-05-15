using System;
using System.Collections.Generic;
using System.Net.Http;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Proxy;
using NUnit.Framework;

namespace APIMatic.Core.Test.Http
{
    [TestFixture]
    public class CoreHttpClientConfigurationTest
    {
        private CoreHttpClientConfiguration _config;

        [SetUp]
        public void SetupCoreHttpClient()
        {
            var proxyConfig = new CoreProxyConfiguration(
                address: "http://localhost",
                port: 8080,
                user: "user",
                pass: "pass",
                tunnel: true
            );
            _config = new CoreHttpClientConfiguration.Builder(proxyConfig).Build();
        }

        [Test]
        public void Builder_BuildWithParameters_CoreHttpClientConfiguration()
        {
            var timeout = 180;
            var skipSslCertVerification = true;
            var numberOfRetries = 3;
            var backoffFactor = 3;
            var retryInterval = 4.5;
            var maximumRetryWaitTime = 360;
            var statusCodesToRetry = new List<int>() { 408, 409 };
            var requestMethodsToRetry = new List<HttpMethod>() { HttpMethod.Post, HttpMethod.Get };

            var config = _config.ToBuilder()
                .Timeout(TimeSpan.FromSeconds(timeout))
                .SkipSslCertVerification(skipSslCertVerification)
                .NumberOfRetries(numberOfRetries)
                .BackoffFactor(backoffFactor)
                .RetryInterval(retryInterval)
                .MaximumRetryWaitTime(TimeSpan.FromSeconds(maximumRetryWaitTime))
                .StatusCodesToRetry(statusCodesToRetry)
                .RequestMethodsToRetry(requestMethodsToRetry)
            .Build();

            Assert.NotNull(config);
            Assert.AreEqual(config.Timeout, TimeSpan.FromSeconds(timeout));
            Assert.AreEqual(config.SkipSslCertVerification, skipSslCertVerification);
            Assert.AreEqual(config.NumberOfRetries, numberOfRetries);
            Assert.AreEqual(config.BackoffFactor, backoffFactor);
            Assert.AreEqual(config.RetryInterval, retryInterval);
            Assert.AreEqual(config.MaximumRetryWaitTime, TimeSpan.FromSeconds(maximumRetryWaitTime));
            CollectionAssert.AreEqual(config.StatusCodesToRetry, statusCodesToRetry);
            CollectionAssert.AreEqual(config.RequestMethodsToRetry, requestMethodsToRetry);
        }

        [Test]
        public void Builder_BuildWithInvalidParameters_CoreHttpClientConfiguration()
        {
            var timeout = 0;
            var numberOfRetries = -1;
            var backoffFactor = 0;
            var retryInterval = -1;
            var maximumRetryWaitTime = -1;
            var skipSslCertVerification = false;

            var config = _config.ToBuilder()
                .HttpClientInstance(null)
                .Timeout(TimeSpan.FromSeconds(timeout))
                .SkipSslCertVerification(skipSslCertVerification)
                .NumberOfRetries(numberOfRetries)
                .BackoffFactor(backoffFactor)
                .RetryInterval(retryInterval)
                .MaximumRetryWaitTime(TimeSpan.FromSeconds(maximumRetryWaitTime))
                .StatusCodesToRetry(null)
                .RequestMethodsToRetry(null)
            .Build();

            var defaultTimeout = 100;
            var defaultNumberOfRetries = 0;
            var defaultBackoffFactor = 2;
            var defaultRetryInterval = 1;
            var defaultMaximumRetryWaitTime = 120;

            Assert.NotNull(config);
            Assert.NotNull(config.HttpClientInstance);
            Assert.AreEqual(config.Timeout, TimeSpan.FromSeconds(defaultTimeout));
            Assert.AreEqual(config.SkipSslCertVerification, skipSslCertVerification);
            Assert.AreEqual(config.NumberOfRetries, defaultNumberOfRetries);
            Assert.AreEqual(config.BackoffFactor, defaultBackoffFactor);
            Assert.AreEqual(config.RetryInterval, defaultRetryInterval);
            Assert.AreEqual(config.MaximumRetryWaitTime, TimeSpan.FromSeconds(defaultMaximumRetryWaitTime));
            CollectionAssert.IsEmpty(config.StatusCodesToRetry);
            CollectionAssert.IsEmpty(config.RequestMethodsToRetry);
        }

        [Test]
        public void ToString_Default_CoreHttpClientConfiguration()
        {
            var actual = _config.ToString();
            var expected = "HttpClientConfiguration: 00:01:40 , False , 0 , 2 , 1 , 00:02:00 , System.Collections.Immutable.ImmutableList`1[System.Int32] , System.Collections.Immutable.ImmutableList`1[System.Net.Http.HttpMethod] , System.Net.Http.HttpClient , True ";
            Assert.AreEqual(expected, actual);
        }
    }
}
