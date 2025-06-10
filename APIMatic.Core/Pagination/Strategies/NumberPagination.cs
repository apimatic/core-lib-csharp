using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination.Strategies
{
    public class NumberPagination : IPaginationStrategy
    {
        private readonly string _input;

        public int CurrentPage { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberPagination"/> class.
        /// </summary>
        /// <param name="input">JsonPointer of a field in the request, representing the page number.</param>
        public NumberPagination(string input)
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
                        var oldValue = int.TryParse(old?.ToString(), out var value) ? value : 0;

                        if (!paginationContext.HasResponse)
                        {
                            CurrentPage = oldValue;
                            isUpdated = true;
                            return old;
                        }

                        var newValue = oldValue + 1;
                        CurrentPage = newValue;
                        isUpdated = true;
                        return newValue;
                    });

            return isUpdated ? currentRequestBuilder : null;
        }
    }
}
