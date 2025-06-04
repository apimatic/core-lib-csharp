using APIMatic.Core.Request;
using APIMatic.Core.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIMatic.Core.Pagination.Strategies;

namespace APIMatic.Core.Pagination
{
    public class Paginator<TItem, TPage> : IEnumerable<TItem>
    {
        private readonly Func<RequestBuilder, IPaginationStrategy, Task<PaginatedResult<TItem, TPage>>> _apiCallExecutor;
        private readonly RequestBuilder _requestBuilder;
        private readonly IPaginationStrategy[] _paginationStrategies;
        private readonly Func<TPage, IEnumerable<TItem>> _pagedResponseItemConverter;

        protected Paginator(
            Func<RequestBuilder, IPaginationStrategy, Task<PaginatedResult<TItem, TPage>>> apiCallExecutor, RequestBuilder requestBuilder,
            Func<TPage, IEnumerable<TItem>> pagedResponseItemConverter,
            IPaginationStrategy[] paginationStrategies)
        {
            _apiCallExecutor = apiCallExecutor;
            _requestBuilder = requestBuilder;
            _pagedResponseItemConverter = pagedResponseItemConverter;
            _paginationStrategies = paginationStrategies;
        }

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

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerable<TPage> GetPages()
        {
            var paginationContext = PaginationContext.CreateDefault(_requestBuilder);
            foreach (var paginationStrategy in _paginationStrategies)
            {
                while (true)
                {
                    var requestBuilder = paginationStrategy.Apply(paginationContext);
                    if (requestBuilder == null)
                        break;

                    var result = CoreHelper.RunTask(_apiCallExecutor(requestBuilder, paginationStrategy));

                    var items = result?.Items;
                    if (items == null || !items.Any())
                        break;

                    paginationContext = PaginationContext.Create(items.Count(), result.Response, requestBuilder);

                    yield return result.PageMetadata;
                }
            }
        }
    }
}
