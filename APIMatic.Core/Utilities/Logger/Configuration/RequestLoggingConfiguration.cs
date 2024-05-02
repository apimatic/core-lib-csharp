using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public class RequestLoggingConfiguration : HttpLoggingConfiguration, IRequestLoggingConfiguration
    {
        public bool IncludeQueryInPath { get; }

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

        public class Builder : LoggingConfigurationBuilder<RequestLoggingConfiguration, Builder>
        {
            private bool _includeQueryInPath = false;

            public Builder IncludeQueryInPath(bool includeQueryInPath)
            {
                _includeQueryInPath = includeQueryInPath;
                return this;
            }
            
            protected override Builder Self => this;

            public override RequestLoggingConfiguration Build()
            {
                return new RequestLoggingConfiguration(_body, _headers, _includeQueryInPath, _headersToInclude,
                    _headersToExclude, _headersToUnmask);
            }
        }
    }
}
