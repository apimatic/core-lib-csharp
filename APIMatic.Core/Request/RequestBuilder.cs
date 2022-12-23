// <copyright file="RequestBuilder.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using APIMatic.Core.Authentication;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Request.Parameters;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Request
{
    /// <summary>
    /// Used to instantiate a new Request object with the provided properties
    /// </summary>
    public class RequestBuilder
    {
        private readonly GlobalConfiguration configuration;
        private RetryOption retryOption = RetryOption.Default;
        private string authName = "";
        private HttpMethod httpMethod;
        private readonly Parameter.Builder parameters = new Parameter.Builder();
        private bool contentTypeAllowed = true;
        private bool xmlRequest = false;
        private Func<dynamic, object> bodySerializer = PrepareBody;

        internal readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        internal dynamic body;
        internal Type bodyType;
        internal readonly Dictionary<string, object> bodyParameters = new Dictionary<string, object>();
        internal readonly List<KeyValuePair<string, object>> formParameters = new List<KeyValuePair<string, object>>();
        internal readonly Dictionary<string, object> queryParameters = new Dictionary<string, object>();

        internal RequestBuilder(GlobalConfiguration configuration) => this.configuration = configuration;
        internal StringBuilder QueryUrl { get; } = new StringBuilder();

        internal ContentType AcceptHeader { get; set; } = ContentType.SCALAR;

        internal ArraySerialization ArraySerialization { get; set; }

        internal bool HasBinaryResponse { get; set; }

        private Type BodyType { get => bodyParameters.Any() ? typeof(object) : bodyType; }

        /// <summary>
        /// Required: Sets the API route and http method
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public RequestBuilder Setup(HttpMethod httpMethod, string path)
        {
            this.httpMethod = httpMethod;
            QueryUrl.Append(path);
            return this;
        }

        /// <summary>
        /// This disables the Accept and Content-Type headers from request
        /// </summary>
        /// <returns></returns>
        public RequestBuilder DisableContentType()
        {
            contentTypeAllowed = false;
            return this;
        }

        /// <summary>
        /// This configures the retry option for the request
        /// </summary>
        /// <param name="retryOption"></param>
        /// <returns></returns>
        public RequestBuilder WithRetryOption(RetryOption retryOption)
        {
            this.retryOption = retryOption;
            return this;
        }

        /// <summary>
        /// This finds and apply the Authentication on to the given request
        /// </summary>
        /// <param name="authName"></param>
        /// <returns></returns>
        public RequestBuilder WithAuth(string authName)
        {
            this.authName = authName;
            return this;
        }

        /// <summary>
        /// Sets the request parameters using <see cref="Parameter.Builder"/>
        /// </summary>
        /// <param name="_params">The action to be applied on Parameter.Builder for this requestBuilder</param>
        /// <returns></returns>
        public RequestBuilder Parameters(Action<Parameter.Builder> _params)
        {
            _params(parameters);
            return this;
        }

        /// <summary>
        /// This converts the request body into XML format
        /// </summary>
        /// <param name="xmlSerializer"></param>
        /// <returns></returns>
        public RequestBuilder XmlBodySerializer(Func<dynamic, object> xmlSerializer)
        {
            xmlRequest = true;
            bodySerializer = xmlSerializer;
            return this;
        }

        /// <summary>
        /// This applies all the configuration and build an instance of CoreRequest
        /// </summary>
        /// <returns></returns>
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
            return new CoreRequest(httpMethod, CoreHelper.CleanUrl(QueryUrl), headers, bodySerializer(body), formParameters, queryParameters)
            {
                RetryOption = retryOption,
                HasBinaryResponse = HasBinaryResponse
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
            if (xmlRequest)
            {
                headers.Add("content-type", ContentType.XML.GetValue());
                return;
            }
            if (body is Stream || body is byte[])
            {
                headers.Add("content-type", ContentType.BINARY.GetValue());
                return;
            }
            if (CoreHelper.IsScalarType(BodyType))
            {
                headers.Add("content-type", ContentType.SCALAR.GetValue());
                return;
            }
            headers.Add("content-type", ContentType.JSON_UTF8.GetValue());
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
            if (headers.Any(p => p.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }
            return true;
        }

        private static object PrepareBody(dynamic value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is CoreFileStreamInfo || value is Stream || value is byte[] || CoreHelper.IsScalarType(value.GetType()))
            {
                return value;
            }
            return CoreHelper.JsonSerialize(value);
        }
    }
}
