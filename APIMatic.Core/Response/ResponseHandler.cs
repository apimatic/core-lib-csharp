// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Response
{
    public class ResponseHandler<Request, Response, Context, ApiException, ReturnType>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly Dictionary<int, Func<Context, ApiException>> errors;
        private readonly ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory;

        internal ResponseHandler(ICompatibilityFactory<Request, Response, Context, ApiException> compatibilityFactory,
            Dictionary<int, Func<Context, ApiException>> errors) => (this.compatibilityFactory, this.errors) = (compatibilityFactory, errors);

        internal ReturnType getResult(CoreContext<CoreRequest, CoreResponse> context)
        {
            return default;
        }
    }
}
