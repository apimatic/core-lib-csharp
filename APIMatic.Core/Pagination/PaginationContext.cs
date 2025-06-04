using APIMatic.Core.Request;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Pagination
{
    public class PaginationContext
    {
        private readonly CoreResponse _response;

        internal int DataSize { get; }
        internal RequestBuilder RequestBuilder { get; }
        internal bool HasResponse => _response != null;
        internal string ResponseBody => _response?.Body;
        internal string ResponseHeaders => CoreHelper.JsonSerialize(_response?.Headers);

        private PaginationContext(RequestBuilder requestBuilder, int dataSize = 0, CoreResponse response = null)
        {
            _response = response;

            DataSize = dataSize;
            RequestBuilder = requestBuilder;
        }

        internal static PaginationContext CreateDefault(RequestBuilder requestBuilder) =>
            new PaginationContext(RequestBuilder.RequestBuilderWithParameters(requestBuilder));
        
        internal static PaginationContext Create(int dataSize, CoreResponse response, RequestBuilder requestBuilder) =>
            new PaginationContext(requestBuilder, dataSize, response);
    }
}
