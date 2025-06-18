using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Pagination;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Request;

namespace APIMatic.Core.Test.MockTypes.Pagination
{
    internal static class PaginatorFactory
    {
        public static Paginator<TItem, TPageMetadata> Create<TItem, TPageMetadata>(
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>> apiCallExecutor,
            RequestBuilder requestBuilder,
            Func<TPageMetadata, IEnumerable<TItem>> responseToItems,
            params IPaginationStrategy[] dataManagers)
            => new(apiCallExecutor, requestBuilder, responseToItems, dataManagers);
    }
}
