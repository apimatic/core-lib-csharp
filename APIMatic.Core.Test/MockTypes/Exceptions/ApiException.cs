// <copyright file="ApiException.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Test.MockTypes.Http;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Types.Sdk;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Test.MockTypes.Exceptions
{
    /// <summary>
    /// This is the base class for all exceptions that represent an error response
    /// from the server.
    /// </summary>
    public class ApiException : CoreApiException<HttpRequest, HttpResponse, HttpContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiException"/> class.
        /// </summary>
        /// <param name="reason"> The reason for throwing exception.</param>
        /// <param name="context"> The HTTP context that encapsulates request and response objects.</param>
        public ApiException(string reason, HttpContext context = null) : base(reason, context) { }

        protected override void SetupAdditionalProperties(string responseBody)
        {
            base.SetupAdditionalProperties(responseBody);
            JObject body = JObject.Parse(responseBody);

            if (body.ContainsKey("data"))
            {
                ExceptionData = body.GetValue("data").ToObject<object>();
            }
        }

        public object ExceptionData { get; private set; }
    }
}
