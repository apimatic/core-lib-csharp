using System.Collections.Generic;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHttpLoggingConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether to log the body of the HTTP request/response.
        /// </summary>
        bool Body { get; }

        /// <summary>
        /// Gets a value indicating whether to log the headers of the HTTP request/response.
        /// </summary>
        bool Headers { get; }

        /// <summary>
        /// Gets the collection of headers to include in the logged output.
        /// </summary>
        IReadOnlyCollection<string> HeadersToInclude { get; }

        /// <summary>
        /// Gets the collection of headers to exclude from the logged output.
        /// </summary>
        IReadOnlyCollection<string> HeadersToExclude { get; }

        /// <summary>
        /// Gets the collection of headers to unmask (e.g., replace sensitive data) in the logged output.
        /// </summary>
        IReadOnlyCollection<string> HeadersToUnmask { get; }
    }
}
