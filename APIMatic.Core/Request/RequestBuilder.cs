// <copyright file="RequestBuilder.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Authentication;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Request.Parameters;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;
using Microsoft.Json.Pointer;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Request
{
    /// <summary>
    /// Used to instantiate a new Request object with the provided properties
    /// </summary>
    public class RequestBuilder
    {
        private readonly GlobalConfiguration configuration;
        private readonly AuthGroupBuilder authGroup;
        private RetryOption retryOption = RetryOption.Default;
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

        internal RequestBuilder(GlobalConfiguration configuration)
        {
            this.configuration = configuration;
            authGroup = new AuthGroupBuilder(configuration.AuthManagers);
        }

        internal StringBuilder QueryUrl { get; } = new StringBuilder();

        internal ContentType AcceptHeader { get; set; } = ContentType.SCALAR;

        internal ArraySerialization ArraySerialization { get; set; }

        internal bool HasBinaryResponse { get; set; }

        private Type BodyType => bodyParameters.Any() ? typeof(object) : bodyType;

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
            return WithAndAuth(auth => auth.Add(authName));
        }

        /// <summary>
        /// This takes in a action to be apply on the OR authentication group
        /// </summary>
        /// <param name="authName"></param>
        /// <returns></returns>
        public RequestBuilder WithOrAuth(Action<AuthGroupBuilder> applyAuth)
        {
            applyAuth(authGroup.ToOrGroup());
            return this;
        }

        /// <summary>
        /// This takes in a action to be apply on the AND authentication group
        /// </summary>
        /// <param name="authName"></param>
        /// <returns></returns>
        public RequestBuilder WithAndAuth(Action<AuthGroupBuilder> applyAuth)
        {
            applyAuth(authGroup.ToAndGroup());
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

        public RequestBuilder UpdateByReference(string pointerString, Func<object, object> setter)
        {
            if (pointerString == null)
                return this;

            var parts = pointerString.Split('#');

            if (parts.Length <= 1)
                return this;

            var jsonPointer = new JsonPointer(parts[1]);

            switch (parts[0])
            {
                case "$request.path": case "$request.query": case "$request.headers":
                    parameters.UpdateParameterValueByPointer(setter, jsonPointer);
                    parameters.Apply(this);
                    return this;
                default:
                    return this;
            }
        }

        /// <summary>
        /// This applies all the configuration and build an instance of CoreRequest
        /// </summary>
        /// <returns></returns>
        public async Task<CoreRequest> Build()
        {
            parameters.Validate().Apply(this);
            configuration.RuntimeParameters.Validate().Apply(this);
            await authGroup.Apply(this).ConfigureAwait(false);
            var queryUrl = new StringBuilder(QueryUrl.ToString());
            CoreHelper.AppendUrlWithQueryParameters(queryUrl, queryParameters, ArraySerialization);
            body = bodyParameters.Any() ? bodyParameters : body;
            AppendContentTypeHeader();
            AppendAcceptHeader();
            return new CoreRequest(httpMethod, CoreHelper.CleanUrl(queryUrl), headers, bodySerializer(body), formParameters, queryParameters)
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
            if (headers.Any(p => p.Key.EqualsIgnoreCase(key)))
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
            if (CoreHelper.TryGetInnerValueForContainer(value, out dynamic innerValue) && innerValue is string)
            {
                return innerValue;
            }
            return CoreHelper.JsonSerialize(value);
        }

        public RequestBuilder Clone()
        {
            var clone = new RequestBuilder(configuration)
                .WithRetryOption(retryOption)
                .DisableContentType();
            clone.contentTypeAllowed = contentTypeAllowed;
            clone.httpMethod = this.httpMethod;
            clone.QueryUrl.Append(this.QueryUrl);
            clone.AcceptHeader = this.AcceptHeader;
            clone.ArraySerialization = this.ArraySerialization;
            clone.HasBinaryResponse = this.HasBinaryResponse;
            clone.xmlRequest = this.xmlRequest;
            clone.bodySerializer = this.bodySerializer;
            clone.body = this.body;
            clone.bodyType = this.bodyType;

            foreach (var kvp in this.headers)
            {
                clone.headers.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in this.bodyParameters)
            {
                clone.bodyParameters.Add(kvp.Key, kvp.Value);
            }

            foreach (var kvp in this.formParameters)
            {
                clone.formParameters.Add(new KeyValuePair<string, object>(kvp.Key, kvp.Value));
            }

            foreach (var kvp in this.queryParameters)
            {
                clone.queryParameters.Add(kvp.Key, kvp.Value);
            }

            // Clone parameters builder
            this.parameters.Clone(clone.parameters);

            // Clone authGroup
            this.authGroup.Clone(clone.authGroup);

            return clone;
        }
    }
}
