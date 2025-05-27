using APIMatic.Core.Utilities;
using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination.Types
{
    public class LinkPagination : IPaginationDataManager
    {
        private readonly string _next;

        public string LinkValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkPagination"/> class.
        /// </summary>
        /// <param name="next">JsonPointer of a field in the response, representing the next request query URL.</param>
        public LinkPagination(string next)
        {
            _next = next;
        }

        public bool TryUpdateRequestBuilder(PaginationInfo paginationInfo, out RequestBuilder updatedRequestBuilder)
        {
            updatedRequestBuilder = paginationInfo.GetLastRequestBuilder();

            LinkValue = CoreHelper.GetValueByReference(
                _next,
                paginationInfo.GetLastResponseBody(),
                paginationInfo.GetLastResponseHeaders()
            );

            if (string.IsNullOrEmpty(LinkValue))
            {
                return false;
            }

            updatedRequestBuilder.Parameters(_parameters =>
            {
                foreach (var pair in CoreHelper.ExtractQueryParametersForUrl(LinkValue))
                {
                    _parameters.Query(_query => _query.Setup(pair.Key, pair.Value));
                }
            });

            return true;
        }
    }
}
