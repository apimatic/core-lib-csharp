using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class LinkPagedResponse<TItem, TPage> : PagedResponse<TItem, TPage>
    {
        public LinkPagedResponse(ApiResponse<TPage> pageData, LinkPagination manager, IEnumerable<TItem> pageItems) : base(
            pageData, pageItems)
        {
            Type = PaginationTypes.Link;
            NextLink = manager.CurrentLinkValue;
        }

        public string NextLink { get; }
    }
    
    internal static class LinkPagedResponseFactory
    {
        public static LinkPagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            IEnumerable<TItem> pageItems)
        {
            return new LinkPagedResponse<TItem, TPage>(
                pageData,
                manager as LinkPagination, 
                pageItems);
        }
    }
}
