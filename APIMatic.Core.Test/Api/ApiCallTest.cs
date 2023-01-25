using System;
using System.Collections.Generic;
using APIMatic.Core.Response;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Http;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Test.MockTypes.Utilities;

namespace APIMatic.Core.Test.Api
{
    public class ApiCallTest : TestBase
    {
        private static readonly MockServer _server = MockServer.Server1;
        private static readonly CompatibilityFactory _compatibilityFactory = new();
        private static readonly Dictionary<string, ErrorCase<HttpRequest, HttpResponse, HttpContext, ApiException>> _globalErrors = new()
        {
            { "400", CreateErrorCase("400 Global Child 1", (reason, context) => new Child1Exception(reason, context)) },
            { "402", CreateErrorCase("402 Global Child 2", (reason, context)  => new Child2Exception(reason, context)) },
            { "403", CreateErrorCase("403 Global", (reason, context)  => new ApiException(reason, context)) },
            { "404", CreateErrorCase("404 Global", (reason, context)  => new ApiException(reason, context)) },
            { "0", CreateErrorCase("Default Global", (reason, context)  => new ApiException(reason, context)) },
        };

        protected static ErrorCase<HttpRequest, HttpResponse, HttpContext, ApiException> CreateErrorCase(string reason, Func<string, HttpContext, ApiException> error, bool isErrorTemplate = false)
            => new(reason, error, isErrorTemplate);

        protected ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, ApiResponse<T>, T> CreateApiCall<T>()
            => new ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, ApiResponse<T>, T>(
                LazyGlobalConfiguration.Value,
                _compatibilityFactory,
                globalErrors: _globalErrors,
                returnTypeCreator: (response, result) => new ApiResponse<T>(response.StatusCode, response.Headers, result)
            ).Server(_server);

        protected ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, ApiResponse<T>, T> CreateApiCallWithoutReturnTypeCreator<T>()
            => new ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, ApiResponse<T>, T>(
                LazyGlobalConfiguration.Value,
                _compatibilityFactory
            ).Server(_server);

        protected ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, T, T> CreateSimpleApiCall<T>()
            => new ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, T, T>(
                LazyGlobalConfiguration.Value,
                _compatibilityFactory,
                globalErrors: _globalErrors
            ).Server(_server);

        protected ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, T, T> CreateSimpleApiCallWithoutErrors<T>()
            => new ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, T, T>(
                LazyGlobalConfiguration.Value,
                _compatibilityFactory
            ).Server(_server);

        protected string GetCompleteUrl(string path)
        {
            return $"{LazyGlobalConfiguration.Value.ServerUrl()}{path}";
        }
    }
}
