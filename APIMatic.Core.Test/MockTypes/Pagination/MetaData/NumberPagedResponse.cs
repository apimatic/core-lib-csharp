using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class NumberPagedResponse<TItem, TPage> : PagedResponse<TItem, TPage>
    {
        public NumberPagedResponse(ApiResponse<TPage> pageData, PagePagination manager, IEnumerable<TItem> pageItems) : base(
            pageData, pageItems)
        {
            Type = PaginationTypes.Page;
            PageNumber = manager.CurrentPage;
        }

        public int PageNumber { get; }
    }

    internal static class NumberPagedResponseFactory
    {
        public static NumberPagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            IEnumerable<TItem> pageItems) =>
            new NumberPagedResponse<TItem, TPage>(pageData, manager as PagePagination, pageItems);
    }
}
