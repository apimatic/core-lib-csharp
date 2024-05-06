using System.Collections.Generic;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;

namespace APIMatic.Core.Utilities.Logger
{
    /// <summary>
    /// Provides logging functionality for SDK operations.
    /// </summary>
    internal class SdkLogger
    {
        private readonly ILogger _logger;
        private readonly LogLevel? _logLevel;
        private readonly RequestLoggingConfiguration _requestConfiguration;
        private readonly ResponseLoggingConfiguration _responseConfiguration;
        private readonly bool _isConfigured;
        private readonly bool _maskSensitiveHeaders;

        /// <summary>
        /// Initializes a new instance of the <see cref="SdkLogger"/> class.
        /// </summary>
        /// <param name="loggingConfiguration">The SDK logging configuration.</param>
        public SdkLogger(SdkLoggingConfiguration loggingConfiguration)
        {
            _logger = loggingConfiguration.Logger;
            _logLevel = loggingConfiguration.LogLevel;
            _requestConfiguration = loggingConfiguration.RequestLoggingConfiguration;
            _responseConfiguration = loggingConfiguration.ResponseLoggingConfiguration;
            _isConfigured = loggingConfiguration.IsConfigured;
            _maskSensitiveHeaders = loggingConfiguration.MaskSensitiveHeaders;
        }

        /// <summary>
        /// Logs the details of a request.
        /// </summary>
        /// <param name="request">The request to be logged.</param>
        public void LogRequest(CoreRequest request)
        {
            if (!_isConfigured) return;
            var localLogLevel = _logLevel.GetValueOrDefault(LogLevel.Information);
            var contentTypeHeader = request.Headers.GetContentType();
            var url = _requestConfiguration.IncludeQueryInPath ? request.QueryUrl : ParseQueryPath(request.QueryUrl);
            _logger.Log(localLogLevel, "Request {HttpMethod} {Url} {ContentType}",
                request.HttpMethod,
                url,
                contentTypeHeader);

            if (_requestConfiguration.Headers)
            {
                var headersToLog = _requestConfiguration.ExtractHeadersToLog(request.Headers, _maskSensitiveHeaders);
                _logger.Log(localLogLevel, "Request Headers {Headers}", headersToLog);
            }

            if (_requestConfiguration.Body)
            {
                var body = request.Body ?? request.FormParameters;
                _logger.Log(localLogLevel, "Request Body {Body}", body);
            }
        }

        /// <summary>
        /// Logs the details of a response.
        /// </summary>
        /// <param name="response">The response to be logged.</param>
        public void LogResponse(CoreResponse response)
        {
            if (!_isConfigured) return;
            var localLogLevel = _logLevel.GetValueOrDefault(LogLevel.Information);
            var contentTypeHeader = response.Headers.GetContentType();
            var contentLengthHeader = response.Headers.GetContentLength();
            _logger.Log(localLogLevel, "Response {HttpStatusCode} {ContentType} {ContentLength}",
                response.StatusCode,
                contentLengthHeader,
                contentTypeHeader);

            if (_responseConfiguration.Headers)
            {
                var headersToLog = _responseConfiguration.ExtractHeadersToLog(response.Headers, _maskSensitiveHeaders);
                _logger.Log(localLogLevel, "Response Headers {Headers}", headersToLog);
            }

            if (_responseConfiguration.Body)
            {
                _logger.Log(localLogLevel, "Response Body {Body}", response.Body);
            }
        }

        /// <summary>
        /// Parses the query path from the URL.
        /// </summary>
        /// <param name="url">The URL to parse.</param>
        /// <returns>The parsed query path.</returns>
        private static string ParseQueryPath(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            int queryStringIndex = url.IndexOf('?');
            return queryStringIndex != -1 ? url.Substring(0, queryStringIndex) : url;
        }
    }

    /// <summary>
    /// Provides extension methods for handling headers.
    /// </summary>
    internal static class HeadersExtensions
    {
        /// <summary>
        /// Gets the content type from the headers.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <returns>The content type.</returns>
        public static string GetContentType(this IDictionary<string, string> requestHeaders) =>
            requestHeaders.GetHeader("content-type") ?? requestHeaders.GetHeader("Content-Type");

        /// <summary>
        /// Gets the content length from the headers.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <returns>The content length.</returns>
        public static string GetContentLength(this IDictionary<string, string> requestHeaders) =>
            requestHeaders.GetHeader("content-length") ?? requestHeaders.GetHeader("Content-Length");

        /// <summary>
        /// Gets a specific header value from the headers.
        /// </summary>
        /// <param name="requestHeaders">The request headers.</param>
        /// <param name="headerName">The name of the header to retrieve.</param>
        /// <returns>The value of the specified header.</returns>
        private static string GetHeader(this IDictionary<string, string> requestHeaders, string headerName) =>
            requestHeaders.TryGetValue(headerName, out var value) ? value : null;
    }
}


