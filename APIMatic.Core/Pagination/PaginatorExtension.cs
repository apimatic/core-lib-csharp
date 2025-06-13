using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Request;

namespace APIMatic.Core.Pagination
{
    internal static class PaginatorExtension
    {
        public static IPaginationStrategy SelectFirstApplicableStrategy(
            this IEnumerable<IPaginationStrategy> strategies, PaginationContext context) => strategies
            .Select(s => new { Strategy = s, Builder = s.Apply(context) })
            .FirstOrDefault(x => x.Builder != null)
            ?.Strategy;

        public static async Task<(TPageMetadata PageMetadata, PaginationContext Context)?> ExecuteAndYieldAsync<TItem,
            TPageMetadata>(
            this IPaginationStrategy strategy,
            RequestBuilder requestBuilder,
            Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>>
                apiCallExecutor,
            Func<TPageMetadata, IEnumerable<TItem>> pagedResponseItemConverter,
            CancellationToken cancellationToken = default)
        {
            var result = await apiCallExecutor(requestBuilder, strategy, cancellationToken).ConfigureAwait(false);
            var items = pagedResponseItemConverter(result.PageMetadata);
            if (items == null || !items.Any()) return null;

            var updatedContext = PaginationContext.Create(items.Count(), result.Response, requestBuilder);
            return (result.PageMetadata, updatedContext);
        }
    }
}
