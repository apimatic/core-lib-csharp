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
        private readonly StringBuilder queryUrl;
        private Func<dynamic, object> bodySerializer = PrepareBody;
        
        internal dynamic body;
        internal Type bodyType;
        internal readonly Dictionary<string, object> headersParameters = new Dictionary<string, object>();
        internal readonly Dictionary<string, object> templateParameters = new Dictionary<string, object>();
        internal readonly Dictionary<string, object> bodyParameters = new Dictionary<string, object>();
        internal readonly List<KeyValuePair<string, object>> formParameters = new List<KeyValuePair<string, object>>();
        internal readonly Dictionary<string, object> queryParameters = new Dictionary<string, object>();

        internal RequestBuilder(GlobalConfiguration configuration, string serverUrl)
        {
            this.configuration = configuration;
            queryUrl = new StringBuilder(serverUrl);
            authGroup = new AuthGroupBuilder(configuration.AuthManagers);
        }
        
        internal RequestBuilder(RequestBuilder source)
        {
            this.configuration = source.configuration;
            this.retryOption = source.retryOption;
            this.contentTypeAllowed = source.contentTypeAllowed;
            this.httpMethod = source.httpMethod;
            this.queryUrl = new StringBuilder(source.queryUrl.ToString());
            this.AcceptHeader = source.AcceptHeader;
            this.ArraySerialization = source.ArraySerialization;
            this.HasBinaryResponse = source.HasBinaryResponse;
            this.xmlRequest = source.xmlRequest;
            this.bodySerializer = source.bodySerializer;
            this.body = source.body;
            this.bodyType = source.bodyType;

            this.headersParameters = new Dictionary<string, object>(source.headersParameters);
            this.templateParameters = new Dictionary<string, object>(source.templateParameters);
            this.bodyParameters = new Dictionary<string, object>(source.bodyParameters);
            this.formParameters = new List<KeyValuePair<string, object>>(source.formParameters);
            this.queryParameters = new Dictionary<string, object>(source.queryParameters);
            this.authGroup = new AuthGroupBuilder(source.configuration.AuthManagers);
        }
        
        internal static RequestBuilder RequestBuilderWithParameters(RequestBuilder source)
        {
            var requestBuilder = new RequestBuilder(source);
            source.parameters.Validate().Apply(requestBuilder);
            return requestBuilder;
        }
        
        internal ContentType AcceptHeader { get; set; } = ContentType.SCALAR;

        internal ArraySerialization ArraySerialization { get; set; }

        internal bool HasBinaryResponse { get; set; }

        internal string GetQueryUrl()
        {
            var queryBuilder = new StringBuilder(this.queryUrl.ToString());
            foreach (var kvp in templateParameters)
            {
                queryBuilder.Replace($"{{{kvp.Key}}}", CoreHelper.GetTemplateReplacerValue(kvp.Value));
            }
            return queryBuilder.ToString();
        }
        
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
            queryUrl.Append(path);
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

        internal RequestBuilder UpdateParameterByJsonPointer(string pointerString, Func<object, object> setter)
        {
            var index = pointerString.IndexOf('#');
            if (index < 0)
                return this;

            var prefix = pointerString.Substring(0, index);
            var pointer = pointerString.Substring(index + 1);

            switch (prefix)
            {
                case "$request.headers":
                    RequestBuilderExtensions.UpdateRequestParametersByPointer(headersParameters, pointer, setter);
                    break;
                case "$request.path":
                    RequestBuilderExtensions.UpdateRequestParametersByPointer(templateParameters, pointer, setter);
                    break;
                case "$request.query":
                    RequestBuilderExtensions.UpdateRequestParametersByPointer(queryParameters, pointer, setter);
                    break;
                case "$request.body":
                    UpdateBodyByPointer(setter, pointer);
                    break;
            }

            return this;
        }

        private void UpdateBodyByPointer(Func<object, object> setter, string pointer)
        {
            if (bodyParameters.Any())
            {
                RequestBuilderExtensions.UpdateRequestParametersByPointer(bodyParameters, pointer, setter);
                return;
            }

            if (body is null)
            {
                RequestBuilderExtensions.UpdateFormParameterValueByPointer(formParameters, pointer, setter);
                return;
            }

            UpdateDynamicBodyContent(setter, pointer);
        }
        
        private void UpdateDynamicBodyContent(Func<object, object> setter, string pointer)
        {
            if (body is CoreFileStreamInfo || setter == null)
                return;

            if (body is string || string.IsNullOrEmpty(pointer))
            {
                body = setter(body);
                return;
            }

            body = RequestBuilderExtensions.UpdateValueByPointer(body, pointer, setter);
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
            var queryBuilder = new StringBuilder(GetQueryUrl());
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, queryParameters, ArraySerialization);
            body = bodyParameters.Any() ? bodyParameters : body;
            AppendContentTypeHeader();
            AppendAcceptHeader();
            return new CoreRequest(httpMethod, CoreHelper.CleanUrl(queryBuilder), headersParameters.ToCoreRequestHeaders(), bodySerializer(body),
                formParameters, queryParameters) { RetryOption = retryOption, HasBinaryResponse = HasBinaryResponse };
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
                headersParameters.Add("content-type", ContentType.XML.GetValue());
                return;
            }
            if (body is Stream || body is byte[])
            {
                headersParameters.Add("content-type", ContentType.BINARY.GetValue());
                return;
            }
            if (CoreHelper.IsScalarType(BodyType))
            {
                headersParameters.Add("content-type", ContentType.SCALAR.GetValue());
                return;
            }
            headersParameters.Add("content-type", ContentType.JSON_UTF8.GetValue());
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
            headersParameters.Add("accept", AcceptHeader.GetValue());
        }

        private bool ContentHeaderKeyRequired(string key)
        {
            if (!contentTypeAllowed)
            {
                return false;
            }
            if (headersParameters.Any(p => p.Key.EqualsIgnoreCase(key)))
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
    }
}
