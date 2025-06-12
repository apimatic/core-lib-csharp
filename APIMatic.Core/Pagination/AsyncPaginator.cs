using APIMatic.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;

namespace APIMatic.Core.Pagination
{
    public class AsyncPaginator<TItem, TPageMetadata> : IAsyncEnumerable<TItem>
    {
        private readonly Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>> _apiCallExecutor;
        private readonly RequestBuilder _requestBuilder;
        private readonly IPaginationStrategy[] _paginationStrategies;
        private readonly Func<TPageMetadata, IEnumerable<TItem>> _pagedResponseItemConverter;

        public AsyncPaginator(
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>> apiCallExecutor,
            RequestBuilder requestBuilder,
            Func<TPageMetadata, IEnumerable<TItem>> pagedResponseItemConverter,
            IPaginationStrategy[] paginationStrategies)
        {
            _apiCallExecutor = apiCallExecutor;
            _requestBuilder = requestBuilder;
            _pagedResponseItemConverter = pagedResponseItemConverter;
            _paginationStrategies = paginationStrategies;
        }

        public async IAsyncEnumerator<TItem> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            await foreach (var pageMetaData in GetPagesAsync(cancellationToken))
            {
                foreach (var item in _pagedResponseItemConverter(pageMetaData))
                {
                    yield return item;
                }
            }
        }

        public async IAsyncEnumerable<TPageMetadata> GetPagesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var paginationContext = PaginationContext.CreateDefault(_requestBuilder);

            foreach (var paginationStrategy in _paginationStrategies)
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var requestBuilder = paginationStrategy.Apply(paginationContext);
                    if (requestBuilder == null)
                        break;

                    var result = await _apiCallExecutor(requestBuilder, paginationStrategy, cancellationToken)
                        .ConfigureAwait(false);

                    var items = _pagedResponseItemConverter(result.PageMetadata);
                    if (items == null || !items.Any())
                        yield break;

                    paginationContext = PaginationContext.Create(items.Count(), result.Response, requestBuilder);

                    yield return result.PageMetadata;
                }
            }
        }
    }
}
