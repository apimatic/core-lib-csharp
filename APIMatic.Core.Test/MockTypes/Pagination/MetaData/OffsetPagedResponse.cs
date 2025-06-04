using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class OffsetPagedResponse<TItem, TPage> : PagedResponse<TItem, TPage>
    {
        public OffsetPagedResponse(ApiResponse<TPage> pageData, OffsetPagination manager, IEnumerable<TItem> pageItems) : base(
            pageData, pageItems)
        {
            Type = PaginationTypes.Offset;
            Offset = manager.OffsetValue;
        }

        public int Offset { get; }
    }

    internal static class OffsetPagedResponseFactory
    {
        public static OffsetPagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            IEnumerable<TItem> pageItems) =>
            new OffsetPagedResponse<TItem, TPage>(pageData, manager as OffsetPagination, pageItems);
    }
}
