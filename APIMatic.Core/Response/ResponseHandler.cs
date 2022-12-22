// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Response
{
    public class ResponseHandler<Request, Response, Context, ApiException, ResponseType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly Dictionary<int, Func<Context, ApiException>> errors;
        private readonly ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory;
        private bool nullOn404 = false;
        private Func<string, ResponseType> deserializer = responseBody => CoreHelper.JsonDeserialize<ResponseType>(responseBody);
        private Func<ResponseType, Context, ResponseType> contextAdder = (result, context) => result;

        internal ContentType AcceptHeader { get; set; } = ContentType.JSON;

        internal ResponseHandler(ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory,
            Dictionary<int, Func<Context, ApiException>> errors)
        {
            this.compatibilityFactory = compatibilityFactory;
            this.errors = errors ?? new Dictionary<int, Func<Context, ApiException>>();
            if (CoreHelper.IsScalarType(typeof(ResponseType)))
            {
                AcceptHeader = ContentType.SCALAR;
            }
        }

        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> ErrorCase(
            int statusCode, Func<Context, ApiException> error)
        {
            errors[statusCode] = error;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> NullOn404()
        {
            nullOn404 = true;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> XmlResponse()
        {
            AcceptHeader = ContentType.XML;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> Deserializer(
            Func<string, ResponseType> deserializer)
        {
            this.deserializer = deserializer;
            return this;
        }

        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> ContextAdder(
            Func<ResponseType, Context, ResponseType> contextAdder)
        {
            this.contextAdder = contextAdder;
            return this;
        }

        internal ReturnType Result<ReturnType>(CoreContext<CoreRequest, CoreResponse> context, Func<Response, ResponseType, ReturnType> returnTypeCreator)
        {
            if (context.IsFailure())
            {
                if (nullOn404 && context.Response.StatusCode == 404)
                {
                    return default;
                }
                throw ResponseError(context);
            }
            if (typeof(ResponseType) == typeof(VoidType))
            {
                return default;
            }
            ResponseType result = ConvertResponse(context.Response);
            result = contextAdder(result, compatibilityFactory.CreateHttpContext(context.Request, context.Response));
            if (result is ReturnType convertedResult)
            {
                return convertedResult;
            }
            if (returnTypeCreator != null)
            {
                return returnTypeCreator(compatibilityFactory.CreateHttpResponse(context.Response), result);
            }
            throw new InvalidOperationException($"Unable to transform {typeof(ResponseType)} into {typeof(ReturnType)}. ReturnTypeCreator is not provided.");
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

        private ResponseType ConvertResponse(CoreResponse response)
        {
            if (response.RawBody is ResponseType streamResponse)
            {
                return streamResponse;
            }
            if (response.Body is ResponseType stringResponse && AcceptHeader != ContentType.XML)
            {
                return stringResponse;
            }
            return deserializer(response.Body);
        }
    }
}
