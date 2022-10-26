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
using APIMatic.Core.Http.Client.Configuration;
using APIMatic.Core.Http.Request;
using APIMatic.Core.Types;
using APIMatic.Core.Utilities;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using MultipartContent = APIMatic.Core.Types.MultipartContent;

namespace APIMatic.Core.Http.Client
{
    /// <summary>
    /// HttpClientWrapper.
    /// </summary>
    internal class HttpClientWrapper : IHttpClient
    {
        private static char parameterSeparator = '&';
        private readonly int numberOfRetries;
        private readonly int backoffFactor;
        private readonly double retryInterval;
        private readonly TimeSpan maximumRetryWaitTime;
        private ArrayDeserialization arrayDeserializationFormat = ArrayDeserialization.Indexed;
        private HttpClient client;
        private IList<HttpStatusCode> statusCodesToRetry;
        private IList<HttpMethod> requestMethodsToRetry;
        private bool overrideHttpClientConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientWrapper"/> class.
        /// </summary>
        /// <param name="httpClientConfig"> HttpClientConfiguration object.</param>
        public HttpClientWrapper(CoreHttpClientConfiguration httpClientConfig)
        {
            this.client = httpClientConfig.HttpClientInstance;
            this.overrideHttpClientConfiguration = httpClientConfig.OverrideHttpClientConfiguration;

            if (overrideHttpClientConfiguration)
            {
                this.statusCodesToRetry = httpClientConfig.StatusCodesToRetry
                .Where(val => Enum.IsDefined(typeof(HttpStatusCode), val))
                .Select(val => (HttpStatusCode)val).ToImmutableList();

                this.requestMethodsToRetry = httpClientConfig.RequestMethodsToRetry
                    .Select(method => new HttpMethod(method.ToString())).ToList();

                this.numberOfRetries = httpClientConfig.NumberOfRetries;
                this.backoffFactor = httpClientConfig.BackoffFactor;
                this.retryInterval = httpClientConfig.RetryInterval;
                this.maximumRetryWaitTime = httpClientConfig.MaximumRetryWaitTime;
                this.client.Timeout = httpClientConfig.Timeout;
            }
        }

        /// <summary>
        /// OnBeforeHttpRequestEvent.
        /// </summary>
        public event OnBeforeHttpRequestEventHandler OnBeforeHttpRequestEvent;

        /// <summary>
        /// OnAfterHttpResponseEvent.
        /// </summary>
        public event OnAfterHttpResponseEventHandler OnAfterHttpResponseEvent;

