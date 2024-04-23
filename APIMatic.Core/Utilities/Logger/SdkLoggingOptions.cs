using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger
{
    public class SdkLoggingOptions
    {

        public SdkLoggingOptions(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        internal ILogger Logger { get; }

        public LogLevel? LogLevel { get; set; }

        public bool MaskSensitiveHeaders { get; set; } = true;

        public RequestOptions Request { get; } = new RequestOptions();

        public ResponseOptions Response { get; } = new ResponseOptions();

        public bool IsConfigured => Logger != NullLogger.Instance;

        public abstract class LogBaseOptions
        {
            private static readonly List<string> NonSensitiveHeaders = new List<string>
            {
                "Accept",
                "Accept-Charset",
                "Accept-Encoding",
                "Accept-Language",
                "Access-Control-Allow-Origin",
                "Cache-Control",
                "Connection",
                "Content-Encoding",
                "Content-Language",
                "Content-Length",
                "Content-Location",
                "Content-MD5",
                "Content-Range",
                "Content-Type",
                "Date",
                "ETag",
                "Expect",
                "Expires",
                "From",
                "Host",
                "If-Match",
                "If-Modified-Since",
                "If-None-Match",
                "If-Range",
                "If-Unmodified-Since",
                "Keep-Alive",
                "Last-Modified",
                "Location",
                "Max-Forwards",
                "Pragma",
                "Range",
                "Referer",
                "Retry-After",
                "Server",
                "Trailer",
                "Transfer-Encoding",
                "Upgrade",
                "User-Agent",
                "Vary",
                "Via",
                "Warning",
                "X-Forwarded-For",
                "X-Requested-With",
                "X-Powered-By"
            };

            public bool LogBody { get; set; }

            public bool LogHeaders { get; set; }

            private IReadOnlyCollection<string> HeadersToWhiteList { get; set; } = new List<string>();

            private IReadOnlyCollection<string> HeadersToInclude { get; set; } = new List<string>();

            private IReadOnlyCollection<string> HeadersToExclude { get; set; } = new List<string>();

            public void UnmaskHeaders(params string[] whitelistHeaders) => HeadersToWhiteList = whitelistHeaders;

            public void ExcludeHeaders(params string[] excludeHeaders) => HeadersToExclude = excludeHeaders;

            public void IncludeHeaders(params string[] includeHeaders) => HeadersToInclude = includeHeaders;

            public IEnumerable<KeyValuePair<string, string>> ExtractHeadersToLog(IDictionary<string, string> headers,
                bool maskSensitiveHeaders)
            {
                IEnumerable<KeyValuePair<string, string>> headersToLog;
                if (HeadersToInclude.Any())
                {
                    headersToLog = headers.Where(h => HeadersToInclude.Contains(h.Key));
                }
                else if (HeadersToExclude.Any())
                    headersToLog = headers.Where(h => !HeadersToExclude.Contains(h.Key));
                else
                    headersToLog = headers;

                return MaskSensitiveHeaders(headersToLog, maskSensitiveHeaders);
            }

            private IDictionary<string, string> MaskSensitiveHeaders(IEnumerable<KeyValuePair<string, string>> headers,
                bool maskSensitiveHeaders)
            {
                if (!maskSensitiveHeaders) return headers.ToDictionary(h => h.Key, h => h.Value);
                return headers.Select(h => new { h.Key, Value = MaskIfHeaderIsSensitive(h.Key, h.Value) })
                    .ToDictionary(h => h.Key, h => h.Value);
            }

            private string MaskIfHeaderIsSensitive(string key, string value) =>
                NonSensitiveHeaders.Contains(key, StringComparer.OrdinalIgnoreCase) ||
                HeadersToWhiteList.Contains(key, StringComparer.OrdinalIgnoreCase)
                    ? value
                    : "**Redacted**";
        }

        public class RequestOptions : LogBaseOptions
        {
            public bool IncludeQueryInPath { get; set; }
        }

        public class ResponseOptions : LogBaseOptions { }

        public void LogBody(bool requestBody = true, bool responseBody = true)
        {
            Request.LogBody = requestBody;
            Response.LogBody = responseBody;
        }

        public void LogHeaders(bool requestHeaders = true, bool responseHeaders = true)
        {
            Request.LogHeaders = requestHeaders;
            Response.LogHeaders = responseHeaders;
        }
    }
}
