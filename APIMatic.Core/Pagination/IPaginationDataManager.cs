using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination
{
    public interface IPaginationDataManager
    {
        bool TryUpdateRequestBuilder(PaginationInfo paginationInfo, out RequestBuilder updatedRequestBuilder);
    }
}
