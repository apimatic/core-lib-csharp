using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class OffsetPagedResponse<TItem, TPage> : BasePagedResponse<TItem, TPage>
    {
        public OffsetPagedResponse(ApiResponse<TPage> pageData, OffsetPagination manager,
            Func<TPage, IEnumerable<TItem>> pageToItems) : base(
            pageData, pageToItems)
        {
            Offset = manager.OffsetValue;
        }

        public int Offset { get; }
    }

    internal static class OffsetPagedResponseFactory
    {
        public static OffsetPagedResponse<TItem, TPage> Create<TItem, TPage>(ApiResponse<TPage> pageData,
            IPaginationStrategy manager, Func<TPage, IEnumerable<TItem>> pageToItems) =>
            new(pageData, manager as OffsetPagination, pageToItems);
    }
}
