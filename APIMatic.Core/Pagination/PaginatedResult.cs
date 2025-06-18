using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Pagination
{
    public class PaginatedResult<TPageMetadata>
    {
        public CoreResponse Response { get; }
        public TPageMetadata PageMetadata { get; }

        internal PaginatedResult(CoreResponse response, TPageMetadata pageMetadata)
        {
            Response = response;
            PageMetadata = pageMetadata;
        }
    }
}
