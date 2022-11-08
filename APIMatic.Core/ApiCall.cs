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
    public class ApiCall<Request, Response, Context, ApiException, ReturnType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly GlobalConfiguration globalConfiguration;
        private readonly Dictionary<int, Lazy<ApiException>> errors;
        private readonly ArrayDeserialization arrayDeserialization;
        private readonly ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory;
        private readonly ResponseHandler<Request, Response, Context, ApiException, ReturnType> responseHandler;
        private Enum apiCallServer;
        private RequestBuilder requestBuilder;

        public ApiCall(GlobalConfiguration configuration, ICompatibilityFactory<Request, Response, Context, ApiException> compatibility,
            Dictionary<int, Lazy<ApiException>> globalErrors = null, ArrayDeserialization serializationType = ArrayDeserialization.Indexed)
        {
            globalConfiguration = configuration;
            compatibilityFactory = compatibility;
            arrayDeserialization = serializationType;
            errors = globalErrors ?? new Dictionary<int, Lazy<ApiException>>();
            responseHandler = new ResponseHandler<Request, Response, Context, ApiException, ReturnType>(compatibilityFactory, errors);
        }

        public ApiCall<Request, Response, Context, ApiException, ReturnType> Server(Enum server)
        {
            apiCallServer = server;
            return this;
        }

        public ApiCall<Request, Response, Context, ApiException, ReturnType> RequestBuilder(Action<RequestBuilder> requestBuilderAction)
        {
            requestBuilder = globalConfiguration.GlobalRequestBuilder(apiCallServer);
            requestBuilder.ArrayDeserialization = arrayDeserialization;
            requestBuilderAction(requestBuilder);
            return this;
        }

        public ApiCall<Request, Response, Context, ApiException, ReturnType> ResponseHandler(
            Action<ResponseHandler<Request, Response, Context, ApiException, ReturnType>> responseHandlerAction)
        {
            responseHandlerAction(responseHandler);
            return this;
        }

        public ReturnType Execute()
        {
            Task<ReturnType> task = ExecuteAsync();
            CoreHelper.RunTaskSynchronously(task);
            return task.Result;
        }

        public async Task<ReturnType> ExecuteAsync()
        {
            CoreRequest request = requestBuilder.Build();
            globalConfiguration.ApiCallback.OnBeforeHttpRequestEventHandler(request);
            CoreResponse response = await globalConfiguration.HttpClient().ExecuteAsync(request);
            globalConfiguration.ApiCallback.OnAfterHttpResponseEventHandler(response);
            var context = new CoreContext<CoreRequest, CoreResponse>(request, response);
            return responseHandler.getResult(context);
        }
    }
}
