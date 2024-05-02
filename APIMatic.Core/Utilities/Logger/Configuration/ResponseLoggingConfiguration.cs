using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public class ResponseLoggingConfiguration : HttpLoggingConfiguration, IResponseLoggingConfiguration
    {
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

        public class Builder : LoggingConfigurationBuilder<ResponseLoggingConfiguration, Builder>
        {
            protected override Builder Self => this;
            
            public override ResponseLoggingConfiguration Build()
            {
                return new ResponseLoggingConfiguration(_body, _headers, _headersToInclude, _headersToExclude,
                    _headersToUnmask);
            }
        }
    }
}
