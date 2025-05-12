// <copyright file="CoreHttpClientConfiguration.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using APIMatic.Core.Proxy;

namespace APIMatic.Core.Http.Configuration
{
    /// <summary>
    /// CoreHttpClientConfiguration represents the current state of the Http Client.
    /// </summary>
    public class CoreHttpClientConfiguration : ICoreHttpClientConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreHttpClientConfiguration"/>
        /// class.
        /// </summary>
        public CoreProxyConfiguration ProxyConfiguration { get; }

        private CoreHttpClientConfiguration(
            TimeSpan timeout,
            bool skipSslCertVerification,
            int numberOfRetries,
            int backoffFactor,
            double retryInterval,
            TimeSpan maximumRetryWaitTime,
            IList<int> statusCodesToRetry,
            IList<HttpMethod> requestMethodsToRetry,
            HttpClient httpClientInstance,
            bool overrideHttpClientConfiguration,
            CoreProxyConfiguration proxyConfiguration)
        {
            Timeout = timeout;
            SkipSslCertVerification = skipSslCertVerification;
            NumberOfRetries = numberOfRetries;
            BackoffFactor = backoffFactor;
            RetryInterval = retryInterval;
            MaximumRetryWaitTime = maximumRetryWaitTime;
            StatusCodesToRetry = statusCodesToRetry;
            RequestMethodsToRetry = requestMethodsToRetry;
            HttpClientInstance = httpClientInstance;
            OverrideHttpClientConfiguration = overrideHttpClientConfiguration;
            ProxyConfiguration = proxyConfiguration;
        }

        public TimeSpan Timeout { get; }
        public bool SkipSslCertVerification { get; }
        public int NumberOfRetries { get; }
        public int BackoffFactor { get; }
        public double RetryInterval { get; }
        public TimeSpan MaximumRetryWaitTime { get; }
        public IList<int> StatusCodesToRetry { get; }
        public IList<HttpMethod> RequestMethodsToRetry { get; }
        public HttpClient HttpClientInstance { get; }
        public bool OverrideHttpClientConfiguration { get; }

        public override string ToString()
        {
            return "HttpClientConfiguration: " +
                $"{Timeout} , " +
                $"{SkipSslCertVerification} , " +
                $"{NumberOfRetries} , " +
                $"{BackoffFactor} , " +
                $"{RetryInterval} , " +
                $"{MaximumRetryWaitTime} , " +
                $"{StatusCodesToRetry} , " +
                $"{RequestMethodsToRetry} , " +
                $"{HttpClientInstance} , " +
                $"{OverrideHttpClientConfiguration} ";
        }

        public Builder ToBuilder()
        {
            var builder = new Builder(ProxyConfiguration)
                .Timeout(Timeout)
                .SkipSslCertVerification(SkipSslCertVerification)
                .NumberOfRetries(NumberOfRetries)
                .BackoffFactor(BackoffFactor)
                .RetryInterval(RetryInterval)
                .MaximumRetryWaitTime(MaximumRetryWaitTime)
                .StatusCodesToRetry(StatusCodesToRetry)
                .RequestMethodsToRetry(RequestMethodsToRetry)
                .HttpClientInstance(HttpClientInstance, OverrideHttpClientConfiguration);

            return builder;
        }

        /// <summary>
        /// Builder class.
        /// </summary>
        public class Builder
        {
            private readonly CoreProxyConfiguration proxyConfiguration;
            private TimeSpan timeout = TimeSpan.FromSeconds(100);
            private bool skipSslCertVerification = false;
            private int numberOfRetries = 0;
            private int backoffFactor = 2;
            private double retryInterval = 1;
            private TimeSpan maximumRetryWaitTime = TimeSpan.FromSeconds(120);
            private IList<int> statusCodesToRetry = new List<int>
            {
                408, 413, 429, 500, 502, 503, 504, 521, 522, 524
            }.ToImmutableList();
            private IList<HttpMethod> requestMethodsToRetry = new List<string>
            {
                "GET", "PUT"
            }.Select(val => new HttpMethod(val)).ToImmutableList();
            private HttpClient httpClientInstance = null;
            private bool overrideHttpClientConfiguration = true;
            public Builder(CoreProxyConfiguration proxyConfig)
            {
                proxyConfiguration = proxyConfig;
            }

            public Builder Timeout(TimeSpan timeout)
            {
                this.timeout = timeout.TotalSeconds <= 0 ? TimeSpan.FromSeconds(100) : timeout;
                return this;
            }

            public Builder SkipSslCertVerification(bool skipSslCertVerification)
            {
                this.skipSslCertVerification = skipSslCertVerification;
                return this;
            }

            public Builder NumberOfRetries(int numberOfRetries)
            {
                this.numberOfRetries = numberOfRetries < 0 ? 0 : numberOfRetries;
                return this;
            }

            public Builder BackoffFactor(int backoffFactor)
            {
                this.backoffFactor = backoffFactor < 1 ? 2 : backoffFactor;
                return this;
            }

            public Builder RetryInterval(double retryInterval)
            {
                this.retryInterval = retryInterval < 0 ? 1 : retryInterval;
                return this;
            }

            public Builder MaximumRetryWaitTime(TimeSpan maximumRetryWaitTime)
            {
                this.maximumRetryWaitTime = maximumRetryWaitTime.TotalSeconds < 0 ? TimeSpan.FromSeconds(120) : maximumRetryWaitTime;
                return this;
            }

            public Builder StatusCodesToRetry(IList<int> statusCodesToRetry)
            {
                this.statusCodesToRetry = statusCodesToRetry ?? new List<int>().ToImmutableList();
                return this;
            }

            public Builder RequestMethodsToRetry(IList<HttpMethod> requestMethodsToRetry)
            {
                this.requestMethodsToRetry = requestMethodsToRetry ?? new List<HttpMethod>().ToImmutableList();
                return this;
            }
            public Builder HttpClientInstance(HttpClient httpClientInstance, bool overrideHttpClientConfiguration = true)
            {
                this.httpClientInstance = httpClientInstance;
                this.overrideHttpClientConfiguration = overrideHttpClientConfiguration;
                return this;
            }

            public CoreHttpClientConfiguration Build()
            {
                return new CoreHttpClientConfiguration(
                        timeout,
                        skipSslCertVerification,
                        numberOfRetries,
                        backoffFactor,
                        retryInterval,
                        maximumRetryWaitTime,
                        statusCodesToRetry,
                        requestMethodsToRetry,
                        httpClientInstance ?? GetInitializedHttpClientInstance(),
                        overrideHttpClientConfiguration,
                        proxyConfiguration);
            }

            private HttpClient GetInitializedHttpClientInstance()
            {
                var handler = new HttpClientHandler();

                if (proxyConfiguration?.Address != null)
                {
                    Console.WriteLine($"Applying Core Proxy: {proxyConfiguration.Address}:{proxyConfiguration.Port}");

                    var proxy = new WebProxy(proxyConfiguration.Address, proxyConfiguration.Port)
                    {
                        BypassProxyOnLocal = false,
                        Credentials = !string.IsNullOrEmpty(proxyConfiguration.User)
                            ? new NetworkCredential(proxyConfiguration.User, proxyConfiguration.Pass)
                            : null
                    };

                    handler.Proxy = proxy;
                    handler.UseProxy = true;
                }

                return new HttpClient(handler);
            }
        }
    }
}
