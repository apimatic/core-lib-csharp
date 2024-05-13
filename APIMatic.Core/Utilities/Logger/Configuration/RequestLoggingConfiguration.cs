using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for logging HTTP requests.
    /// </summary>
    public class RequestLoggingConfiguration : HttpLoggingConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether to include the query string in the logged path of HTTP requests.
        /// </summary>
        public bool IncludeQueryInPath { get; }

        public RequestLoggingConfiguration(bool body, bool headers, IReadOnlyCollection<string> headersToInclude,
            IReadOnlyCollection<string> headersToExclude, IReadOnlyCollection<string> headersToUnmask,
            bool includeQueryInPath) : base(body, headers, headersToInclude, headersToExclude, headersToUnmask)
        {
            IncludeQueryInPath = includeQueryInPath;
        }

        internal static RequestLoggingConfiguration Default() =>
            new RequestLoggingConfiguration(
                false,
                false,
                Enumerable.Empty<string>().ToList(),
                Enumerable.Empty<string>().ToList(),
                Enumerable.Empty<string>().ToList(),
                false);

        public static RequestLoggingConfiguration Builder(bool logBody, bool logHeaders, bool includeQueryInPath,
            IReadOnlyCollection<string> headersToInclude, IReadOnlyCollection<string> headersToExclude,
            IReadOnlyCollection<string> headersToUnmask) =>
            new RequestLoggingConfiguration(logBody,
                logHeaders,
                headersToInclude ?? Enumerable.Empty<string>().ToList(),
                headersToExclude ?? Enumerable.Empty<string>().ToList(),
                headersToUnmask ?? Enumerable.Empty<string>().ToList(), 
                includeQueryInPath);
    }
}
