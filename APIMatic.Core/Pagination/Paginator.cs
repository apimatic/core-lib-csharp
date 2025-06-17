using APIMatic.Core.Request;
using APIMatic.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;
using System.Threading;

namespace APIMatic.Core.Pagination
{
    /// <summary>
    /// Provides synchronous iteration over a paginated data source by fetching items across multiple pages.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of individual items to be iterated over.
    /// </typeparam>
    /// <typeparam name="TPageMetadata">
    /// The type of metadata associated with each page of results, such as pagination state or response headers.
    /// </typeparam>
    public class Paginator<TItem, TPageMetadata> : IEnumerable<TItem>
    {
        private readonly
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>>
            _apiCallExecutor;

        private readonly RequestBuilder _requestBuilder;
        private readonly IReadOnlyCollection<IPaginationStrategy> _paginationStrategies;
        private readonly Func<TPageMetadata, IEnumerable<TItem>> _pagedResponseItemConverter;

        public Paginator(
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>>
                apiCallExecutor, RequestBuilder requestBuilder,
            Func<TPageMetadata, IEnumerable<TItem>> pagedResponseItemConverter,
            IReadOnlyCollection<IPaginationStrategy> paginationStrategies)
        {
            _apiCallExecutor = apiCallExecutor;
            _requestBuilder = requestBuilder;
            _pagedResponseItemConverter = pagedResponseItemConverter;
            _paginationStrategies = paginationStrategies?.Count > 0
                ? paginationStrategies
                : throw new ArgumentException("At least one pagination strategy must be provided.",
                    nameof(paginationStrategies));
        }

        /// <summary>
        /// Returns an enumerator that iterates through all items retrieved from a paginated data source.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator{TItem}"/> that iterates through individual items across all pages.
        /// </returns>
        /// <remarks>
        /// This method synchronously retrieves pages using <see cref="GetPages"/> and extracts items from each page
        /// using the configured response-to-item converter.
        /// </remarks>
        public IEnumerator<TItem> GetEnumerator()
        {
            foreach (var pageMetaData in GetPages())
            {
                foreach (var item in _pagedResponseItemConverter(pageMetaData))
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Returns a non-generic enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> that iterates through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Retrieves metadata for each page of results from a paginated data source synchronously.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{TPageMetadata}"/> that yields metadata for each page retrieved using the configured pagination strategies.
        /// </returns>
        /// <remarks>
        /// This method applies pagination strategies to generate requests and yields metadata for each page until no further pages are available.
        /// It is the synchronous counterpart to <c>GetPagesAsync</c>.
        /// </remarks>
        public IEnumerable<TPageMetadata> GetPages()
        {
            var paginationContext = PaginationContext.CreateDefault(_requestBuilder);
            var initialStrategy = _paginationStrategies.First();
            var requestBuilder = initialStrategy.Apply(paginationContext);

            var result = CoreHelper.RunTask(initialStrategy.ExecuteAndYieldAsync(requestBuilder, _apiCallExecutor,
                _pagedResponseItemConverter, CancellationToken.None));
            if (result == null) yield break;

            yield return result.Value.PageMetadata;
            paginationContext = result.Value.Context;

            var strategy = _paginationStrategies.SelectFirstApplicableStrategy(paginationContext);
            if (strategy == null) yield break;

            while (true)
            {
                requestBuilder = strategy.Apply(paginationContext);
                if (requestBuilder == null) yield break;

                result = CoreHelper.RunTask(strategy.ExecuteAndYieldAsync(requestBuilder, _apiCallExecutor,
                    _pagedResponseItemConverter, CancellationToken.None));
                if (result == null) yield break;

                yield return result.Value.PageMetadata;
                paginationContext = result.Value.Context;
            }
        }
    }
}
