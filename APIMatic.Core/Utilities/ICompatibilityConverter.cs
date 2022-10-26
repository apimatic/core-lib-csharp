// <copyright file="ICompatibilityConverter.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Types;

namespace APIMatic.Core.Utilities
{
    public interface ICompatibilityConverter<Request, Response, Context>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context: CoreContext<Request, Response>
    {
        Request CreateHttpRequest(CoreRequest request);
        Response CreateHttpResponse(CoreResponse response);
        Context CreateHttpContext(CoreRequest request, CoreResponse response);
    }
}
