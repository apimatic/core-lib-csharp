using System;
using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Test.MockTypes.Pagination.MetaData;

namespace APIMatic.Core.Test.MockTypes.Pagination
{
    public class PagedResponse<TItem, TPage> : ApiResponse<TPage>
    {
        protected PagedResponse(ApiResponse<TPage> pageData, IEnumerable<TItem> pageItems) : base(pageData.StatusCode,
            pageData.Headers, pageData.Data)
        {
            this.Items = pageItems;
        }

        public IEnumerable<TItem> Items { get; }

        public PaginationTypes Type { get; protected set; }
    }

    public enum PaginationTypes
    {
        Offset,
        Cursor,
        Page,
        Link
    }

    internal static class PagedResponseFactory
    {
        public static PagedResponse<TItem, TPage> Create<TItem, TPage>(
            ApiResponse<TPage> pageData,
            IPaginationStrategy manager,
            IEnumerable<TItem> pageItems)
        {
            return manager switch
            {
                OffsetPagination offsetPagination => OffsetPagedResponseFactory.Create(pageData,
                    offsetPagination, pageItems),
                CursorPagination cursorPagination => CursorPagedResponseFactory.Create(pageData,
                    cursorPagination, pageItems),
                NumberPagination pagePagination => NumberPagedResponseFactory.Create(pageData,
                    pagePagination, pageItems),
                LinkPagination linkPagination => LinkPagedResponseFactory.Create(pageData, linkPagination,
                    pageItems),
                _ => throw new NotImplementedException("Unknown pagination type")
            };
        }
    }
}
