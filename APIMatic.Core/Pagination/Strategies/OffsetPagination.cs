using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination.Strategies
{
    public class OffsetPagination : IPaginationStrategy
    {
        private readonly string _input;

        public int OffsetValue { get; private set; }

        /// <summary>
        /// Constructor accepting a JSON pointer to the offset field in the request.
        /// </summary>
        /// <param name="input">JsonPointer of a field in request, representing offset.</param>
        public OffsetPagination(string input)
        {
            _input = input;
        }

        /// <summary>
        /// Applies offset-based pagination by updating the request builder with a new offset value.
        /// If the response is not available, it uses the existing offset value from the request.
        /// Otherwise, it increments the offset by the size of the data returned in the previous response.
        /// </summary>
        /// <param name="paginationContext">
        /// The context containing the current request builder, response data, and data size for pagination.
        /// </param>
        /// <returns>
        /// A new <see cref="RequestBuilder"/> with the updated offset value if pagination was applied;
        /// otherwise, <c>null</c> if no update occurred.
        /// </returns>
        public RequestBuilder Apply(PaginationContext paginationContext)
        {
            var isUpdated = false;

            var currentRequestBuilder = RequestBuilder.RequestBuilderWithParameters(paginationContext.RequestBuilder)
                .UpdateByReference(
                    _input, old =>
                    {
                        var oldValue = int.Parse(old?.ToString() ?? "0");

                        if (!paginationContext.HasResponse)
                        {
                            OffsetValue = oldValue;
                            isUpdated = true;
                            return old;
                        }

                        var newValue = oldValue + paginationContext.DataSize;
                        OffsetValue = newValue;
                        isUpdated = true;
                        return newValue;
                    });

            return isUpdated ? currentRequestBuilder : null;
        }
    }
}
