// <copyright file="CoreApiResponse.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Types.Sdk
{
    /// <summary>
    /// This is the base class for all exceptions that represent an error response
    /// from the server.
    /// </summary>
    [JsonObject]
    public class CoreApiException<Request, Response, Context> : Exception
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreApiException{Request, Response, Context}"/> class.
        /// </summary>
        /// <param name="statusCode">Status code.</param>
        /// <param name="headers">Headers.</param>
        /// <param name="data">Data.</param>
        public CoreApiException(string reason, Context context) : base(reason)
        {
            HttpContext = context;
            // If a derived exception class is used, then perform deserialization of response body.
            if ((context == null) || (context.Response == null)
                || (context.Response.RawBody == null)
                || (!context.Response.RawBody.CanRead))
            {
                return;
            }

            using (var reader = new StreamReader(context.Response.RawBody))
            {
                string responseBody = reader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(responseBody))
                {
                    try
                    {
                        JObject body = JObject.Parse(responseBody);

                        if (!GetType().Name.Equals("ApiException", StringComparison.OrdinalIgnoreCase))
                        {
                            JsonConvert.PopulateObject(responseBody, this);
                        }
                        SetupAdditionalProperties(body);
                    }
                    catch (JsonReaderException)
                    {
                        // Ignore deserialization and IO issues to prevent exception being thrown when this exception
                        // instance is being constructed.
                    }
                }
            }
        }

        protected virtual void SetupAdditionalProperties(JObject body) { }

        /// <summary>
        /// Gets the HTTP response code from the API request.
        /// </summary>
        [JsonIgnore]
        public int ResponseCode
        {
            get { return HttpContext != null ? HttpContext.Response.StatusCode : -1; }
        }

        /// <summary>
        /// Gets or sets the HttpContext for the request and response.
        /// </summary>
        [JsonIgnore]
        public Context HttpContext { get; private set; }
    }
}