        /// <summary>
        /// Executes the http request.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="retryConfiguration">The <see cref="RetryConfiguration"/> for request.</param>
        /// <returns>HttpStringResponse.</returns>
        public CoreResponse ExecuteAsString(CoreRequest request, RetryConfiguration retryConfiguration = null)
        {
            Task<CoreResponse> t = this.ExecuteAsStringAsync(request, retryConfiguration);
            CoreHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Executes the http request asynchronously.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="cancellationToken"> cancellationToken.</param>
        /// <param name="retryConfiguration">The <see cref="RetryConfiguration"/> for request.</param>
        /// <returns>Returns the HttpStringResponse.</returns>
        public async Task<CoreResponse> ExecuteAsStringAsync(
            CoreRequest request,
            RetryConfiguration retryConfiguration = null,
            CancellationToken cancellationToken = default)
        {
            // raise the on before request event.
            this.RaiseOnBeforeHttpRequestEvent(request);

            HttpResponseMessage responseMessage;

            if (overrideHttpClientConfiguration)
            {
                responseMessage = await this.GetCombinedPolicy(retryConfiguration).ExecuteAsync(
                    async (cancellation) => await this.HttpResponseMessage(request, cancellation).ConfigureAwait(false), cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                responseMessage = await this.HttpResponseMessage(request, cancellationToken).ConfigureAwait(false);
            }

            int statusCode = (int)responseMessage.StatusCode;
            var headers = GetCombinedResponseHeaders(responseMessage);
            Stream rawBody = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
            string body = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            var response = new CoreResponse(statusCode, headers, rawBody, body);

            // raise the on after response event.
            this.RaiseOnAfterHttpResponseEvent(response);

            return response;
        }

        /// <summary>
        /// Executes the http request.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="retryConfiguration">The <see cref="RetryConfiguration"/> for request.</param>
        /// <returns>HttpResponse.</returns>
        public CoreResponse ExecuteAsBinary(CoreRequest request, RetryConfiguration retryConfiguration = null)
        {
            Task<CoreResponse> t = this.ExecuteAsBinaryAsync(request, retryConfiguration);
            CoreHelper.RunTaskSynchronously(t);
            return t.Result;
        }

        /// <summary>
        /// Executes the http request asynchronously.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <param name="retryConfiguration">The <see cref="RetryConfiguration"/> for request.</param>
        /// <returns>HttpResponse.</returns>
        public async Task<CoreResponse> ExecuteAsBinaryAsync(
            CoreRequest request,
            RetryConfiguration retryConfiguration = null,
            CancellationToken cancellationToken = default)
        {
            // raise the on before request event.
            this.RaiseOnBeforeHttpRequestEvent(request);

            HttpResponseMessage responseMessage;

            if (overrideHttpClientConfiguration)
            {
                responseMessage = await this.GetCombinedPolicy(retryConfiguration).ExecuteAsync(
                    async (cancellation) => await this.HttpResponseMessage(request, cancellation).ConfigureAwait(false), cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                responseMessage = await this.HttpResponseMessage(request, cancellationToken).ConfigureAwait(false);
            }

            int statusCode = (int)responseMessage.StatusCode;
            var headers = GetCombinedResponseHeaders(responseMessage);
            Stream rawBody = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);

            CoreResponse response = new CoreResponse(statusCode, headers, rawBody);

            // raise the on after response event.
            this.RaiseOnAfterHttpResponseEvent(response);

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

        private void RaiseOnBeforeHttpRequestEvent(CoreRequest request)
        {
            if ((this.OnBeforeHttpRequestEvent != null) && (request != null))
            {
                this.OnBeforeHttpRequestEvent(this, request);
            }
        }

        private void RaiseOnAfterHttpResponseEvent(CoreResponse response)
        {
            if ((this.OnAfterHttpResponseEvent != null) && (response != null))
            {
                this.OnAfterHttpResponseEvent(this, response);
            }
        }

        private async Task<HttpResponseMessage> HttpResponseMessage(
            CoreRequest request,
            CancellationToken cancellationToken)
        {
            var queryBuilder = new StringBuilder(request.QueryUrl);

            if (request.QueryParameters != null)
            {
                CoreHelper.AppendUrlWithQueryParameters(queryBuilder, request.QueryParameters, this.arrayDeserializationFormat, parameterSeparator);
            }

            // validate and preprocess url.
            string queryUrl = CoreHelper.CleanUrl(queryBuilder);

            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(queryUrl),
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

                        if (request.Body is Stream)
                        {
                            Stream s = (Stream)request.Body;
                            s.Position = 0;
                            using (BinaryReader br = new BinaryReader(s))
                            {
                                bytes = br.ReadBytes((int)s.Length);
                            }
                        }
                        else if (request.Body is byte[])
                        {
                            bytes = (byte[])request.Body;
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
                        if (param.Value is CoreFileStreamInfo fileParam)
                        {
                            var fileContent = new MultipartFileContent(fileParam);
                            fileContent.Rewind();
                            formContent.Add(fileContent.ToHttpContent(param.Key));
                        }
                        else if (param.Value is MultipartContent wrapperObject)
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

            return await this.client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
        }

        private bool ShouldRetry(HttpResponseMessage response, RetryConfiguration retryConfiguration)
        {
            bool isWhiteListedMethod = this.requestMethodsToRetry.Contains(response.RequestMessage.Method);

            return retryConfiguration.RetryOption.IsRetryAllowed(isWhiteListedMethod) &&
                (this.statusCodesToRetry.Contains(response.StatusCode) || response?.Headers?.RetryAfter != null);
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

        private AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(RetryConfiguration retryConfiguration)
        {
            return Policy.HandleResult<HttpResponseMessage>(response => this.ShouldRetry(response, retryConfiguration))
                .Or<TaskCanceledException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(
                retryCount: this.numberOfRetries,
                sleepDurationProvider: (retryAttempt, result, context) =>
                TimeSpan.FromMilliseconds(Math.Max(this.GetExponentialWaitTime(retryAttempt), this.GetServerWaitDuration(result).TotalMilliseconds)),
                onRetryAsync: async (result, timespan, retryAttempt, context) => await Task.CompletedTask);
        }

        private AsyncTimeoutPolicy GetTimeoutPolicy()
        {
            return this.maximumRetryWaitTime.TotalSeconds == 0
                ? Policy.TimeoutAsync(Timeout.InfiniteTimeSpan)
                : Policy.TimeoutAsync(this.maximumRetryWaitTime);
        }

        private AsyncPolicyWrap<HttpResponseMessage> GetCombinedPolicy(RetryConfiguration retryConfiguration = null)
        {
            if (retryConfiguration == null)
            {
                retryConfiguration = DefaultRetryConfiguration.RetryConfiguration;
            }

            return this.GetTimeoutPolicy().WrapAsync(this.GetRetryPolicy(retryConfiguration));
        }

        private double GetExponentialWaitTime(int retryAttempt)
        {
            double noise = new Random().NextDouble() * 100;
            return (1000 * this.retryInterval * Math.Pow(this.backoffFactor, retryAttempt - 1)) + noise;

        }
    }
}
