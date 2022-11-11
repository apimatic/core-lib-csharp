// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Http.Client.Configuration;
using APIMatic.Core.Request;
using APIMatic.Core.Response;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIMatic.Core
{
    public class ApiCall<Request, Response, Context, ApiException, ReturnType, InnerType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly GlobalConfiguration globalConfiguration;
        private readonly ArraySerialization arraySerialization;
        private readonly ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory;
        private readonly ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> responseHandler;
        private Enum apiCallServer;
        private RequestBuilder requestBuilder;

        public ApiCall(GlobalConfiguration configuration, ICompatibilityFactory<Request, Response, Context, ApiException> compatibility,
            Dictionary<int, Func<Context, ApiException>> errors = null, ArraySerialization serialization = ArraySerialization.Indexed)
        {
            globalConfiguration = configuration;
            compatibilityFactory = compatibility;
            arraySerialization = serialization;
            responseHandler = new ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType>(compatibilityFactory, errors);
        }

        public ApiCall<Request, Response, Context, ApiException, ReturnType, InnerType> Server(Enum server)
        {
            apiCallServer = server;
            return this;
        }

        public ApiCall<Request, Response, Context, ApiException, ReturnType, InnerType> RequestBuilder(Action<RequestBuilder> requestBuilderAction)
        {
            requestBuilder = globalConfiguration.GlobalRequestBuilder(apiCallServer);
            requestBuilder.ArraySerialization = arraySerialization;
            requestBuilderAction(requestBuilder);
            return this;
        }

        public ApiCall<Request, Response, Context, ApiException, ReturnType, InnerType> ResponseHandler(
            Action<ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType>> responseHandlerAction)
        {
            responseHandlerAction(responseHandler);
            return this;
        }

        public async Task<ReturnType> ExecuteAsync()
        {
            CoreRequest request = requestBuilder.Build();
            request.HasBinaryResponse = responseHandler.IsBinaryResponse;
            globalConfiguration.ApiCallback?.OnBeforeHttpRequestEventHandler(request);
            CoreResponse response = await globalConfiguration.HttpClient.ExecuteAsync(request);
            globalConfiguration.ApiCallback?.OnAfterHttpResponseEventHandler(response);
            var context = new CoreContext<CoreRequest, CoreResponse>(request, response);
            return responseHandler.Result(context);
        }
    }
}
