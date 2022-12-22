using System;
using Tester.Standard.Utilities;
using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Http;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.Api
{
    public class ApiCallTest : TestBase
    {
        private static MockServer _server = MockServer.Server1;
        private static readonly CompatibilityFactory _compatibilityFactory = new CompatibilityFactory();
        private static readonly Dictionary<int, Func<HttpContext, ApiException>> _globalErrors = new Dictionary<int, Func<HttpContext, ApiException>>
        {
            { 400, context => new Child1Exception("400 Global Child 1", context) },
            { 402, context => new Child2Exception("402 Global Child 2", context) },
            { 403, context => new ApiException("403 Global", context) },
            { 404, context => new ApiException("404 Global", context) }
        };

        protected ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, ApiResponse<T>, T> CreateApiCall<T>()
            => new ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, ApiResponse<T>, T>(
                LazyGlobalConfiguration.Value,
                _compatibilityFactory,
                errors: _globalErrors,
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
                errors: _globalErrors
            ).Server(_server);

        protected string GetCompleteUrl(string path)
        {
            return $"{LazyGlobalConfiguration.Value.ServerUrl()}{path}";
        }
    }
}
