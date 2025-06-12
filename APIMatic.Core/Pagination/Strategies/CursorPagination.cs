using APIMatic.Core.Request;
using APIMatic.Core.Utilities;
using APIMatic.Core.Utilities.Json;

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

        /// <summary>
        /// Applies pagination logic by updating the request builder with a new cursor value 
        /// extracted from the response, if available.
        /// </summary>
        /// <param name="paginationContext">The context containing the current request builder, response data, and headers.</param>
        /// <returns>
        /// A new <see cref="RequestBuilder"/> instance with updated cursor parameter if the response was processed; 
        /// otherwise, <c>null</c> if no update occurred.
        /// </returns>
        public RequestBuilder Apply(PaginationContext paginationContext)
        {
            var isUpdated = false;

            var updatedBuilder = RequestBuilder
                .RequestBuilderWithParameters(paginationContext.RequestBuilder)
                .UpdateByReference(_input, old =>
                {
                    var oldValue = old?.ToString();
                    if (!paginationContext.HasResponse)
                    {
                        CursorValue = oldValue;
                        isUpdated = true;
                        return oldValue;
                    }

                    CursorValue = JsonPointerAccessor.ResolveJsonValueByReference(
                        _output, paginationContext.ResponseBody, paginationContext.ResponseHeaders
                    ) ?? oldValue;

                    isUpdated |= CursorValue != oldValue;
                    return CursorValue;
                });

            return isUpdated ? updatedBuilder : null;
        }
    }
}
