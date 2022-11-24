﻿// <copyright file="CompatibilityFactory.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Http;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace Tester.Standard.Utilities
{
    internal class CompatibilityFactory : ICompatibilityFactory<HttpRequest, HttpResponse, HttpContext, ApiException>
    {
        public ApiException CreateApiException(string reason, CoreContext<CoreRequest, CoreResponse> context) =>
            new ApiException(reason, CreateHttpContext(context.Request, context.Response));

        public HttpContext CreateHttpContext(CoreRequest request, CoreResponse response) =>
            new HttpContext(CreateHttpRequest(request), CreateHttpResponse(response));

        public HttpRequest CreateHttpRequest(CoreRequest request) =>
            new HttpRequest(request.HttpMethod, request.QueryUrl, request.Headers, request.Body,
                request.FormParameters, request.Username, request.Password, request.QueryParameters);

        public HttpResponse CreateHttpResponse(CoreResponse response) =>
            new HttpResponse(response.StatusCode, response.Headers, response.RawBody);
    }
}