// <copyright file="HttpClientWrapper.cs" company="APIMatic">
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
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
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
        private HttpClient _client;
        private IList<HttpStatusCode> _statusCodesToRetry;
        private IList<HttpMethod> _requestMethodsToRetry;
        private bool _overrideHttpClientConfiguration;

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
                _client.Timeout = httpClientConfig.Timeout;
            }
        }

        /// <summary>
        /// Executes the http request asynchronously.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="cancellationToken"> cancellationToken.</param>
        /// <param name="retryConfiguration">The <see cref="RetryConfiguration"/> for request.</param>
        /// <returns>Returns the HttpStringResponse.</returns>
        public async Task<CoreResponse> ExecuteAsync(CoreRequest request, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage;
    
            if (_overrideHttpClientConfiguration)
            {
                responseMessage = await GetCombinedPolicy(request.RetryOption).ExecuteAsync(
                    async (cancellation) => await HttpResponseMessage(request, cancellation).ConfigureAwait(false), cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                responseMessage = await HttpResponseMessage(request, cancellationToken).ConfigureAwait(false);
            }

            int statusCode = (int)responseMessage.StatusCode;
            var headers = GetCombinedResponseHeaders(responseMessage);
            Stream rawBody = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            string body = request.HasBinaryResponse ? null : await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            var response = new CoreResponse(statusCode, headers, rawBody, body);

            return response;
        }

        private static Dictionary<string, string> GetCombinedResponseHeaders(HttpResponseMessage responseMessage)
        {
            var headers = responseMessage.Headers.ToDictionary(l => l.Key, k => k.Value.First());
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

        private async Task<HttpResponseMessage> HttpResponseMessage(
            CoreRequest request,
            CancellationToken cancellationToken)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(request.QueryUrl),
                Method = request.HttpMethod,
            };

            if (request.Headers != null)
            {
                foreach (var headers in request.Headers)
                {
                    requestMessage.Headers.TryAddWithoutValidation(headers.Key, headers.Value);
                }
            }

            if (request.HttpMethod.Equals(HttpMethod.Delete) || request.HttpMethod.Equals(HttpMethod.Post) || request.HttpMethod.Equals(HttpMethod.Put) || request.HttpMethod.Equals(new HttpMethod("PATCH")))
            {
                bool multipartRequest = request.FormParameters != null &&
                        (request.FormParameters.Any(f => f.Value is MultipartContent) ||
                        request.FormParameters.Any(f => f.Value is CoreFileStreamInfo));

                if (request.Body != null)
                {
                    string contentType = request.Headers.Where(p => p.Key.Equals("content-type", StringComparison.InvariantCultureIgnoreCase))
                                                    .Select(x => x.Value)
                                                    .FirstOrDefault();

                    if (request.Body is CoreFileStreamInfo file)
                    {
                        file.FileStream.Position = 0;
                        requestMessage.Content = new StreamContent(file.FileStream);
                        if (!string.IsNullOrWhiteSpace(file.ContentType))
                        {
                            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                        }
                        else if (!string.IsNullOrEmpty(contentType))
                        {
                            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                        }
                        else
                        {
                            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        }
                    }
                    else if (!string.IsNullOrEmpty(contentType) && contentType.Equals("application/json; charset=utf-8", StringComparison.OrdinalIgnoreCase))
                    {
                        requestMessage.Content = new StringContent((string)request.Body ?? string.Empty, Encoding.UTF8, "application/json");
                    }
                    else if (!string.IsNullOrEmpty(contentType))
                    {
                        byte[] bytes = null;

                        if (request.Body is Stream stream)
                        {
                            stream.Position = 0;
                            using (BinaryReader br = new BinaryReader(stream))
                            {
                                bytes = br.ReadBytes((int)stream.Length);
                            }
                        }
                        else if (request.Body is byte[] byteArray)
                        {
                            bytes = byteArray;
                        }
                        else
                        {
                            bytes = Encoding.UTF8.GetBytes((string)request.Body);
                        }

                        requestMessage.Content = new ByteArrayContent(bytes ?? Array.Empty<byte>());

                        try
                        {
                            requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                        }
                        catch (Exception)
                        {
                            requestMessage.Content.Headers.TryAddWithoutValidation("content-type", contentType);
                        }
                    }
                    else
                    {
                        requestMessage.Content = new StringContent(request.Body.ToString() ?? string.Empty, Encoding.UTF8, "text/plain");
                    }
                }
                else if (multipartRequest)
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

                    requestMessage.Content = formContent;
                }
                else if (request.FormParameters != null)
                {
                    var parameters = new List<KeyValuePair<string, string>>();
                    foreach (var param in request.FormParameters)
                    {
                        parameters.Add(new KeyValuePair<string, string>(param.Key, param.Value.ToString()));
                    }

                    requestMessage.Content = new FormUrlEncodedContent(parameters);
                }
            }

            return await _client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        }

        private bool ShouldRetry(HttpResponseMessage response, RetryOption retryOption)
        {
            bool isWhiteListedMethod = _requestMethodsToRetry.Contains(response.RequestMessage.Method);

            return retryOption.IsRetryAllowed(isWhiteListedMethod) &&
                (_statusCodesToRetry.Contains(response.StatusCode) || response?.Headers?.RetryAfter != null);
        }

        private TimeSpan GetServerWaitDuration(DelegateResult<HttpResponseMessage> response)
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
                retryCount: this._numberOfRetries,
                sleepDurationProvider: (retryAttempt, result, context) =>
                TimeSpan.FromMilliseconds(Math.Max(GetExponentialWaitTime(retryAttempt), GetServerWaitDuration(result).TotalMilliseconds)),
                onRetryAsync: async (result, timespan, retryAttempt, context) => await Task.CompletedTask);

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
            return (1000 * this._retryInterval * Math.Pow(_backoffFactor, retryAttempt - 1)) + noise;

        }
    }
}
