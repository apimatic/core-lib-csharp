// <copyright file="ICompatibilityFactory.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Utilities
{
    /// <summary>
    /// Provides compatibile conversion from parent to child classes
    /// </summary>
    /// <typeparam name="Request"> Class Type that holds http request info </typeparam>
    /// <typeparam name="Response"> Class Type that holds http response info </typeparam>
    /// <typeparam name="Context"> Class Type that holds http context info </typeparam>
    /// <typeparam name="ApiException"> Class Type that holds BaseException info </typeparam>
    public interface ICompatibilityFactory<Request, Response, Context, ApiException>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context: CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        ApiException CreateApiException(string reason, CoreContext<CoreRequest, CoreResponse> context);
        Request CreateHttpRequest(CoreRequest request);
        Response CreateHttpResponse(CoreResponse response);
        Context CreateHttpContext(CoreRequest request, CoreResponse response);
    }
}
