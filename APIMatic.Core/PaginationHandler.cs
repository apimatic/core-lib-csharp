using APIMatic.Core.Types.Sdk;
using System;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Pagination;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Request;
namespace APIMatic.Core
{
    public class PaginationHandler<TReturnType, TPageableType, TPageMetadata>
    {
        private readonly Func<RequestBuilder, CancellationToken, Task<(TReturnType result, CoreResponse response)>>
            _requestExecutor;

        private readonly RequestBuilder _requestBuilder;
        private readonly Func<TReturnType, IPaginationStrategy, TPageMetadata> _createPageResponse;

        private readonly Func<Func<RequestBuilder, IPaginationStrategy, CancellationToken,
                Task<PaginatedResult<TPageMetadata>>>,
            RequestBuilder, TPageableType> _createPageable;

        public PaginationHandler(
            Func<RequestBuilder, CancellationToken, Task<(TReturnType result, CoreResponse response)>> requestExecutor,
            RequestBuilder requestBuilder,
            Func<TReturnType, IPaginationStrategy, TPageMetadata> createPageResponse,
            Func<Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>>,
                RequestBuilder, TPageableType> createPageable)
        {
            _requestExecutor = requestExecutor;
            _requestBuilder = requestBuilder;
            _createPageResponse = createPageResponse;
            _createPageable = createPageable;
        }

        public TPageableType Paginate()
        {
            return _createPageable(async (reqBuilder, strategy, cancellationToken) =>
                {
                    var (result, response) =
                        await _requestExecutor(reqBuilder, cancellationToken).ConfigureAwait(false);
                    var pageMeta = _createPageResponse(result, strategy);
                    return new PaginatedResult<TPageMetadata>(response, pageMeta);
                },
                _requestBuilder);
        }
    }

    public static class PaginationFactory
    {
        public static PaginationHandler<ReturnType, TPageableType, TPageMetadata> CreateHandler<Request, Response, Context, ApiException, ReturnType, ResponseType, TPageableType, TPageMetadata>(
            ApiCall<Request, Response, Context, ApiException, ReturnType, ResponseType> apiCall,
            Func<ReturnType, IPaginationStrategy, TPageMetadata> createPageResponse,
            Func<Func<RequestBuilder, IPaginationStrategy, CancellationToken, Task<PaginatedResult<TPageMetadata>>>,
                RequestBuilder, TPageableType> createPageable)
            where Request : CoreRequest
            where Response : CoreResponse
            where Context : CoreContext<Request, Response>
            where ApiException : CoreApiException<Request, Response, Context>
        {
            return new PaginationHandler<ReturnType, TPageableType, TPageMetadata>((builder, token) =>
                    apiCall.RequestBuilder(builder).ExecuteRequestAsync(token),
                apiCall.GetRequestBuilder(), createPageResponse, createPageable);
        }
    }
}
