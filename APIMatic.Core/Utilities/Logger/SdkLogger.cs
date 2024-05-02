using System.Collections.Generic;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;

namespace APIMatic.Core.Utilities.Logger
{
    internal class SdkLogger
    {
        private readonly ILogger _logger;
        private readonly LogLevel? _logLevel;
        private readonly RequestLoggingConfiguration _requestConfiguration;
        private readonly ResponseLoggingConfiguration _responseConfiguration;
        private readonly bool _isConfigured;
        private readonly bool _maskSensitiveHeaders;

        public SdkLogger(ISdkLoggingConfiguration loggingConfiguration)
        {
            _logger = loggingConfiguration.Logger;
            _logLevel = loggingConfiguration.LogLevel;
            _requestConfiguration = (RequestLoggingConfiguration)loggingConfiguration.RequestLoggingConfiguration;
            _responseConfiguration = (ResponseLoggingConfiguration)loggingConfiguration.ResponseLoggingConfiguration;
            _isConfigured = loggingConfiguration.IsConfigured;
            _maskSensitiveHeaders = loggingConfiguration.MaskSensitiveHeaders;
        }

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


