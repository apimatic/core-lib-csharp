using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Pagination;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Request;

namespace APIMatic.Core.Test.MockTypes.Pagination
{
    internal static class AsyncPaginatorFactory
    {
        public static AsyncPaginator<TItem, TPageMetadata> Create<TItem, TPageMetadata>(
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>> apiCallExecutor,
            RequestBuilder requestBuilder,
            Func<TPageMetadata, IEnumerable<TItem>> pagedResponseItemConverter,
            params IPaginationStrategy[] dataManagers)
            => new(apiCallExecutor, requestBuilder, pagedResponseItemConverter, dataManagers);
    }
}
