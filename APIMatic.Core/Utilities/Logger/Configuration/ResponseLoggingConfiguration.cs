using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for logging HTTP responses.
    /// </summary>
    public class ResponseLoggingConfiguration : HttpLoggingConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseLoggingConfiguration"/> class with the specified
        /// parameters.
        /// </summary>
        /// <param name="body">Specifies whether to include the body in the logged output.</param>
        /// <param name="headers">Specifies whether to include the headers in the logged output.</param>
        /// <param name="headersToInclude">The headers to include in the logged output.</param>
        /// <param name="headersToExclude">The headers to exclude from the logged output.</param>
        /// <param name="headersToUnmask">The headers to unmask (e.g., sensitive data) in the logged output.</param>
        private ResponseLoggingConfiguration(bool body, bool headers, IReadOnlyCollection<string> headersToInclude,
            IReadOnlyCollection<string> headersToExclude, IReadOnlyCollection<string> headersToUnmask)
        {
            Body = body;
            Headers = headers;
            HeadersToInclude = headersToInclude;
            HeadersToExclude = headersToExclude;
            HeadersToUnmask = headersToUnmask;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "ResponseConfiguration: " +
                   $"{base.ToString()} ";
        }

        /// <summary>
        /// Returns a builder instance to modify this <see cref="ResponseLoggingConfiguration"/>.
        /// </summary>
        /// <returns>A builder instance to modify this <see cref="ResponseLoggingConfiguration"/>.</returns>
        public Builder ToBuilder()
        {
            var builder = new Builder()
                .Body(Body)
                .Headers(Headers)
                .IncludeHeaders(HeadersToInclude.ToArray())
                .ExcludeHeaders(HeadersToExclude.ToArray())
                .UnmaskHeaders(HeadersToUnmask.ToArray());

            return builder;
        }

        /// <summary>
        /// Builder class for constructing <see cref="ResponseLoggingConfiguration"/>.
        /// </summary>
        public class Builder : LoggingConfigurationBuilder<ResponseLoggingConfiguration, Builder>
        {
            /// <inheritdoc/>
            protected override Builder Self => this;
            
            /// <inheritdoc/>
            public override ResponseLoggingConfiguration Build()
            {
                return new ResponseLoggingConfiguration(LogBody, LogHeaders, HeadersToInclude, HeadersToExclude,
                    HeadersToUnmask);
            }
        }
    }
}
