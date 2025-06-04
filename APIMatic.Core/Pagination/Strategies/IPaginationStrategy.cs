using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination.Strategies
{
    public interface IPaginationStrategy
    {
        RequestBuilder Apply(PaginationContext paginationContext);
    }
}
