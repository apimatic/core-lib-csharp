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

        public class Builder
        {
            private bool _body;
            private bool _headers;
            private bool _includeQueryInPath;
            private IReadOnlyList<string> _headersToInclude = new List<string>();
            private IReadOnlyList<string> _headersToExclude = new List<string>();
            private IReadOnlyList<string> _headersToUnmask = new List<string>();

            public Builder Body(bool includeBody)
            {
                _body = includeBody;
                return this;
            }

            public Builder Headers(bool includeHeaders)
            {
                _headers = includeHeaders;
                return this;
            }

            public Builder IncludeQueryInPath(bool includeQueryInPath)
            {
                _includeQueryInPath = includeQueryInPath;
                return this;
            }

            public Builder IncludeHeaders(params string[] headersToInclude)
            {
                _headersToInclude = headersToInclude.ToArray();
                return this;
            }

            public Builder ExcludeHeaders(params string[] headersToExclude)
            {
                _headersToExclude = headersToExclude.ToArray();
                return this;
            }

            public Builder UnmaskHeaders(params string[] headersToUnmask)
            {
                _headersToUnmask = headersToUnmask.ToArray();
                return this;
            }

            public RequestLoggingConfiguration Build()
            {
                return new RequestLoggingConfiguration(_body, _headers, _includeQueryInPath, _headersToInclude,
                    _headersToExclude, _headersToUnmask);
            }
        }
    }
}
