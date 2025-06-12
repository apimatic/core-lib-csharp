using System;
using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class NumberPagedResponse<TItem, TPage> : BasePagedResponse<TItem, TPage>
    {
        public NumberPagedResponse(ApiResponse<TPage> pageData, NumberPagination manager, Func<TPage, IEnumerable<TItem>> pageToItems) : base(
            pageData, pageToItems)
        {
            PageNumber = manager.CurrentPage;
        }

        public int PageNumber { get; }
    }

    internal static class NumberPagedResponseFactory
    {
        public static NumberPagedResponse<TItem, TPage> Create<TItem, TPage>(ApiResponse<TPage> pageData,
            IPaginationStrategy manager, Func<TPage, IEnumerable<TItem>> pageToItems) =>
            new(pageData, manager as NumberPagination, pageToItems);
    }
}
