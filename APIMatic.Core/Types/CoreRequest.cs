// <copyright file="CoreRequest.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace APIMatic.Core.Types
{
    /// <summary>
    /// CoreRequest.
    /// </summary>
    public class CoreRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreRequest"/> class.
        /// </summary>
        /// <param name="method">Http verb to use for the http request.</param>
        /// <param name="queryUrl">The query url for the http request.</param>
        /// <param name="headers">Headers to send with the request.</param>
        /// <param name="body">The string to use as raw body of the http request.</param>
        /// <param name="formParameters">The form parameters to be sent in the http request.</param>
        /// <param name="username">Basic auth username.</param>
        /// <param name="password">Basic auth password.</param>
        /// <param name="queryParameters">QueryParameters.</param>
        public CoreRequest(HttpMethod method, string queryUrl, Dictionary<string, string> headers, object body,
            List<KeyValuePair<string, object>> formParameters, Dictionary<string, object> queryParameters = null,
             string username = null, string password = null) =>
            (HttpMethod, QueryUrl, Headers, Body, FormParameters, QueryParameters, Username, Password) =
            (method, queryUrl, headers, body, formParameters, queryParameters, username, password);

        /// <summary>
        /// Gets the HTTP verb to use for this request.
        /// </summary>
        public HttpMethod HttpMethod { get; }

        /// <summary>
        /// Gets the query url for the http request.
        /// </summary>
        public string QueryUrl { get; }

        /// <summary>
        /// Gets the query parameters collection for the current http request.
        /// </summary>
        public Dictionary<string, object> QueryParameters { get; private set; }

        /// <summary>
        /// Gets the headers collection for the current http request.
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Gets the form parameters for the current http request.
        /// </summary>
        public List<KeyValuePair<string, object>> FormParameters { get; }

        /// <summary>
        /// Gets the optional raw string to send as request body.
        /// </summary>
        public object Body { get; }

        /// <summary>
        /// Gets the optional username for Basic Auth.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the optional password for Basic Auth.
        /// </summary>
        public string Password { get; }

        /// <summary>
        /// Concatenate values from a Dictionary to this object.
        /// </summary>
        /// <param name="headersToAdd"> headersToAdd. </param>
        /// <returns>Dictionary.</returns>
        public Dictionary<string, string> AddHeaders(Dictionary<string, string> headersToAdd)
        {
            Headers = Headers?.Concat(headersToAdd).ToDictionary(x => x.Key, x => x.Value)
                ?? new Dictionary<string, string>(headersToAdd);
            return Headers;
        }

        /// <summary>
        /// Concatenate values from a Dictionary to query parameters dictionary.
        /// </summary>
        /// <param name="queryParamaters"> queryParamaters. </param>
        public void AddQueryParameters(Dictionary<string, object> queryParamaters)
        {
            QueryParameters = QueryParameters?.Concat(queryParamaters).ToDictionary(x => x.Key, x => x.Value)
                ?? new Dictionary<string, object>(queryParamaters);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $" HttpMethod = {HttpMethod}, " +
                $" QueryUrl = {QueryUrl}, " +
                $" QueryParameters = {CoreHelper.JsonSerialize(QueryParameters)}, " +
                $" Headers = {CoreHelper.JsonSerialize(Headers)}, " +
                $" FormParameters = {CoreHelper.JsonSerialize(FormParameters)}, " +
                $" Body = {Body}, " +
                $" Username = {Username}, " +
                $" Password = {Password}";
        }
    }
}
