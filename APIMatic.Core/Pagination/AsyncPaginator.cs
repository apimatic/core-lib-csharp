﻿using APIMatic.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;

namespace APIMatic.Core.Pagination
{
    /// <summary>
    /// Provides asynchronous iteration over a paginated data source by fetching items across multiple pages.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of individual items being enumerated.
    /// </typeparam>
    /// <typeparam name="TPageMetadata">
    /// The type of metadata associated with each page of results (e.g., pagination tokens, total counts).
    /// </typeparam>
    public class AsyncPaginator<TItem, TPageMetadata> : IAsyncEnumerable<TItem>
    {
        private readonly
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>>
            _apiCallExecutor;

        private readonly RequestBuilder _requestBuilder;
        private readonly IReadOnlyCollection<IPaginationStrategy> _paginationStrategies;
        private readonly Func<TPageMetadata, IEnumerable<TItem>> _pagedResponseItemConverter;

        public AsyncPaginator(
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>>
                apiCallExecutor,
            RequestBuilder requestBuilder,
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
        /// Returns an asynchronous enumerator that iterates through all items across multiple paged responses.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used to cancel the asynchronous iteration.
        /// </param>
        /// <returns>
        /// An asynchronous enumerator that yields items of type <typeparamref name="TItem"/> from all retrieved pages.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="GetPagesAsync"/> to retrieve pages and applies a converter function to extract items from each page.
        /// Supports cancellation via the provided <paramref name="cancellationToken"/>.
        /// </remarks>
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

        /// <summary>
        /// Asynchronously retrieves a sequence of page metadata objects, one page at a time.
        /// </summary>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// An asynchronous stream of <typeparamref name="TPageMetadata"/> representing metadata for each page.
        /// </returns>
        /// <remarks>
        /// Use <c>await foreach</c> to iterate over the returned pages asynchronously.
        /// </remarks>
        public async IAsyncEnumerable<TPageMetadata> GetPagesAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var paginationContext = PaginationContext.CreateDefault(_requestBuilder);
            var initialStrategy = _paginationStrategies.First();
            var requestBuilder = initialStrategy.Apply(paginationContext);

            var result = await initialStrategy.ExecuteAndYieldAsync(requestBuilder,
                    _apiCallExecutor, _pagedResponseItemConverter, cancellationToken).ConfigureAwait(false);
            
            if (result == null) yield break;

            yield return result.Value.PageMetadata;
            paginationContext = result.Value.Context;

            var strategy = _paginationStrategies.SelectFirstApplicableStrategy(paginationContext);
            if (strategy == null) yield break;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                requestBuilder = strategy.Apply(paginationContext);
                if (requestBuilder == null) yield break;

                result = await strategy.ExecuteAndYieldAsync(requestBuilder, _apiCallExecutor, 
                    _pagedResponseItemConverter, cancellationToken).ConfigureAwait(false);
                if (result == null) yield break;

                yield return result.Value.PageMetadata;
                paginationContext = result.Value.Context;
            }
        }
    }
}
