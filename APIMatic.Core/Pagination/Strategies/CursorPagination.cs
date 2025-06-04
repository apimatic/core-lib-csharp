using APIMatic.Core.Request;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Pagination.Strategies
{
    public class CursorPagination : IPaginationStrategy
    {
        private readonly string _output;
        private readonly string _input;
        
        public string CursorValue { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorPagination"/> class.
        /// </summary>
        /// <param name="output">JsonPointer of a field received in the response, representing the next cursor.</param>
        /// <param name="input">JsonPointer of a field in the request, representing the cursor.</param>
        public CursorPagination(string output, string input)
        {
            _output = output;
            _input = input;
        }

        public RequestBuilder Apply(PaginationContext paginationContext)
        {
            var isUpdated = false;
            var currentRequestBuilder = RequestBuilder.RequestBuilderWithParameters(paginationContext.RequestBuilder)
                .UpdateByReference(
                    _input, old =>
                    {
                        var oldValue = old?.ToString();
                        if (!paginationContext.HasResponse)
                        {
                            CursorValue = oldValue;
                            isUpdated = true;
                            return oldValue;
                        }

                        var cursorValue = CoreHelper.GetValueByReference(
                            _output,
                            paginationContext.ResponseBody,
                            paginationContext.ResponseHeaders
                        );
                        
                        if (cursorValue == null)
                        {
                            return oldValue;
                        }

                        CursorValue = cursorValue;
                        isUpdated = true;
                        return CursorValue;
                    });

            return isUpdated ? currentRequestBuilder : null;
        }
    }
}
