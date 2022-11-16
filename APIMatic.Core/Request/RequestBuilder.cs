// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using APIMatic.Core.Http.Client.Configuration;
using APIMatic.Core.Request.Parameters;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;
using System.Linq;
using APIMatic.Core.Authentication;

namespace APIMatic.Core.Request
{
    public class RequestBuilder
    {
        private readonly GlobalConfiguration configuration;
        private RetryOption retryOption = RetryOption.Default;
        private string authName = "";
        private HttpMethod httpMethod;
        private readonly Parameter.Builder parameters = new Parameter.Builder();
        private bool contentTypeAllowed = true;
        private ContentType contentType;
        private Func<dynamic, string> bodySerializer = value => CoreHelper.IsScalarType(value?.GetType()) ? value?.ToString() : CoreHelper.JsonSerialize(value);

        internal readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        internal dynamic body;
        internal readonly Dictionary<string, object> bodyParameters = new Dictionary<string, object>();
        internal readonly List<KeyValuePair<string, object>> formParameters = new List<KeyValuePair<string, object>>();
        internal readonly Dictionary<string, object> queryParameters = new Dictionary<string, object>();

        internal RequestBuilder(GlobalConfiguration configuration) => this.configuration = configuration;
        internal StringBuilder QueryUrl { get; } = new StringBuilder();

        internal ContentType AcceptHeader { get; set; } = ContentType.SCALAR;

        internal ArraySerialization ArraySerialization { get; set; }

        internal bool HasBinaryResponse { get; set; }

        private string SerializedBody { get => bodySerializer(body); }

        public RequestBuilder Setup(HttpMethod httpMethod, string path)
        {
            this.httpMethod = httpMethod;
            QueryUrl.Append(path);
            return this;
        }

        public RequestBuilder DisableContentType()
        {
            contentTypeAllowed = false;
            return this;
        }

        public RequestBuilder WithRetryOption(RetryOption retryOption)
        {
            this.retryOption = retryOption;
            return this;
        }

        public RequestBuilder WithAuth(string authName)
        {
            this.authName = authName;
            return this;
        }

        /// <summary>
        /// Sets the request parameters using <see cref="Parameter.Builder"/>
        /// </summary>
        /// <param name="action">The action to be applied on Parameter.Builder for this requestBuilder</param>
        /// <returns></returns>
        public RequestBuilder Parameters(Action<Parameter.Builder> action)
        {
            action(parameters);
            return this;
        }

        public RequestBuilder XmlBodySerializer(Func<dynamic, string> xmlSerializer)
        {
            contentType = ContentType.XML;
            bodySerializer = xmlSerializer;
            return this;
        }

        public CoreRequest Build()
        {
            parameters.Validate().Apply(this);
            configuration.RuntimeParameters.Validate().Apply(this);
            if (configuration.AuthManagers.TryGetValue(authName, out AuthManager authManager))
            {
                authManager.Apply(this);
            }
            CoreHelper.AppendUrlWithQueryParameters(QueryUrl, queryParameters, ArraySerialization);
            body = bodyParameters.Any() ? bodyParameters : body;
            AppendContentTypeHeader();
            AppendAcceptHeader();
            return new CoreRequest(httpMethod, CoreHelper.CleanUrl(QueryUrl), headers, SerializedBody, formParameters, queryParameters)
            {
                RetryOption = retryOption
            };
        }

        private void AppendContentTypeHeader()
        {
            if (!ContentHeaderKeyRequired("content-type"))
            {
                return;
            }
            if (body is CoreFileStreamInfo)
            {
                return;
            }
            if (contentType == ContentType.XML)
            {
                headers.Add("content-type", contentType.GetValue());
                return;
            }
            if (CoreHelper.IsScalarType(body?.GetType()))
            {
                headers.Add("content-type", ContentType.SCALAR.GetValue());
                return;
            }
            headers.Add("content-type", ContentType.JSON.GetValue());
        }

        private void AppendAcceptHeader()
        {
            if (!ContentHeaderKeyRequired("accept"))
            {
                return;
            }
            if (AcceptHeader == ContentType.SCALAR)
            {
                return;
            }
            headers.Add("accept", AcceptHeader.GetValue());
        }

        private bool ContentHeaderKeyRequired(string key)
        {
            if (!contentTypeAllowed)
            {
                return false;
            }
            if (headers.Where(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)).Any())
            {
                return false;
            }
            return true;
        }
    }
}
