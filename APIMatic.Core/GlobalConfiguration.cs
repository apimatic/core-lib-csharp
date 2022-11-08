// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Authentication;
using APIMatic.Core.Http.Client;
using APIMatic.Core.Http.Client.Configuration;
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
        private readonly ICoreHttpClientConfiguration httpConfiguration;
        private readonly Dictionary<Enum, string> serverUrls;
        private readonly Enum defaultServer;
        private readonly Parameter.Builder globalParameters;

        private GlobalConfiguration(ICoreHttpClientConfiguration httpConfiguration, Dictionary<string, AuthManager> authManagers,
            Dictionary<Enum, string> serverUrls, Enum defaultServer, Parameter.Builder globalParameters,
            Parameter.Builder globalRuntimeParameters, HttpCallBack apiCallback)
        {
            this.httpConfiguration = httpConfiguration;
            this.serverUrls = serverUrls;
            this.defaultServer = defaultServer;
            this.globalParameters = globalParameters;
            ApiCallback = apiCallback;
            AuthManagers = authManagers;
            GlobalRuntimeParameters = globalRuntimeParameters;
        }
        internal Dictionary<string, AuthManager> AuthManagers { get; private set; }
        internal Parameter.Builder GlobalRuntimeParameters { get; private set; }
        internal HttpCallBack ApiCallback { get; private set; }

        public string ServerUrl(Enum server = null) => GlobalRequestBuilder(server).QueryUrl.ToString();

        internal RequestBuilder GlobalRequestBuilder(Enum server)
        {
            RequestBuilder requestBuilder = new RequestBuilder(this);
            requestBuilder.QueryUrl.Append(serverUrls[server ?? defaultServer]);
            globalParameters.Validate().Apply(requestBuilder);
            return requestBuilder;
        }

        internal IHttpClient HttpClient()
        {
            return new HttpClientWrapper(httpConfiguration);
        }

        public class Builder
        {
            private ICoreHttpClientConfiguration httpConfiguration;
            private Dictionary<string, AuthManager> authManagers = new Dictionary<string, AuthManager>();
            private Dictionary<Enum, string> serverUrls = new Dictionary<Enum, string>();
            private Enum defaultServer;
            private readonly Parameter.Builder globalParameters = new Parameter.Builder();
            private readonly Parameter.Builder globalRuntimeParameters = new Parameter.Builder();
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

            public Builder GlobalParameters(Action<Parameter.Builder> action)
            {
                action(globalParameters);
                return this;
            }

            public Builder GlobalRuntimeParameters(Action<Parameter.Builder> action)
            {
                action(globalRuntimeParameters);
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
                globalParameters.Header(h => h.Setup("user-agent", userAgent));
                return this;
            }

            public GlobalConfiguration Build() => new GlobalConfiguration(httpConfiguration, authManagers, serverUrls, defaultServer,
                    globalParameters, globalRuntimeParameters, apiCallback);

        }
    }
}
