using System;
using System.Collections.Generic;
using System.Linq;
using APIMatic.Core.Types.Sdk;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger
{
    internal class SdkLogger
    {
        private readonly ILogger _logger;
        private readonly LogLevel? _logLevel;
        private readonly SdkLoggingOptions.RequestOptions _requestOptions;
        private readonly SdkLoggingOptions.ResponseOptions _responseOptions;
        private readonly bool _isConfigured;

        public SdkLogger(SdkLoggingOptions options)
        {
            var localOptions = options ?? SdkLoggingOptions.Default;
            _logger = localOptions.Logger;
            _logLevel = localOptions.LogLevel;
            _requestOptions = localOptions.Request;
            _responseOptions = localOptions.Response;
            _isConfigured = localOptions.Logger != NullLogger.Instance;
        }

        public void LogRequest(CoreRequest request)
        {
            if (!_isConfigured) return;
            var localLogLevel = _logLevel.GetValueOrDefault(LogLevel.Information);
            var contentTypeHeader = request.Headers.GetContentType();
            var url = _requestOptions.IncludeQueryInPath ? request.QueryUrl : ParseQueryPath(request.QueryUrl);
            _logger.Log(localLogLevel, "Request {HttpMethod} {Url} {ContentType}",
                request.HttpMethod,
                url,
                contentTypeHeader);

            if (_requestOptions.LogHeaders)
            {
                var headersToLog = ExtractHeadersToLog(request.Headers, _requestOptions.HeadersToInclude,
                    _requestOptions.HeadersToExclude);

                _logger.Log(localLogLevel, "Request Headers {Headers}", headersToLog);
            }

            if (_requestOptions.LogBody)
            {
                var body = request.Body ?? request.FormParameters;
                _logger.Log(localLogLevel, "Request Body {Body}", body);
            }
        }

        public void LogResponse(CoreResponse response)
        {
            if (!_isConfigured) return;
            var localLogLevel = _logLevel.GetValueOrDefault(LogLevel.Information);
            var contentTypeHeader = response.Headers.GetContentType();
            var contentLengthHeader = response.Headers.GetContentLength();
            _logger.Log(localLogLevel, "Response {HttpStatusCode} {Length} {ContentType}",
                response.StatusCode,
                contentLengthHeader,
                contentTypeHeader);

            if (_responseOptions.LogHeaders)
            {
                var headersToLog = ExtractHeadersToLog(response.Headers, _responseOptions.HeadersToInclude,
                    _responseOptions.HeadersToExclude);

                _logger.Log(localLogLevel, "Response Headers {Headers}", headersToLog);
            }

            if (_responseOptions.LogBody)
            {
                _logger.Log(localLogLevel, "Request Body {Body}", response.Body);
            }
        }

        private static string ParseQueryPath(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            int queryStringIndex = url.IndexOf('?');
            return queryStringIndex != -1 ? url.Substring(0, queryStringIndex) : url;
        }


        private static IDictionary<string, string> ExtractHeadersToLog(IDictionary<string, string> headers,
            IReadOnlyCollection<string> headersToInclude,
            IReadOnlyCollection<string> headersToExclude)
        {
            if (headersToInclude.Any())
            {
                return headers
                    .Where(h => headersToInclude.Contains(h.Key))
                    .ToDictionary(h => h.Key, h => h.Value);
            }

            if (headersToExclude.Any())
            {
                return headers
                    .Where(h => !headersToExclude.Contains(h.Key))
                    .ToDictionary(h => h.Key, h => h.Value);
            }

            return headers;
        }
    }

    internal static class HeadersExtensions
    {
        public static string GetContentType(this IDictionary<string, string> requestHeaders) =>
            requestHeaders.GetHeader("content-type") ?? requestHeaders.GetHeader("Content-Type");

        public static string GetContentLength(this IDictionary<string, string> requestHeaders) =>
            requestHeaders.GetHeader("content-length") ?? requestHeaders.GetHeader("Content-Length");

        private static string GetHeader(this IDictionary<string, string> requestHeaders, string headerName) =>
            requestHeaders.TryGetValue(headerName, out var value) ? value : null;
    }
}


