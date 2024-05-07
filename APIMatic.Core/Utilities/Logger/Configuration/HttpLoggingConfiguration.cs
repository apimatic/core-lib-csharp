using System;
using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Abstract class representing configuration settings for HTTP request/response logging.
    /// </summary>
    public abstract class HttpLoggingConfiguration
    {
        /// <summary>
        /// List of non-sensitive headers for unmasking.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether to log the body of the HTTP request/response.
        /// </summary>
        public bool Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log the headers of the HTTP request/response.
        /// </summary>
        public bool Headers { get; set; }

        /// <summary>
        /// Gets or sets the collection of headers to include in the logged output.
        /// </summary>
        public IReadOnlyCollection<string> HeadersToInclude { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the collection of headers to exclude from the logged output.
        /// </summary>
        public IReadOnlyCollection<string> HeadersToExclude { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the collection of headers to unmask (e.g., replace sensitive data) in the logged output.
        /// </summary>
        public IReadOnlyCollection<string> HeadersToUnmask { get; set; } = new List<string>();

        /// <summary>
        /// Retrieves the headers to be logged based on the logging configuration, headers, and sensitivity
        /// masking configuration.
        /// </summary>
        /// <param name="headers">The headers to be evaluated for logging.</param>
        /// <param name="maskSensitiveHeaders">Determines whether sensitive headers should be masked in the
        /// log.</param>
        /// <returns>Headers to be logged, considering the provided configuration and sensitivity masking.</returns>
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

        /// <summary>
        /// Mask sensitive headers from the given headers.
        /// </summary>
        /// <param name="headers">The list of headers to filter.</param>
        /// <param name="maskSensitiveHeaders">The list of headers to unmask.</param>
        /// <returns>Masked headers.</returns>
        private IDictionary<string, string> MaskSensitiveHeaders(IEnumerable<KeyValuePair<string, string>> headers,
            bool maskSensitiveHeaders)
        {
            if (!maskSensitiveHeaders) return headers.ToDictionary(h => h.Key, h => h.Value);
            return headers.Select(h => new { h.Key, Value = MaskIfHeaderIsSensitive(h.Key, h.Value) })
                .ToDictionary(h => h.Key, h => h.Value);
        }

        /// <summary>
        /// Mask the header value if not found in NonSensitiveHeaders and HeadersToUnmask.
        /// </summary>
        /// <param name="key">Header key.</param>
        /// <param name="value">Header Value.</param>
        /// <returns>The masked header value or "**Redacted**" if the header is sensitive.</returns>
        private string MaskIfHeaderIsSensitive(string key, string value) =>
            NonSensitiveHeaders.Contains(key, StringComparer.OrdinalIgnoreCase) ||
            HeadersToUnmask.Contains(key, StringComparer.OrdinalIgnoreCase)
                ? value
                : "**Redacted**";
    }
}
