﻿// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Response
{
    public class ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly Dictionary<int, Func<Context, ApiException>> errors;
        private readonly ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory;
        private bool nullOn404 = false;
        private Func<string, InnerType> deserializer = responseBody => CoreHelper.JsonDeserialize<InnerType>(responseBody);
        private Func<Response, InnerType, ReturnType> returnTypeCreator;

        internal ResponseHandler(ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory,
            Dictionary<int, Func<Context, ApiException>> errors)
            => (this.compatibilityFactory, this.errors) = (compatibilityFactory, errors);

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> ErrorCase(
            int statusCode, Func<Context, ApiException> error)
        {
            errors[statusCode] = error;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> NullOn404()
        {
            nullOn404 = true;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> Deserializer(
            Func<string, InnerType> deserializer)
        {
            this.deserializer = deserializer;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> ReturnTypeCreator(
            Func<Response, InnerType, ReturnType> returnTypeCreator)
        {
            this.returnTypeCreator = returnTypeCreator;
            return this;
        }

        private ApiException ResponseError(CoreContext<CoreRequest, CoreResponse> context)
        {
            Context httpContext = compatibilityFactory.CreateHttpContext(context.Request, context.Response);
            if (errors.TryGetValue(context.Response.StatusCode, out var errorFunction))
            {
                return errorFunction(httpContext);
            }
            if (errors.TryGetValue(0, out var defaultErrorFunction))
            {
                return defaultErrorFunction(httpContext);
            }
            return compatibilityFactory.CreateApiException("HTTP Response Not OK", context);
        }

        internal ReturnType Result(CoreContext<CoreRequest, CoreResponse> context)
        {
            if (context.IsFailure())
            {
                if (nullOn404 && context.Response.StatusCode == 404)
                {
                    return default;
                }
                throw ResponseError(context);
            }
            InnerType result = deserializer(context.Response.Body);
            if (result is ReturnType deserializedResult)
            {
                return deserializedResult;
            }
            if (returnTypeCreator != null)
            {
                return returnTypeCreator(compatibilityFactory.CreateHttpResponse(context.Response), result);
            }
            return default;
        }
    }
}
