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
