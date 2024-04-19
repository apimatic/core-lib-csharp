using System.Collections.Generic;
using APIMatic.Core.Types.Sdk;
using Microsoft.Extensions.Logging;

namespace APIMatic.Core.Utilities.Logger
{
    internal class SdkLogger
    {
        private readonly ILogger _logger;
        private readonly LogLevel? _logLevel;
        private readonly SdkLoggingOptions.RequestOptions _requestOptions;
        private readonly SdkLoggingOptions.ResponseOptions _responseOptions;
        private readonly bool _isConfigured;
        private readonly bool _maskSensitiveHeaders;

        public SdkLogger(SdkLoggingOptions options)
        {
            _logger = options.Logger;
            _logLevel = options.LogLevel;
            _requestOptions = options.Request;
            _responseOptions = options.Response;
            _isConfigured = options.IsConfigured;
            _maskSensitiveHeaders = options.MaskSensitiveHeaders;
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
                var headersToLog = _requestOptions.ExtractHeadersToLog(request.Headers, _maskSensitiveHeaders);
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
            _logger.Log(localLogLevel, "Response {HttpStatusCode} {ContentType} ContentLength: {ContentLength}",
                response.StatusCode,
                contentLengthHeader,
                contentTypeHeader);

            if (_responseOptions.LogHeaders)
            {
                var headersToLog = _responseOptions.ExtractHeadersToLog(response.Headers, _maskSensitiveHeaders);
                _logger.Log(localLogLevel, "Response Headers {Headers}", headersToLog);
            }

            if (_responseOptions.LogBody)
            {
                _logger.Log(localLogLevel, "Response Body {Body}", response.Body);
            }
        }

        private static string ParseQueryPath(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            int queryStringIndex = url.IndexOf('?');
            return queryStringIndex != -1 ? url.Substring(0, queryStringIndex) : url;
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


