﻿// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using MultipartContent = APIMatic.Core.Types.MultipartContent;

namespace APIMatic.Core.Http
{
    /// <summary>
    /// HttpClientWrapper.
    /// </summary>
    internal class HttpClientWrapper
    {
        private readonly int _numberOfRetries;
        private readonly int _backoffFactor;
        private readonly double _retryInterval;
        private readonly TimeSpan _maximumRetryWaitTime;
        private readonly HttpClient _client;
        private readonly IList<HttpStatusCode> _statusCodesToRetry;
        private readonly IList<HttpMethod> _requestMethodsToRetry;
        private readonly bool _overrideHttpClientConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        /// <param name="httpClientConfig"> HttpClientConfiguration object.</param>
        public HttpClientWrapper(ICoreHttpClientConfiguration httpClientConfig)
        {
            _client = httpClientConfig.HttpClientInstance;
            _overrideHttpClientConfiguration = httpClientConfig.OverrideHttpClientConfiguration;

            if (_overrideHttpClientConfiguration)
            {
                _statusCodesToRetry = httpClientConfig.StatusCodesToRetry
                .Where(val => Enum.IsDefined(typeof(HttpStatusCode), val))
                .Select(val => (HttpStatusCode)val).ToImmutableList();

                _requestMethodsToRetry = httpClientConfig.RequestMethodsToRetry
                    .Select(method => new HttpMethod(method.ToString())).ToList();

                _numberOfRetries = httpClientConfig.NumberOfRetries;
                _backoffFactor = httpClientConfig.BackoffFactor;
                _retryInterval = httpClientConfig.RetryInterval;
                _maximumRetryWaitTime = httpClientConfig.MaximumRetryWaitTime;
            }
        }

        /// <summary>
        /// Executes the http request asynchronously.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="cancellationToken"> cancellationToken.</param>
        /// <returns>Returns the HttpStringResponse.</returns>
        public async Task<CoreResponse> ExecuteAsync(CoreRequest request, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage;
            if (_overrideHttpClientConfiguration)
            {
                responseMessage = await GetCombinedPolicy(request.RetryOption).ExecuteAsync(
                    async (cancellation) => await ExecuteHttpRequest(request, cancellation).ConfigureAwait(false), cancellationToken, false)
                    .ConfigureAwait(false);
            }
            else
            {
                responseMessage = await ExecuteHttpRequest(request, cancellationToken).ConfigureAwait(false);
            }

            int statusCode = (int)responseMessage.StatusCode;
            var headers = GetCombinedResponseHeaders(responseMessage);
            Stream rawBody = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            string body = request.HasBinaryResponse ? null : await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            var response = new CoreResponse(statusCode, headers, rawBody, body);

            return response;
        }

        private async Task<HttpResponseMessage> ExecuteHttpRequest(
            CoreRequest request,
            CancellationToken cancellationToken)
        {
            var httpRequestMessage = CreateHttpRequestMessageFromRequest(request);
            return await _client.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
        }

        private HttpRequestMessage CreateHttpRequestMessageFromRequest(CoreRequest request)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(request.QueryUrl),
                Method = request.HttpMethod
            };

            AddHeadersToRequestMessage(requestMessage, request);
            if (IsHeaderOnlyHttpMethod(request.HttpMethod))
            {
                return requestMessage;
            }

            if (request.HasFormParameters)
            {
                requestMessage.Content = GetFormContent(request);
                return requestMessage;
            }

            var contentType = request.GetContentType();
            if (request.Body is CoreFileStreamInfo file)
            {
                file.FileStream.Position = 0;
                requestMessage.Content = new StreamContent(file.FileStream);
                requestMessage.Content.Headers.ContentType = GetFileStreamContentType(file, contentType);
                return requestMessage;
            }

            if (string.IsNullOrEmpty(contentType))
            {
                requestMessage.Content =
                    new StringContent(request.GetBodyAsString(), Encoding.UTF8, "text/plain");
                return requestMessage;
            }

            if (IsContentTypeJsonUtf8(contentType))
            {
                requestMessage.Content =
                    new StringContent(request.GetBodyAsString(), Encoding.UTF8, "application/json");
                return requestMessage;
            }

            if (request.Body == null)
            {
                return requestMessage;
            }

