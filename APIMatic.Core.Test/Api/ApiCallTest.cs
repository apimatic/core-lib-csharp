using System;
using NUnit.Framework;
using Tester.Standard.Utilities;
using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Http;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Request;
using APIMatic.Core.Response;
using Polly;
using System.Net.Http;

namespace APIMatic.Core.Test.Api
{
    [TestFixture]
    public class ApiCallTest : TestBase
    {
        private static MockServer _server = MockServer.Server1;
        private static readonly CompatibilityFactory _compatibilityFactory = new CompatibilityFactory();
        private static readonly Dictionary<int, Func<HttpContext, ApiException>> _globalErrors = new Dictionary<int, Func<HttpContext, ApiException>>
        {
            { 400, context => new ApiException("400 Global", context) },
            { 402, context => new ApiException("402 Global", context) },
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

        protected string GetCompleteUrl(string path)
        {
            return $"{LazyGlobalConfiguration.Value.ServerUrl()}{path}";
        }
    }
}
