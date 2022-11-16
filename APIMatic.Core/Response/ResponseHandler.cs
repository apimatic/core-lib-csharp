// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using APIMatic.Core.Http.Client.Configuration;
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
        private Func<InnerType, Context, InnerType> contextAdder;
        private Func<Response, InnerType, ReturnType> returnTypeCreator;

        internal ContentType AcceptHeader { get; set; } = ContentType.JSON;

        internal ResponseHandler(ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory,
            Dictionary<int, Func<Context, ApiException>> errors)
        {
            this.compatibilityFactory = compatibilityFactory;
            this.errors = errors ?? new Dictionary<int, Func<Context, ApiException>>();
            if (CoreHelper.IsScalarType(typeof(InnerType)))
            {
                AcceptHeader = ContentType.SCALAR;
            }
        }

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

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> XmlResponse()
        {
            AcceptHeader = ContentType.XML;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> Deserializer(
            Func<string, InnerType> deserializer)
        {
            this.deserializer = deserializer;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> ContextAdder(
            Func<InnerType, Context, InnerType> contextAdder)
        {
            this.contextAdder = contextAdder;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ReturnType, InnerType> ReturnTypeCreator(
            Func<Response, InnerType, ReturnType> returnTypeCreator)
        {
            this.returnTypeCreator = returnTypeCreator;
            return this;
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
            InnerType result = ConvertedResponse(context.Response);
            result = contextAdder(result, compatibilityFactory.CreateHttpContext(context.Request, context.Response));
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

        private InnerType ConvertedResponse(CoreResponse response)
        {
            if (response.RawBody is InnerType streamResponse)
            {
                return streamResponse;
            }
            return deserializer(response.Body);
        }
    }
}
