// <copyright file="ResponseHandler.cs" company="APIMatic">
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
    /// <summary>
    /// Used to handle and process the response received from HttpClient
    /// </summary>
    /// <typeparam name="Request"> Class Type that holds http request info </typeparam>
    /// <typeparam name="Response"> Class Type that holds http response info </typeparam>
    /// <typeparam name="Context"> Class Type that holds http context info </typeparam>
    /// <typeparam name="ApiException"> Class Type that holds BaseException info </typeparam>
    /// <typeparam name="ResponseType"> Expected type of http response </typeparam>
    public class ResponseHandler<Request, Response, Context, ApiException, ResponseType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly Dictionary<string, ErrorCase<Request, Response, Context, ApiException>> globalErrors;
        private readonly Dictionary<string, ErrorCase<Request, Response, Context, ApiException>> localErrors;
        private readonly ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory;
        private bool nullOn404 = false;
        private Func<string, ResponseType> deserializer = responseBody => CoreHelper.JsonDeserialize<ResponseType>(responseBody);
        private Func<ResponseType, Context, ResponseType> contextAdder = (result, context) => result;

        internal ContentType AcceptHeader { get; set; } = ContentType.JSON;

        internal ResponseHandler(ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory,
            Dictionary<string, ErrorCase<Request, Response, Context, ApiException>> globalErrors)
        {
            this.compatibilityFactory = compatibilityFactory;
            this.globalErrors = globalErrors ?? new Dictionary<string, ErrorCase<Request, Response, Context, ApiException>>();
            localErrors = new Dictionary<string, ErrorCase<Request, Response, Context, ApiException>>();

            if (CoreHelper.IsScalarType(typeof(ResponseType)))
            {
                AcceptHeader = ContentType.SCALAR;
            }
        }

        /// <summary>
        /// This adds an case for throwing error, for a particular error code
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> ErrorCase(
            string statusCode, ErrorCase<Request, Response, Context, ApiException> errorCase)
        {
            localErrors[statusCode] = errorCase;
            return this;
        }

        /// <summary>
        /// This controls returning null on error code 404
        /// </summary>
        /// <returns></returns>
        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> NullOn404()
        {
            nullOn404 = true;
            return this;
        }

        /// <summary>
        /// This controls converting response into xml response
        /// </summary>
        /// <returns></returns>
        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> XmlResponse()
        {
            AcceptHeader = ContentType.XML;
            return this;
        }

        /// <summary>
        /// Used to Deserialize the body of the received http response
        /// </summary>
        /// <param name="deserializer"></param>
        /// <returns></returns>
        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> Deserializer(
            Func<string, ResponseType> deserializer)
        {
            this.deserializer = deserializer;
            return this;
        }

        /// <summary>
        /// Updates the actual http response with http context using the provided function
        /// </summary>
        /// <param name="contextAdder"></param>
        /// <returns></returns>
        public ResponseHandler<Request, Response, Context, ApiException, ResponseType> ContextAdder(
            Func<ResponseType, Context, ResponseType> contextAdder)
        {
            this.contextAdder = contextAdder;
            return this;
        }

        /// <summary>
        /// This applies all the configurations, processes the http response and returns expected response
        /// </summary>
        /// <typeparam name="ReturnType"> Real expected type from the API endpoint </typeparam>
        /// <param name="context"></param>
        /// <param name="returnTypeCreator"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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
            if (CoreHelper.IsNullableType(typeof(ResponseType)) && IsBodyMissing(context.Response))
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

        private bool IsBodyMissing(CoreResponse response) => string.Equals(response.Body, string.Empty);

        private ApiException ResponseError(CoreContext<CoreRequest, CoreResponse> context)
        {
            Context httpContext = compatibilityFactory.CreateHttpContext(context.Request, context.Response);
            var statusCode = context.Response.StatusCode.ToString();
            if (GetStatusCodeErrorCase(localErrors, statusCode, out var localErrorCase))
            {
                return localErrorCase.GetError(httpContext);
            }
            if (GetStatusCodeErrorCase(globalErrors, statusCode, out var globalErrorCase))
            {
                return globalErrorCase.GetError(httpContext);
            }
            if (GetDefaultErrorCaseFromErrors(out var defaultErrorCase))
            {
                return defaultErrorCase.GetError(httpContext);
            }
            return compatibilityFactory.CreateApiException("HTTP Response Not OK", context);
        }

        private bool GetStatusCodeErrorCase(Dictionary<string, ErrorCase<Request, Response, Context, ApiException>> errors, string statusCode, out ErrorCase<Request, Response, Context, ApiException> errorCase)
        {
            if (errors.TryGetValue(statusCode, out errorCase))
            {
                return true;
            }
            if (GetRangedStatusCodeErrorCase(errors, statusCode, out errorCase))
            {
                return true;
            }
            return false;
        }

        private bool GetRangedStatusCodeErrorCase(Dictionary<string, ErrorCase<Request, Response, Context, ApiException>> errors, string statusCode, out ErrorCase<Request, Response, Context, ApiException> errorCase)
        {
            errorCase = default;
            if (statusCode.StartsWith("4") || statusCode.StartsWith("5"))
            {
                var rangedStatusCode = $"{statusCode[0]}XX";
                return errors.TryGetValue(rangedStatusCode, out errorCase);
            }
            return false;
        }

        private bool GetDefaultErrorCaseFromErrors(out ErrorCase<Request, Response, Context, ApiException> errorCase)
        {
            if (localErrors.TryGetValue("0", out errorCase))
            {
                return true;
            }
            if (globalErrors.TryGetValue("0", out errorCase))
            {
                return true;
            }
            return false;
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
