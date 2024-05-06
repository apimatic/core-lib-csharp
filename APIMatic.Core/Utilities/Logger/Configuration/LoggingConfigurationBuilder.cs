using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Base class for building logging configurations.
    /// </summary>
    /// <typeparam name="T">Type of HTTP logging configuration.</typeparam>
    /// <typeparam name="TB">Type of logging configuration builder.</typeparam>
    /// <remarks>
    /// This class provides methods to configure HTTP logging settings such as body, headers, and header manipulation.
    /// </remarks>
    public abstract class LoggingConfigurationBuilder<T, TB> where T : HttpLoggingConfiguration
        where TB : LoggingConfigurationBuilder<T, TB>
    {
        protected bool LogBody;
        protected bool LogHeaders;
        protected IReadOnlyList<string> HeadersToInclude = new List<string>();
        protected IReadOnlyList<string> HeadersToExclude = new List<string>();
        protected IReadOnlyList<string> HeadersToUnmask = new List<string>();

        /// <summary>
        /// Sets whether to include the body in the logged output.
        /// </summary>
        /// <param name="includeBody">True to include the body; otherwise, false.</param>
        /// <returns>The current instance of the builder.</returns>
        public TB Body(bool includeBody)
        {
            LogBody = includeBody;
            return Self;
        }

        /// <summary>
        /// Sets whether to include the headers in the logged output.
        /// </summary>
        /// <param name="includeHeaders">True to include the headers; otherwise, false.</param>
        /// <returns>The current instance of the builder.</returns>
        public TB Headers(bool includeHeaders)
        {
            LogHeaders = includeHeaders;
            return Self;
        }

        /// <summary>
        /// Specifies headers to be included in the logged output.
        /// </summary>
        /// <param name="headersToInclude">The headers to include.</param>
        /// <returns>The current instance of the builder.</returns>
        public TB IncludeHeaders(params string[] headersToInclude)
        {
            HeadersToInclude = headersToInclude;
            return Self;
        }

        /// <summary>
        /// Specifies headers to be excluded from the logged output.
        /// </summary>
        /// <param name="headersToExclude">The headers to exclude.</param>
        /// <returns>The current instance of the builder.</returns>
        public TB ExcludeHeaders(params string[] headersToExclude)
        {
            HeadersToExclude = headersToExclude;
            return Self;
        }

        /// <summary>
        /// Specifies headers to be unmasked (e.g., sensitive data) in the logged output.
        /// </summary>
        /// <param name="headersToUnmask">The headers to unmask.</param>
        /// <returns>The current instance of the builder.</returns>
        public TB UnmaskHeaders(params string[] headersToUnmask)
        {
            HeadersToUnmask = headersToUnmask;
            return Self;
        }

        /// <summary>
        /// Gets the current instance of the builder.
        /// </summary>
        protected abstract TB Self { get; }

        /// <summary>
        /// Constructs an instance of the HTTP logging configuration based on the builder settings.
        /// </summary>
        /// <returns>An instance of the HTTP logging configuration.</returns>
        public abstract T Build();
    }
}
