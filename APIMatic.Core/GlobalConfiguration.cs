// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Authentication;
using APIMatic.Core.Http;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Request;
using APIMatic.Core.Request.Parameters;
using APIMatic.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace APIMatic.Core
{
    public class GlobalConfiguration
    {
        private readonly Dictionary<Enum, string> _serverUrls;
        private readonly Enum _defaultServer;
        private readonly Parameter.Builder _parameters;

        internal Dictionary<string, AuthManager> AuthManagers { get; private set; }
        internal Parameter.Builder RuntimeParameters { get; private set; }
        internal HttpCallBack ApiCallback { get; private set; }
        internal HttpClientWrapper HttpClient { get; private set; }

        private GlobalConfiguration(ICoreHttpClientConfiguration httpConfiguration, Dictionary<string, AuthManager> authManagers,
            Dictionary<Enum, string> serverUrls, Enum defaultServer, Parameter.Builder parameters,
            Parameter.Builder runtimeParameters, HttpCallBack apiCallback)
        {
            _serverUrls = serverUrls;
            _defaultServer = defaultServer;
            _parameters = parameters;
            ApiCallback = apiCallback;
            AuthManagers = authManagers;
            RuntimeParameters = runtimeParameters;
            HttpClient = new HttpClientWrapper(httpConfiguration);
        }

        public string ServerUrl(Enum server = null) => GlobalRequestBuilder(server).QueryUrl.ToString();

        public RequestBuilder GlobalRequestBuilder(Enum server = null)
        {
            RequestBuilder requestBuilder = new RequestBuilder(this);
            requestBuilder.QueryUrl.Append(_serverUrls[server ?? _defaultServer]);
            _parameters.Validate().Apply(requestBuilder);
            return requestBuilder;
        }

        public class Builder
        {
            private ICoreHttpClientConfiguration httpConfiguration;
            private Dictionary<string, AuthManager> authManagers = new Dictionary<string, AuthManager>();
            private Dictionary<Enum, string> serverUrls = new Dictionary<Enum, string>();
            private Enum defaultServer;
            private readonly Parameter.Builder parameters = new Parameter.Builder();
            private readonly Parameter.Builder runtimeParameters = new Parameter.Builder();
            private HttpCallBack apiCallback;

            public Builder HttpConfiguration(ICoreHttpClientConfiguration httpConfiguration)
            {
                this.httpConfiguration = httpConfiguration;
                return this;
            }

            public Builder AuthManagers(Dictionary<string, AuthManager> authManagers)
            {
                this.authManagers = authManagers;
                return this;
            }

            public Builder ServerUrls(Dictionary<Enum, string> serverUrls, Enum defaultServer)
            {
                this.serverUrls = serverUrls;
                this.defaultServer = defaultServer;
                return this;
            }

            public Builder Parameters(Action<Parameter.Builder> action)
            {
                action(parameters);
                return this;
            }

            public Builder RuntimeParameters(Action<Parameter.Builder> action)
            {
                action(runtimeParameters);
                return this;
            }

            public Builder ApiCallback(HttpCallBack apiCallback)
            {
                this.apiCallback = apiCallback;
                return this;
            }

            public Builder UserAgent(string userAgent, List<(string placeHolder, string value)> userAgentConfig = null)
            {
                if (userAgent == null)
                {
                    return this;
                }
                userAgent = Regex.Replace(userAgent, "{engine}", RuntimeInformation.FrameworkDescription.ToString(), RegexOptions.IgnoreCase);
                userAgent = Regex.Replace(userAgent, "{engine-version}", Environment.Version.ToString(), RegexOptions.IgnoreCase);
                userAgent = Regex.Replace(userAgent, "{os-info}", Environment.OSVersion.ToString(), RegexOptions.IgnoreCase);

                userAgentConfig?.ForEach(c => userAgent = Regex.Replace(userAgent, c.placeHolder, c.value, RegexOptions.IgnoreCase));
                parameters.Header(h => h.Setup("user-agent", userAgent));
                return this;
            }

            public GlobalConfiguration Build() => new GlobalConfiguration(httpConfiguration, authManagers, serverUrls, defaultServer,
                    parameters, runtimeParameters, apiCallback);

        }
    }
}
