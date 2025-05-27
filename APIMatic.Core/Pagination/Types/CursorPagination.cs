using APIMatic.Core.Request;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Pagination.Types
{
    public class CursorPagination : IPaginationDataManager
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

        public bool TryUpdateRequestBuilder(PaginationInfo paginationInfo, out RequestBuilder updatedRequestBuilder)
        {
            updatedRequestBuilder = paginationInfo.GetLastRequestBuilder();

            CursorValue = CoreHelper.GetValueByReference(
                _output,
                paginationInfo.GetLastResponseBody(),
                paginationInfo.GetLastResponseHeaders()
            );

            bool isUpdated = false;
            updatedRequestBuilder.UpdateByReference(_input, old =>
            {
                if (CursorValue == null)
                {
                    CursorValue = old.ToString();
                }
                isUpdated = true;
                return CursorValue;
            });

            return isUpdated;
        }
    }
}
