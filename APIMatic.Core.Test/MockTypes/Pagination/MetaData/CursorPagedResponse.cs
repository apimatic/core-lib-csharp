using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class CursorPagedResponse<TItem, TPage> : PagedResponse<TItem, TPage>
    {
        public CursorPagedResponse(ApiResponse<TPage> pageData, CursorPagination manager, IEnumerable<TItem> pageItems) : base(
            pageData, pageItems)
        {
            Type = PaginationTypes.Cursor;
            NextCursor = manager.CursorValue;
        }

        public string NextCursor { get; }
    }
    
    internal static class CursorPagedResponseFactory
    {
        public static CursorPagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            IEnumerable<TItem> pageItems)
        {
            return new CursorPagedResponse<TItem, TPage>(
                pageData,
                manager as CursorPagination, 
                pageItems);
        }
    }
}
