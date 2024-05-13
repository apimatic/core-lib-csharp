// <copyright file="CoreApiException.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>

using System;
using System.IO;
using Newtonsoft.Json;

namespace APIMatic.Core.Types.Sdk
{
    /// <summary>
    ///     This is the base class for all exceptions that represent an error response
    ///     from the server.
    /// </summary>
    [JsonObject]
    public class CoreApiException<Request, Response, Context> : Exception
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CoreApiException{Request, Response, Context}" /> class.
        /// </summary>
        public CoreApiException(string reason, Context context) : base(reason)
        {
            HttpContext = context;
            // If a derived exception class is used, then perform deserialization of response body.
            if (context == null || context.Response == null
                                || context.Response.RawBody == null
                                || !context.Response.RawBody.CanRead)
            {
                return;
            }

            var responseBody = new StreamReader(context.Response.RawBody).ReadToEnd();
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return;
            }

            try
            {
                SetupAdditionalProperties(responseBody);
            }
            catch (JsonReaderException)
            {
                // Ignore deserialization and IO issues to prevent exception being thrown when this exception
                // instance is being constructed.
            }
        }

        /// <summary>
        ///     Gets the HTTP response code from the API request.
        /// </summary>
        [JsonIgnore]
        public int ResponseCode => HttpContext?.Response?.StatusCode ?? -1;

        /// <summary>
        ///     Gets or sets the HttpContext for the request and response.
        /// </summary>
        [JsonIgnore]
        public Context HttpContext { get; private set; }

        protected virtual void SetupAdditionalProperties(string responseBody)
        {
            if (!GetType().Name.Equals("ApiException", StringComparison.OrdinalIgnoreCase))
            {
                JsonConvert.PopulateObject(responseBody, this);
            }
        }
    }
}
