using System;
using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public abstract class HttpLoggingConfiguration : IHttpLoggingConfiguration
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
            //"X-Powered-By"
        };

        public bool Body { get; protected set; }

        public bool Headers { get; protected set; }

        public IReadOnlyCollection<string> HeadersToInclude { get; protected set; }

        public IReadOnlyCollection<string> HeadersToExclude { get; protected set; }

        public IReadOnlyCollection<string> HeadersToUnmask { get; protected set; }

        internal IEnumerable<KeyValuePair<string, string>> ExtractHeadersToLog(IDictionary<string, string> headers,
            bool maskSensitiveHeaders)
        {
            IEnumerable<KeyValuePair<string, string>> headersToLog;
            if (HeadersToInclude.Any())
            {
                headersToLog = headers.Where(h => HeadersToInclude.Contains(h.Key, StringComparer.OrdinalIgnoreCase));
            }
            else if (HeadersToExclude.Any())
                headersToLog = headers.Where(h => !HeadersToExclude.Contains(h.Key, StringComparer.OrdinalIgnoreCase));
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
            HeadersToUnmask.Contains(key, StringComparer.OrdinalIgnoreCase)
                ? value
                : "**Redacted**";

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Body} , " +
                   $"{Headers} , " +
                   $"{HeadersToInclude} , " +
                   $"{HeadersToExclude} , " +
                   $"{HeadersToUnmask} ";
        }
    }
}
