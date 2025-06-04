using APIMatic.Core.Utilities;
using APIMatic.Core.Request;

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

        public RequestBuilder Apply(PaginationContext paginationContext)
        {
            CurrentLinkValue = CoreHelper.GetValueByReference(
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
