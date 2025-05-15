﻿// <copyright file="CoreHttpClientConfiguration.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;

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
            bool overrideHttpClientConfiguration)
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
        }

        /// <summary>
        /// Gets Http client timeout.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Gets Whether to skip verification of SSL certificates.
        /// </summary>
        public bool SkipSslCertVerification { get; }

        /// <summary>
        /// Gets Number of times the request is retried.
        /// </summary>
        public int NumberOfRetries { get; }

        /// <summary>
        /// Gets Exponential backoff factor for duration between retry calls.
        /// </summary>
        public int BackoffFactor { get; }

        /// <summary>
        /// Gets The time interval between the endpoint calls.
        /// </summary>
        public double RetryInterval { get; }

        /// <summary>
        /// Gets The maximum retry wait time.
        /// </summary>
        public TimeSpan MaximumRetryWaitTime { get; }

        /// <summary>
        /// Gets List of Http status codes to invoke retry.
        /// </summary>
        public IList<int> StatusCodesToRetry { get; }

        /// <summary>
        /// Gets List of Http request methods to invoke retry.
        /// </summary>
        public IList<HttpMethod> RequestMethodsToRetry { get; }

        /// <summary>
        /// Gets HttpClient instance used to make the HTTP calls
        /// </summary>
        public HttpClient HttpClientInstance { get; }

        /// <summary>
        /// Gets Boolean which allows the SDK to override http client instance's settings used for features like retries, timeouts etc.
        /// </summary>
        public bool OverrideHttpClientConfiguration { get; }

        /// <inheritdoc/>
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

        /// <summary>
        /// Creates an object of the HttpClientConfiguration using the values provided for the builder.
        /// </summary>
        /// <returns>Builder.</returns>
        public Builder ToBuilder()
        {
            var builder = new Builder()
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

            /// <summary>
            /// Sets the Timeout.
            /// </summary>
            /// <param name="timeout"> Timeout. </param>
            /// <returns>Builder.</returns>
            public Builder Timeout(TimeSpan timeout)
            {
                this.timeout = timeout.TotalSeconds <= 0 ? TimeSpan.FromSeconds(100) : timeout;
                return this;
            }

            /// <summary>
            /// Sets the SkipSslCertVerification.
            /// </summary>
            /// <param name="skipSslCertVerification">Bool for skipping (or not) SSL certificate verification</param>
            /// <returns>Builder.</returns>
            public Builder SkipSslCertVerification(bool skipSslCertVerification)
            {
                this.skipSslCertVerification = skipSslCertVerification;
                return this;
            }

            /// <summary>
            /// Sets the NumberOfRetries.
            /// </summary>
            /// <param name="numberOfRetries"> NumberOfRetries. </param>
            /// <returns>Builder.</returns>
            public Builder NumberOfRetries(int numberOfRetries)
            {
                this.numberOfRetries = numberOfRetries < 0 ? 0 : numberOfRetries;
                return this;
            }

            /// <summary>
            /// Sets the BackoffFactor.
            /// </summary>
            /// <param name="backoffFactor"> BackoffFactor. </param>
            /// <returns>Builder.</returns>
            public Builder BackoffFactor(int backoffFactor)
            {
                this.backoffFactor = backoffFactor < 1 ? 2 : backoffFactor;
                return this;
            }

            /// <summary>
            /// Sets the RetryInterval.
            /// </summary>
            /// <param name="retryInterval"> RetryInterval. </param>
            /// <returns>Builder.</returns>
            public Builder RetryInterval(double retryInterval)
            {
                this.retryInterval = retryInterval < 0 ? 1 : retryInterval;
                return this;
            }

            /// <summary>
            /// Sets the MaximumRetryWaitTime.
            /// </summary>
            /// <param name="maximumRetryWaitTime"> MaximumRetryWaitTime. </param>
            /// <returns>Builder.</returns>
            public Builder MaximumRetryWaitTime(TimeSpan maximumRetryWaitTime)
            {
                this.maximumRetryWaitTime = maximumRetryWaitTime.TotalSeconds < 0 ? TimeSpan.FromSeconds(120) : maximumRetryWaitTime;
                return this;
            }

            /// <summary>
            /// Sets the StatusCodesToRetry.
            /// </summary>
            /// <param name="statusCodesToRetry"> StatusCodesToRetry. </param>
            /// <returns>Builder.</returns>
            public Builder StatusCodesToRetry(IList<int> statusCodesToRetry)
            {
                this.statusCodesToRetry = statusCodesToRetry ?? new List<int>().ToImmutableList();
                return this;
            }

            /// <summary>
            /// Sets the RequestMethodsToRetry.
            /// </summary>
            /// <param name="requestMethodsToRetry"> RequestMethodsToRetry. </param>
            /// <returns>Builder.</returns>
            public Builder RequestMethodsToRetry(IList<HttpMethod> requestMethodsToRetry)
            {
                this.requestMethodsToRetry = requestMethodsToRetry ?? new List<HttpMethod>().ToImmutableList();
                return this;
            }

            /// <summary>
            /// Sets the HttpClientInstance.
            /// </summary>
            /// <param name="httpClientInstance"> HttpClientInstance. </param>
            /// <param name="overrideHttpClientConfiguration"> OverrideHttpClientConfiguration. </param>
            /// <returns>Builder.</returns>
            public Builder HttpClientInstance(HttpClient httpClientInstance, bool overrideHttpClientConfiguration = true)
            {
                this.httpClientInstance = httpClientInstance;
                this.overrideHttpClientConfiguration = overrideHttpClientConfiguration;
                return this;
            }

            /// <summary>
            /// Creates an object of the HttpClientConfiguration using the values provided for the builder.
            /// </summary>
            /// <returns>HttpClientConfiguration.</returns>
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
                        GetInitializedHttpClientInstance(),
                        overrideHttpClientConfiguration);
            }

            private HttpClient GetInitializedHttpClientInstance()
            {
                if (overrideHttpClientConfiguration)
                {
                    if (skipSslCertVerification)
                    {
                        var httpClientHandler = new HttpClientHandler
                        {
                            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                        };
                        return new HttpClient(httpClientHandler, disposeHandler: true)
                        {
                            Timeout = timeout,
                        };
                    }

                    var httpClient = httpClientInstance ?? new HttpClient();
                    httpClient.Timeout = timeout;

                    return httpClient;
                }

                return httpClientInstance ?? new HttpClient();
            }
        }
    }
}
