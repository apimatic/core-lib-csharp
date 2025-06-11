// <copyright file="ApiCall.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Pagination;
using APIMatic.Core.Pagination.Strategies;
using APIMatic.Core.Request;
using APIMatic.Core.Response;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;
using APIMatic.Core.Utilities.Logger;

namespace APIMatic.Core
{
    /// <summary>
    /// Deals with the execution of request created from RequestBuilder and processes the response through ResponseHandler
    /// </summary>
    /// <typeparam name="Request"> Class Type that holds http request info </typeparam>
    /// <typeparam name="Response"> Class Type that holds http response info </typeparam>
    /// <typeparam name="Context"> Class Type that holds http context info </typeparam>
    /// <typeparam name="ApiException"> Class Type that holds BaseException info </typeparam>
    /// <typeparam name="ReturnType"> Real expected type from the API endpoint </typeparam>
    /// <typeparam name="ResponseType"> Expected type of http response </typeparam>
    public class ApiCall<Request, Response, Context, ApiException, ReturnType, ResponseType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly GlobalConfiguration globalConfiguration;
        private readonly ArraySerialization arraySerialization;
        private readonly ResponseHandler<Request, Response, Context, ApiException, ResponseType> responseHandler;
        private readonly Func<Response, ResponseType, ReturnType> returnTypeCreator;
        private Enum apiCallServer;
        private RequestBuilder requestBuilder;
        private readonly ISdkLogger _sdkLogger;

        /// <summary>
        /// Creates a new instance of ApiCall
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="compatibility"></param>
        /// <param name="globalErrors"></param>
        /// <param name="serialization"></param>
        /// <param name="returnTypeCreator"></param>
        public ApiCall(GlobalConfiguration configuration, ICompatibilityFactory<Request, Response, Context, ApiException> compatibility,
            Dictionary<string, ErrorCase<Request, Response, Context, ApiException>> globalErrors = null, ArraySerialization serialization = ArraySerialization.Indexed,
            Func<Response, ResponseType, ReturnType> returnTypeCreator = null)
        {
            globalConfiguration = configuration;
            arraySerialization = serialization;
            this.returnTypeCreator = returnTypeCreator;
            responseHandler = new ResponseHandler<Request, Response, Context, ApiException, ResponseType>(compatibility, globalErrors);
            _sdkLogger = SdkLoggerFactory.Create(configuration.SdkLoggingConfiguration);
        }

        /// <summary>
        /// Configures the Server for this API call
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public ApiCall<Request, Response, Context, ApiException, ReturnType, ResponseType> Server(Enum server)
        {
            apiCallServer = server;
            return this;
        }

        /// <summary>
        /// Setup the request builder
        /// </summary>
        /// <param name="_request"></param>
        /// <returns></returns>
        public ApiCall<Request, Response, Context, ApiException, ReturnType, ResponseType> RequestBuilder(Action<RequestBuilder> _request)
        {
            requestBuilder = globalConfiguration.GlobalRequestBuilder(apiCallServer);
            requestBuilder.ArraySerialization = arraySerialization;
            requestBuilder.HasBinaryResponse = typeof(ResponseType) == typeof(Stream);
            _request(requestBuilder);
            return this;
        }

        public ApiCall<Request, Response, Context, ApiException, ReturnType, ResponseType> RequestBuilder(RequestBuilder _requestBuilder)
        {
            requestBuilder = _requestBuilder;
            return this;
        }

        /// <summary>
        /// Setup the response handler
        /// </summary>
        /// <param name="_response"></param>
        /// <returns></returns>
        public ApiCall<Request, Response, Context, ApiException, ReturnType, ResponseType> ResponseHandler(
            Action<ResponseHandler<Request, Response, Context, ApiException, ResponseType>> _response)
        {
            _response(responseHandler);
            return this;
        }

        /// <summary>
        /// Execute the request asynchronously
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ReturnType> ExecuteAsync(CancellationToken cancellationToken = default)
            => responseHandler.Result(await ExecuteRequestAsync(cancellationToken), returnTypeCreator);

        public async Task<CoreContext<CoreRequest, CoreResponse>> ExecuteRequestAsync(
            CancellationToken cancellationToken = default)
        {
            requestBuilder.AcceptHeader = responseHandler.AcceptHeader;
            CoreRequest request = await requestBuilder.Build().ConfigureAwait(false);
            globalConfiguration.ApiCallback?.OnBeforeHttpRequestEventHandler(request);
            _sdkLogger.LogRequest(request);
            CoreResponse response = await globalConfiguration.HttpClient.ExecuteAsync(request, cancellationToken)
                .ConfigureAwait(false);
            globalConfiguration.ApiCallback?.OnAfterHttpResponseEventHandler(response);
            _sdkLogger.LogResponse(response);
            return new CoreContext<CoreRequest, CoreResponse>(request, response);
        }

        public TEnumerable PaginateAsync<TItem, TEnumerable, TPageMetadata>(
            Func<ReturnType, IEnumerable<TItem>> converter,
            Func<ReturnType, IPaginationStrategy, IEnumerable<TItem>, TPageMetadata> pageResponseConverter,
            Func<TPageMetadata, IEnumerable<TItem>> pagedResponseItemConverter,
            Func<
                Func<RequestBuilder, IPaginationStrategy, CancellationToken,
                    Task<PaginatedResult<TItem, TPageMetadata>>>,
                RequestBuilder, Func<TPageMetadata, IEnumerable<TItem>>, IPaginationStrategy[],
                TEnumerable> returnTypeGetter,
            params IPaginationStrategy[] dataManagers)
        {
            return returnTypeGetter(async (reqBuilder, paginationStrategy, cancellationToken) =>
                {
                    var context = await RequestBuilder(reqBuilder).ExecuteRequestAsync(cancellationToken).ConfigureAwait(false);
                    return responseHandler.PaginatedResult(context, paginationStrategy, returnTypeCreator, converter, pageResponseConverter);
                },
                requestBuilder,
                pagedResponseItemConverter,
                dataManagers);
        }
    }
}
