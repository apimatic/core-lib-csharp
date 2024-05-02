using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public abstract class LoggingConfigurationBuilder<T, TB> where T : IHttpLoggingConfiguration
        where TB : LoggingConfigurationBuilder<T, TB>
    {
        protected bool _body;
        protected bool _headers;
        protected IReadOnlyList<string> _headersToInclude = new List<string>();
        protected IReadOnlyList<string> _headersToExclude = new List<string>();
        protected IReadOnlyList<string> _headersToUnmask = new List<string>();

        public TB Body(bool includeBody)
        {
            _body = includeBody;
            return Self;
        }

        public TB Headers(bool includeHeaders)
        {
            _headers = includeHeaders;
            return Self;
        }

        public TB IncludeHeaders(params string[] headersToInclude)
        {
            _headersToInclude = headersToInclude.ToArray();
            return Self;
        }

        public TB ExcludeHeaders(params string[] headersToExclude)
        {
            _headersToExclude = headersToExclude.ToArray();
            return Self;
        }

        public TB UnmaskHeaders(params string[] headersToUnmask)
        {
            _headersToUnmask = headersToUnmask.ToArray();
            return Self;
        }

        protected abstract TB Self { get; }

        public abstract T Build();
    }
}
