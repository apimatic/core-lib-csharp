using System;
using NUnit.Framework;
using Tester.Standard.Utilities;
using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Http;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Test.MockTypes.Http.Response;

namespace APIMatic.Core.Test.Api
{
    [TestFixture]
    public class ApiCallTest : TestBase
    {
        private static readonly CompatibilityFactory compatibilityFactory = new CompatibilityFactory();
        private static readonly Dictionary<int, Func<HttpContext, ApiException>> globalErrors = new Dictionary<int, Func<HttpContext, ApiException>>
        {
            { 400, context => new ApiException("400 Global", context) },
            { 402, context => new ApiException("402 Global", context) },
            { 403, context => new ApiException("403 Global", context) },
            { 404, context => new ApiException("404 Global", context) }
        };

        protected ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, Ret, Inner> CreateApiCall<Ret, Inner>()
            => new ApiCall<HttpRequest, HttpResponse, HttpContext, ApiException, Ret, Inner>(
                LazyGlobalConfiguration.Value,
                compatibilityFactory,
                errors: globalErrors
            );
    }
}
