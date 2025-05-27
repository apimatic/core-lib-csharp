using System;
using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination.Types
{

    public class OffsetPagination : IPaginationDataManager
    {
        private readonly string input;

        public int OffsetValue { get; private set; }
        /// <summary>
        /// Constructor accepting a JSON pointer to the offset field in the request.
        /// </summary>
        /// <param name="input">JsonPointer of a field in request, representing offset.</param>
        public OffsetPagination(string input)
        {
            this.input = input;
        }

        public bool TryUpdateRequestBuilder(PaginationInfo paginationInfo, out RequestBuilder updatedRequestBuilder)
        {
            updatedRequestBuilder = paginationInfo.GetLastRequestBuilder();

            if (input == null)
            {
                return false;
            }

            bool isUpdated = false;

            updatedRequestBuilder.UpdateByReference(input, old =>
            {
                int oldValue = Convert.ToInt32(old);
                int newValue = oldValue + paginationInfo.GetLastDataSize();
                isUpdated = true;
                OffsetValue = newValue;
                return OffsetValue;
            });

            return isUpdated;
        }
    }
}
