using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination.Types
{
    public class PagePagination : IPaginationDataManager
    {
        private readonly string _input;

        public int CurrentPage { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="PagePagination"/> class.
        /// </summary>
        /// <param name="input">JsonPointer of a field in the request, representing the page number.</param>
        public PagePagination(string input)
        {
            _input = input;
        }

        public bool TryUpdateRequestBuilder(PaginationInfo paginationInfo, out RequestBuilder updatedRequestBuilder)
        {
            updatedRequestBuilder = paginationInfo.GetLastRequestBuilder();

            if (string.IsNullOrEmpty(_input))
            {
                return false;
            }

            bool isUpdated = false;
            updatedRequestBuilder.UpdateByReference(_input, oldValue =>
            {
                if (!int.TryParse(oldValue?.ToString(), out int currentPage))
                {
                    currentPage = 1;
                }

                isUpdated = true;
                CurrentPage = currentPage + 1;
                return CurrentPage;
            });

            return isUpdated;
        }
    }
}
