using System;
using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for logging HTTP responses.
    /// </summary>
    public class ResponseLoggingConfiguration : HttpLoggingConfiguration
    {
        public ResponseLoggingConfiguration(bool body, bool headers, IReadOnlyCollection<string> headersToInclude,
            IReadOnlyCollection<string> headersToExclude, IReadOnlyCollection<string> headersToUnmask) : base(body,
            headers, headersToInclude, headersToExclude, headersToUnmask)
        {
        }

        public static ResponseLoggingConfiguration Default() =>
            new ResponseLoggingConfiguration(
                false,
                false,
                Enumerable.Empty<string>().ToList(),
                Enumerable.Empty<string>().ToList(),
                Enumerable.Empty<string>().ToList());

        public static ResponseLoggingConfiguration Builder(bool logBody, bool logHeaders,
            IReadOnlyCollection<string> headersToInclude, IReadOnlyCollection<string> headersToExclude,
            IReadOnlyCollection<string> headersToUnmask) =>
                new ResponseLoggingConfiguration(logBody,
                    logHeaders,
                    headersToInclude ?? Enumerable.Empty<string>().ToList(),
                    headersToExclude ?? Enumerable.Empty<string>().ToList(),
                    headersToUnmask ?? Enumerable.Empty<string>().ToList());
    }
}
