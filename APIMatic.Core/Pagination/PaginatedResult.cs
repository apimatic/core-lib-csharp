using APIMatic.Core.Types.Sdk;
using System.Collections.Generic;

namespace APIMatic.Core.Pagination
{
    public class PaginatedResult<TItem, TPage>
    {
        public CoreResponse Response { get; }
        public IEnumerable<TItem> Items { get; }
        public TPage PageMetadata { get; }

        public PaginatedResult(CoreResponse response, IEnumerable<TItem> items, TPage pageMetadata)
        {
            Response = response;
            Items = items;
            PageMetadata = pageMetadata;
        }
    }
}
