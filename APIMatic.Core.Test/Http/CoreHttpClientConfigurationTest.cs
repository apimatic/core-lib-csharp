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

        private class MockProxyConfiguration : ICoreProxyConfiguration
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
            public bool Tunnel { get; set; }
        }

        [SetUp]
        public void SetupCoreHttpClient()
        {
            _config = new CoreHttpClientConfiguration.Builder().Build();
        }

        [Test]
        public void Builder_BuildWithPrameters_CoreHttpClientConfiguration()
        {
            // Arrange
            var timeout = 180;
            var skipSslCertVerification = true;
            var numberOfRetries = 3;
            var backoffFactor = 3;
            var retryInterval = 4.5;
            var maximumRetryWaitTime = 360;
            var statusCodesToRetry = new List<int>() { 408, 409 };
            var requestMethodsToRetry = new List<HttpMethod>() { HttpMethod.Post, HttpMethod.Get };

            // Act
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

            // Assert
            Assert.NotNull(config);
            Assert.AreEqual(TimeSpan.FromSeconds(timeout), config.Timeout);
            Assert.AreEqual(skipSslCertVerification, config.SkipSslCertVerification);
            Assert.AreEqual(numberOfRetries, config.NumberOfRetries);
            Assert.AreEqual(backoffFactor, config.BackoffFactor);
            Assert.AreEqual(retryInterval, config.RetryInterval);
            Assert.AreEqual(TimeSpan.FromSeconds(maximumRetryWaitTime), config.MaximumRetryWaitTime);
            CollectionAssert.AreEqual(statusCodesToRetry, config.StatusCodesToRetry);
            CollectionAssert.AreEqual(requestMethodsToRetry, config.RequestMethodsToRetry);
        }

        [Test]
        public void Builder_BuildWithInvalidPrameters_CoreHttpClientConfiguration()
        {
            // Arrange
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

            // Expected default values
            var defaultTimeout = 100;
            var defaultNumberOfRetries = 0;
            var defaultBackoffFactor = 2;
            var defaultRetryInterval = 1;
            var defaultMaximumRetryWaitTime = 120;

            // Assert
            Assert.NotNull(config);
            Assert.NotNull(config.HttpClientInstance);
            Assert.AreEqual(TimeSpan.FromSeconds(defaultTimeout), config.Timeout);
            Assert.AreEqual(skipSslCertVerification, config.SkipSslCertVerification);
            Assert.AreEqual(defaultNumberOfRetries, config.NumberOfRetries);
            Assert.AreEqual(defaultBackoffFactor, config.BackoffFactor);
            Assert.AreEqual(defaultRetryInterval, config.RetryInterval);
            Assert.AreEqual(TimeSpan.FromSeconds(defaultMaximumRetryWaitTime), config.MaximumRetryWaitTime);
            CollectionAssert.IsEmpty(config.StatusCodesToRetry);
            CollectionAssert.IsEmpty(config.RequestMethodsToRetry);
        }

        [Test]
        public void ToString_Default_CoreHttpClientConfiguration()
        {
            // Act
            var actual = _config.ToString();

            // Assert
            var expected = "HttpClientConfiguration: 00:01:40 , False , 0 , 2 , 1 , 00:02:00 , System.Collections.Immutable.ImmutableList`1[System.Int32] , System.Collections.Immutable.ImmutableList`1[System.Net.Http.HttpMethod] , System.Net.Http.HttpClient , True ";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Builder_WithProxyConfiguration_ShouldIncludeProxySettings()
        {
            // Arrange
            var proxy = new MockProxyConfiguration
            {
                Address = "http://proxy.example.com",
                Port = 3128,
                User = "user",
                Pass = "pass",
                Tunnel = true
            };

            // Act
            var config = _config.ToBuilder()
                .ProxyConfiguration(new CoreProxyConfiguration(proxy))
                .Build();

            // Assert
            Assert.NotNull(config.ProxyConfiguration);
            Assert.AreEqual("http://proxy.example.com", config.ProxyConfiguration.Address);
            Assert.AreEqual(3128, config.ProxyConfiguration.Port);
            Assert.AreEqual("user", config.ProxyConfiguration.User);
            Assert.AreEqual("pass", config.ProxyConfiguration.Pass);
            Assert.IsTrue(config.ProxyConfiguration.Tunnel);
        }

    }
}
