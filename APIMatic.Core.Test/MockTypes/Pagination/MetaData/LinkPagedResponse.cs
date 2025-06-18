using System;
using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.MockTypes.Pagination.MetaData
{
    public class LinkPagedResponse<TItem, TPage> : BasePagedResponse<TItem, TPage>
    {
        public LinkPagedResponse(ApiResponse<TPage> pageData, LinkPagination manager,
            Func<TPage, IEnumerable<TItem>> pageToItems) : base(
            pageData, pageToItems)
        {
            NextLink = manager.CurrentLinkValue;
        }

        public string NextLink { get; }
    }

    internal static class LinkPagedResponseFactory
    {
        public static LinkPagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            Func<TPage, IEnumerable<TItem>> pageToItems) => new(pageData, manager as LinkPagination, pageToItems);
    }
}
