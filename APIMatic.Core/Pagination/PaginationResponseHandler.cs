using System;
using System.Collections.Generic;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Response;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Pagination
{
    public class PaginationResponseHandler<Request, Response, Context, ApiException, ResponseType, TPage, TItem, TPageMetadata>
        : ResponseHandler<Request, Response, Context, ApiException, ResponseType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly Func<TPage, IReadOnlyCollection<TItem>> _converter;
        private readonly Func<TPage, IPaginationStrategy, IEnumerable<TItem>, TPageMetadata> _pageResponseConverter;
        private readonly IPaginationStrategy _manager;

        internal PaginationResponseHandler(
            ResponseHandler<Request, Response, Context, ApiException, ResponseType> responseHandler,
            IPaginationStrategy manager,
            Func<TPage, IReadOnlyCollection<TItem>> converter,
            Func<TPage, IPaginationStrategy, IEnumerable<TItem>, TPageMetadata> pageResponseConverter
            ) : base(responseHandler)
        {
            _manager = manager;
            _converter = converter;
            _pageResponseConverter = pageResponseConverter;
        }

        // This test case covers the pagination logic `APIMatic.Core.Test.Api.Pagination.AsyncPaginator.PaginateAsync_QueryOffsetPaginationYieldsAllItems_AcrossPages`
        // To use the same logic as the `ExecuteAsync` function in `ApiCall`, we need to handle the pagination response within the `ResponseHandler.Result` method.
        // I implemented a `PaginationResponseHandler` class that inherits from the `ResponseHandler` class, and attempted to override the `.Result` method as shown below.
        // The `Result` method is expected to return an object of the `PaginatedResult` class, which includes the required `TPage` and the response object.
        // The base `.Result` method returns an object of type `TPage`, and we need to convert that `TPage` object into a `PaginatedResult<TItem, TPageMetadata>` object.
        // However, overriding the `Result` method in the `ResponseHandler` class is not feasible, because the method signature breaks due to the generic type constraints.

        // After discussing with the team, I found that a similar approach was initially attempted for Java and Python. However, since the API call is reconstructed for every request, handling pagination in the response handler violated the Separation of Concerns principle. As a result, they discarded that approach and moved the pagination logic back into the `ApiCall`, similar to what we originally did in C#.

        internal override ReturnType Result<ReturnType>(
            CoreContext<CoreRequest, CoreResponse> context,
            Func<Response, ResponseType, TPage> returnTypeCreator)
        {
            var page = base.Result(context, returnTypeCreator);
            var pageItems = _converter(page);
            var pageMeta = _pageResponseConverter(page, _manager, pageItems);
            return new PaginatedResult<TItem, TPageMetadata>(context.Response, pageItems, pageMeta);
        }
    }
}

