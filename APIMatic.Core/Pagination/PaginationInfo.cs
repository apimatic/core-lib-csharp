using APIMatic.Core.Request;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Pagination
{
    public class PaginationInfo
    {
        private readonly int _lastDataSize;
        private readonly CoreResponse _lastResponse;
        private readonly RequestBuilder _lastRequestBuilder;

        public PaginationInfo(int lastDataSize, CoreResponse lastResponse, RequestBuilder lastRequestBuilder)
        {
            _lastDataSize = lastDataSize;
            _lastResponse = lastResponse;
            _lastRequestBuilder = lastRequestBuilder;
        }

        internal RequestBuilder GetLastRequestBuilder() => _lastRequestBuilder;
        internal string GetLastResponseBody() => _lastResponse?.Body;
        internal string GetLastResponseHeaders() => CoreHelper.JsonSerialize(_lastResponse?.Headers);
        internal int GetLastDataSize() => _lastDataSize;
    }
}
