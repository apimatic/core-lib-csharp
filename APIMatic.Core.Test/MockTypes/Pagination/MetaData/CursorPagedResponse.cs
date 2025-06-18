using System;
using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class CursorPagedResponse<TItem, TPage> : BasePagedResponse<TItem, TPage>
    {
        public CursorPagedResponse(ApiResponse<TPage> pageData, CursorPagination manager,
            Func<TPage, IEnumerable<TItem>> pageToItems) : base(
            pageData, pageToItems)
        {
            NextCursor = manager.CursorValue;
        }

        public string NextCursor { get; }
    }

    internal static class CursorPagedResponseFactory
    {
        public static CursorPagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            Func<TPage, IEnumerable<TItem>> pageToItems) => new(pageData, manager as CursorPagination, pageToItems);
    }
}
