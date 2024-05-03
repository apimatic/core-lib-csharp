using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for logging HTTP requests.
    /// </summary>
    public class RequestLoggingConfiguration : HttpLoggingConfiguration, IRequestLoggingConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether to include the query string in the logged path of HTTP requests.
        /// </summary>
        public bool IncludeQueryInPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingConfiguration"/> class with the specified
        /// parameters.
        /// </summary>
        /// <param name="body">Specifies whether to include the body in the logged output.</param>
        /// <param name="headers">Specifies whether to include the headers in the logged output.</param>
        /// <param name="includeQueryInPath">Specifies whether to include the query string in the logged path of HTTP
        /// requests.</param>
        /// <param name="headersToInclude">The headers to include in the logged output.</param>
        /// <param name="headersToExclude">The headers to exclude from the logged output.</param>
        /// <param name="headersToUnmask">The headers to unmask (e.g., sensitive data) in the logged output.</param>
        private RequestLoggingConfiguration(bool body, bool headers, bool includeQueryInPath,
            IReadOnlyCollection<string> headersToInclude,
            IReadOnlyCollection<string> headersToExclude, IReadOnlyCollection<string> headersToUnmask)
        {
            Body = body;
            Headers = headers;
            IncludeQueryInPath = includeQueryInPath;
            HeadersToInclude = headersToInclude;
            HeadersToExclude = headersToExclude;
            HeadersToUnmask = headersToUnmask;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "RequestConfiguration: " +
                   $"{base.ToString()} , " +
                   $"{IncludeQueryInPath} ";
        }

        /// <summary>
        /// Returns a builder instance to modify this <see cref="RequestLoggingConfiguration"/>.
        /// </summary>
        /// <returns>A builder instance to modify this <see cref="RequestLoggingConfiguration"/>.</returns>
        public Builder ToBuilder()
        {
            var builder = new Builder()
                .Body(Body)
                .Headers(Headers)
                .IncludeQueryInPath(IncludeQueryInPath)
                .IncludeHeaders(HeadersToInclude.ToArray())
                .ExcludeHeaders(HeadersToExclude.ToArray())
                .UnmaskHeaders(HeadersToUnmask.ToArray());

            return builder;
        }

        /// <summary>
        /// Builder class for constructing <see cref="RequestLoggingConfiguration"/>.
        /// </summary>
        public class Builder : LoggingConfigurationBuilder<RequestLoggingConfiguration, Builder>
        {
            private bool _includeQueryInPath = false;

            /// <summary>
            /// Sets whether to include the query string in the logged path of HTTP requests.
            /// </summary>
            /// <param name="includeQueryInPath">True to include the query string; otherwise, false.</param>
            /// <returns>The current builder instance.</returns>
            public Builder IncludeQueryInPath(bool includeQueryInPath)
            {
                _includeQueryInPath = includeQueryInPath;
                return this;
            }
            
            /// <inheritdoc/>
            protected override Builder Self => this;

            /// <inheritdoc/>
            public override RequestLoggingConfiguration Build()
            {
                return new RequestLoggingConfiguration(LogBody, LogHeaders, _includeQueryInPath, HeadersToInclude,
                    HeadersToExclude, HeadersToUnmask);
            }
        }
    }
}
