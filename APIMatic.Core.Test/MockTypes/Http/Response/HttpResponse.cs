// <copyright file="HttpResponse.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using System.IO;
using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Test.MockTypes.Http.Response
{
    /// <summary>
    /// HttpResponse stores necessary information about the http response.
    /// </summary>
    public class HttpResponse : CoreResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class.
        /// </summary>
        /// <param name="statusCode">statusCode.</param>
        /// <param name="headers">headers.</param>
        /// <param name="rawBody">rawBody.</param>
        public HttpResponse(int statusCode, Dictionary<string, string> headers, Stream rawBody, string body = null)
            : base(statusCode, headers, rawBody, body) { }
    }
}