            requestMessage.Content = GetByteArrayContentFromRequestBody(request.Body);
            GetByteArrayContentType(requestMessage.Content.Headers, contentType);
            return requestMessage;
        }
        private static void AddHeadersToRequestMessage(HttpRequestMessage requestMessage, CoreRequest request)
        {
            foreach (var headers in request.Headers ?? Enumerable.Empty<KeyValuePair<string, string>>())
            {
                requestMessage.Headers.TryAddWithoutValidation(headers.Key, headers.Value);
            }
        }

        private static HttpContent GetFormContent(CoreRequest request)
        {
            if (CheckFormParametersForMultiPart(request.FormParameters))
            {
                return GetMultipartFormDataContentFromRequest(request);
            }
            return new FormUrlEncodedContent(request.FormParameters
                .Select(param => new KeyValuePair<string, string>(param.Key, param.Value.ToString())).ToList());
        }
        private static bool IsContentTypeJsonUtf8(string contentType)
        {
            return contentType.EqualsIgnoreCase("application/json") || contentType.EqualsIgnoreCase("application/json; charset=utf-8");
        }
        private static void GetByteArrayContentType(HttpContentHeaders contentHeader, string contentType)
        {
            try
            {
                contentHeader.ContentType = MediaTypeHeaderValue.Parse(contentType);
            }
            catch (Exception)
            {
                contentHeader.TryAddWithoutValidation("content-type", contentType);
            }
        }

        private static MultipartFormDataContent GetMultipartFormDataContentFromRequest(CoreRequest request)
        {
            MultipartFormDataContent formContent = new MultipartFormDataContent();

            foreach (var param in request.FormParameters)
            {
                if (param.Value is MultipartContent wrapperObject)
                {
                    wrapperObject.Rewind();
                    formContent.Add(wrapperObject.ToHttpContent(param.Key));
                }
                else
                {
                    formContent.Add(new StringContent(param.Value.ToString()), param.Key);
                }
            }

            return formContent;
        }

        private static ByteArrayContent GetByteArrayContentFromRequestBody(object requestBody)
        {
            byte[] bytes;
            if (requestBody is Stream stream)
            {
                stream.Position = 0;
                using (BinaryReader br = new BinaryReader(stream))
                {
                    bytes = br.ReadBytes((int)stream.Length);
                }
            }
            else if (requestBody is byte[] byteArray)
            {
                bytes = byteArray;
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes((string)requestBody);
            }

            return new ByteArrayContent(bytes ?? Array.Empty<byte>());
        }

        private static MediaTypeHeaderValue GetFileStreamContentType(CoreFileStreamInfo file, string contentType)
        {
            if (!string.IsNullOrWhiteSpace(file.ContentType))
            {
                return new MediaTypeHeaderValue(file.ContentType);
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                return new MediaTypeHeaderValue(contentType);
            }

            return new MediaTypeHeaderValue("application/octet-stream");
        }

        private static bool IsHeaderOnlyHttpMethod(HttpMethod method)
        {
            return !method.Equals(HttpMethod.Delete) && !method.Equals(HttpMethod.Post) &&
                   !method.Equals(HttpMethod.Put) && !method.Equals(new HttpMethod("PATCH"));
        }

        private static bool CheckFormParametersForMultiPart(IReadOnlyCollection<KeyValuePair<string, object>> formParameters)
        {
            return formParameters.Any(f => f.Value is MultipartContent) ||
                    formParameters.Any(f => f.Value is CoreFileStreamInfo);
        }

        private bool ShouldRetry(HttpResponseMessage response, RetryOption retryOption)
        {
            bool isWhiteListedMethod = _requestMethodsToRetry.Contains(response.RequestMessage.Method);

            return retryOption.IsRetryAllowed(isWhiteListedMethod) &&
                (_statusCodesToRetry.Contains(response.StatusCode) || response?.Headers?.RetryAfter != null);
        }

        private static TimeSpan GetServerWaitDuration(DelegateResult<HttpResponseMessage> response)
        {
            var retryAfter = response?.Result?.Headers?.RetryAfter;
            if (retryAfter == null)
            {
                return TimeSpan.Zero;
            }

            return retryAfter.Date.HasValue
                ? retryAfter.Date.Value - DateTime.UtcNow
                : retryAfter.Delta.GetValueOrDefault(TimeSpan.Zero);
        }

        private AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(RetryOption retryOption) =>
            Policy.HandleResult<HttpResponseMessage>(response => ShouldRetry(response, retryOption))
                .Or<TaskCanceledException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: _numberOfRetries,
                    sleepDurationProvider: (retryAttempt, result, context) =>
                        TimeSpan.FromMilliseconds(Math.Max(GetExponentialWaitTime(retryAttempt),
                            GetServerWaitDuration(result).TotalMilliseconds)),
                    onRetryAsync: async (result, timespan, retryAttempt, context) =>
                    {
                        await Task.CompletedTask.ConfigureAwait(false);
                    });

        private AsyncTimeoutPolicy GetTimeoutPolicy()
        {
            return _maximumRetryWaitTime.TotalSeconds == 0
                ? Policy.TimeoutAsync(Timeout.InfiniteTimeSpan)
                : Policy.TimeoutAsync(_maximumRetryWaitTime);
        }

        private AsyncPolicyWrap<HttpResponseMessage> GetCombinedPolicy(RetryOption retryOption) =>
            GetTimeoutPolicy().WrapAsync(GetRetryPolicy(retryOption));

        private double GetExponentialWaitTime(int retryAttempt)
        {
            double noise = new Random().NextDouble() * 100;
            return (1000 * _retryInterval * Math.Pow(_backoffFactor, retryAttempt - 1)) + noise;
        }

        private static Dictionary<string, string> GetCombinedResponseHeaders(HttpResponseMessage responseMessage)
        {
            var headers = responseMessage.Headers.ToDictionary(l => l.Key, k => k.Value.First(), StringComparer.InvariantCultureIgnoreCase);
            if (responseMessage.Content != null)
            {
                foreach (var contentHeader in responseMessage.Content.Headers)
                {
                    if (headers.ContainsKey(contentHeader.Key))
                    {
                        continue;
                    }

                    headers.Add(contentHeader.Key, contentHeader.Value.First());
                }
            }
            return headers;
        }
    }
}
