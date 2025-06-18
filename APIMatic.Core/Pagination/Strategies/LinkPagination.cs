using APIMatic.Core.Utilities;
using APIMatic.Core.Request;
using APIMatic.Core.Utilities.Json;

namespace APIMatic.Core.Pagination.Strategies
{
    public class LinkPagination : IPaginationStrategy
    {
        private readonly string _next;

        public string CurrentLinkValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkPagination"/> class.
        /// </summary>
        /// <param name="next">JsonPointer of a field in the response, representing the next request query URL.</param>
        public LinkPagination(string next)
        {
            _next = next;
        }

        /// <summary>
        /// Applies link-based pagination by extracting the next page URL from the response
        /// and updating the request builder with query parameters from that URL.
        /// </summary>
        /// <param name="paginationContext">
        /// The context containing the current request builder and response data (body and headers).
        /// </param>
        /// <returns>
        /// A new <see cref="RequestBuilder"/> with updated query parameters if a next link is found;
        /// the original request builder if no response is available; otherwise, <c>null</c> if no link is present.
        /// </returns>
        public RequestBuilder Apply(PaginationContext paginationContext)
        {
            CurrentLinkValue = JsonPointerResolver.ResolveScopedJsonValue(
                _next,
                paginationContext.ResponseBody,
                paginationContext.ResponseHeaders
            );

            if (!paginationContext.HasResponse)
                return RequestBuilder.RequestBuilderWithParameters(paginationContext.RequestBuilder);

            if (string.IsNullOrEmpty(CurrentLinkValue))
                return null;

            return new RequestBuilder(paginationContext.RequestBuilder).Parameters(parameters =>
            {
                foreach (var pair in CoreHelper.ExtractQueryParametersForUrl(CurrentLinkValue))
                {
                    parameters.Query(query => query.Setup(pair.Key, pair.Value));
                }
            });
        }
    }
}
