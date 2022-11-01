// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Http.Client.Configuration;
using APIMatic.Core.Types.Sdk;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace APIMatic.Core.Request
{
    public class RequestBuilder
    {
        private readonly GlobalConfiguration configuration;
        private RetryOption retryOption = RetryOption.Default;
        private HttpMethod httpMethod;
        internal StringBuilder serverUrl = new StringBuilder();
        internal Dictionary<string, string> headers;
        internal object body;
        internal List<KeyValuePair<string, object>> formParameters;
        internal Dictionary<string, object> queryParameters;

        internal RequestBuilder(GlobalConfiguration configuration) => this.configuration = configuration;

        internal RequestBuilder ServerUrl(string serverUrl)
        {
            this.serverUrl.Append(serverUrl);
            return this;
        }

        public RequestBuilder Init(HttpMethod httpMethod, string path)
        {
            this.httpMethod = httpMethod;
            serverUrl.Append(path);
            return this;
        }

        public RequestBuilder WithRetryOption(RetryOption retryOption)
        {
            this.retryOption = retryOption;
            return this;
        }

        public CoreRequest Build()
        {
            configuration.GlobalRuntimeParameters.ForEach(param => param.Apply(this));

            return new CoreRequest(httpMethod, serverUrl.ToString(), headers, body, formParameters, queryParameters)
            {
                RetryOption = retryOption
            };
        }
    }
}
