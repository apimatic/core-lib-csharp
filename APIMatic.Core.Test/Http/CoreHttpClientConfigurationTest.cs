using System;
using System.Collections.Generic;
using System.Net;
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
            _config = new CoreHttpClientConfiguration.Builder().Build();
        }

        [Test]
        public void Builder_BuildWithParameters_CoreHttpClientConfiguration()
        {
            // Arrange
            var timeout = 180;
            var skipSslCertVerification = true;
            var numberOfRetries = 3;
            var backoffFactor = 3;
            var retryInterval = 4.5;
            var maximumRetryWaitTime = 360;
            var statusCodesToRetry = new List<int> { 408, 409 };
            var requestMethodsToRetry = new List<HttpMethod> { HttpMethod.Post, HttpMethod.Get };

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
        public void Builder_BuildWithInvalidParameters_CoreHttpClientConfiguration()
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
            var expected = "HttpClientConfiguration: 00:01:40 , False , 0 , 2 , 1 , 00:02:00 , System.Collections.Immutable.ImmutableList`1[System.Int32] , System.Collections.Immutable.ImmutableList`1[System.Net.Http.HttpMethod] ,  , System.Net.Http.HttpClient , True ";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToString_WithProxy_CoreHttpClientConfiguration()
        {
            // Arrange
            var proxyConfig = new CoreProxyConfiguration(
                address: "proxy.example.com",
                port: 8080,
                user: "admin",
                pass: "secret",
                tunnel: true
            );

            var config = new CoreHttpClientConfiguration.Builder()
                .ProxyConfiguration(proxyConfig)
                .HttpClientInstance(new HttpClient())
                .Build();

            // Act
            var actual = config.ToString();

            // Assert
            var expected = "HttpClientConfiguration: 00:01:40 , False , 0 , 2 , 1 , 00:02:00 , " +
                           "System.Collections.Immutable.ImmutableList`1[System.Int32] , " +
                           "System.Collections.Immutable.ImmutableList`1[System.Net.Http.HttpMethod] , " +
                           "CoreProxyConfiguration: Address=proxy.example.com, Port=8080, User=admin, Pass=****, Tunnel=True , " +
                           "System.Net.Http.HttpClient , True ";

            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void GetInitializedHttpClientInstance_ShouldUseProxyWithoutSsl()
        {
            // Arrange
            var proxyConfig = new Proxy.CoreProxyConfiguration(
                address: "localhost",
                port: 8080,
                user: "user",
                pass: "pass",
                tunnel: true);

            // Act
            var handler = new CoreHttpClientConfiguration.Builder()
                .ProxyConfiguration(proxyConfig)
                .GetHandler();

            // Assert
            var webProxy = handler.Proxy as WebProxy;
            Assert.IsNotNull(webProxy);
            Assert.AreEqual("http://localhost:8080/", webProxy.Address.ToString());
            Assert.AreEqual("user", ((NetworkCredential)webProxy.Credentials).UserName);
            Assert.AreEqual("pass", ((NetworkCredential)webProxy.Credentials).Password);
            Assert.IsTrue(handler.UseProxy);
            Assert.IsFalse(handler.UseDefaultCredentials, "Tunnel mode should disable default credentials");
            Assert.IsTrue(handler.PreAuthenticate);

        }

        [Test]
        public void GetInitializedHttpClientInstance_ShouldUseProxyWithSkipSslVerification()
        {
            // Arrange
            var proxyConfig = new Proxy.CoreProxyConfiguration(
                address: "proxy.example.com",
                port: 8080,
                user: "user",
                pass: "pass",
                tunnel: false);

            // Act
            var handler = new CoreHttpClientConfiguration.Builder()
                .SkipSslCertVerification(true)   // Enable skipping SSL cert validation
                .ProxyConfiguration(proxyConfig) // Set proxy config
                .GetHandler();

            // Assert Proxy
            var webProxy = handler.Proxy as WebProxy;
            Assert.IsNotNull(webProxy);
            Assert.AreEqual("http://proxy.example.com:8080/", webProxy.Address.ToString());

            var credentials = webProxy.Credentials as NetworkCredential;
            Assert.IsNotNull(credentials);
            Assert.AreEqual("user", credentials.UserName);
            Assert.AreEqual("pass", credentials.Password);
            Assert.IsFalse(handler.PreAuthenticate);
            Assert.IsTrue(handler.UseProxy);
            Assert.IsNotNull(handler.ServerCertificateCustomValidationCallback);

            bool validationResult = handler.ServerCertificateCustomValidationCallback(
                null, null, null, System.Net.Security.SslPolicyErrors.None);

            Assert.IsTrue(validationResult, "SSL certificate validation callback should return true to skip validation");
        }


        [Test]
        public void GetInitializedHttpClientInstance_ShouldNotUseProxyOrSkipSsl()
        {
            // Act
            var handler = new CoreHttpClientConfiguration.Builder()
                .SkipSslCertVerification(false)
                .GetHandler();

            Assert.IsNull(handler.Proxy, "Proxy should be null when not configured");

            Assert.IsNull(handler.ServerCertificateCustomValidationCallback, "No custom SSL callback should be set");
        }




    }
}
