using System;
using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Test.MockTypes.Pagination.MetaData;

namespace APIMatic.Core.Test.MockTypes.Pagination
{
    public class BasePagedResponse<TItem, TPage> : ApiResponse<TPage>
    {
        protected BasePagedResponse(ApiResponse<TPage> pageData, Func<TPage, IEnumerable<TItem>> pageToItems) : base(
            pageData.StatusCode,
            pageData.Headers, pageData.Data)
        {
            this.Data = pageData.Data;
            this.Items = pageToItems(pageData.Data);
        }

        public new TPage Data { get; }

        public IEnumerable<TItem> Items { get; }
    }

    internal static class BasePagedResponseFactory
    {
        public static BasePagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            Func<TPage, IEnumerable<TItem>> pageToItems)
        {
            return manager switch
            {
                OffsetPagination offsetPagination => OffsetPagedResponseFactory.Create(pageData,
                    offsetPagination, pageToItems),
                CursorPagination cursorPagination => CursorPagedResponseFactory.Create(pageData,
                    cursorPagination, pageToItems),
                NumberPagination pagePagination => NumberPagedResponseFactory.Create(pageData,
                    pagePagination, pageToItems),
                LinkPagination linkPagination => LinkPagedResponseFactory.Create(pageData, linkPagination,
                    pageToItems),
                _ => throw new NotImplementedException("Unknown pagination type")
            };
        }
    }
}
