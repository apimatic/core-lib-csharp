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

        public class Builder
        {
            private bool _body;
            private bool _headers;
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

            public ResponseLoggingConfiguration Build()
            {
                return new ResponseLoggingConfiguration(_body, _headers, _headersToInclude,
                    _headersToExclude, _headersToUnmask);
            }
        }
    }
}
