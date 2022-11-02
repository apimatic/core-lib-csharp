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

namespace APIMatic.Core.Request
{
    public class RequestBuilder
    {
        private readonly GlobalConfiguration configuration;
        private RetryOption retryOption = RetryOption.Default;
        private bool contentTypeAllowed = true;
        private string authName = "";
        private HttpMethod httpMethod;
        private readonly Parameter.Builder parameters = new Parameter.Builder();

        internal readonly StringBuilder queryUrl = new StringBuilder();
        internal readonly Dictionary<string, string> headers = new Dictionary<string, string>();
        internal object body;
        internal readonly Dictionary<string, object> bodyParameters = new Dictionary<string, object>();
        internal readonly List<KeyValuePair<string, object>> formParameters = new List<KeyValuePair<string, object>>();
        internal readonly Dictionary<string, object> queryParameters = new Dictionary<string, object>();

        internal RequestBuilder(GlobalConfiguration configuration) => this.configuration = configuration;

        internal RequestBuilder ServerUrl(string serverUrl)
        {
            queryUrl.Append(serverUrl);
            return this;
        }

        public RequestBuilder Setup(HttpMethod httpMethod, string path)
        {
            this.httpMethod = httpMethod;
            queryUrl.Append(path);
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

        public CoreRequest Build()
        {
            parameters.Validate().Apply(this);
            configuration.GlobalRuntimeParameters.Validate().Apply(this);
            bool authManagerFound = configuration.AuthManagers.TryGetValue(authName, out var authManager);
            if (authManagerFound)
            {
                authManager.Validate().Apply(this);
            }
            return new CoreRequest(httpMethod, queryUrl.ToString(), headers, body, formParameters, queryParameters)
            {
                RetryOption = retryOption
            };
        }
    }
}
