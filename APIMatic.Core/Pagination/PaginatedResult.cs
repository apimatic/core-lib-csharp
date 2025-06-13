using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Pagination
{
    public class PaginatedResult<TPage>
    {
        public CoreResponse Response { get; }
        public TPage PageMetadata { get; }

        public PaginatedResult(CoreResponse response, TPage pageMetadata)
        {
            Response = response;
            PageMetadata = pageMetadata;
        }
    }
}
