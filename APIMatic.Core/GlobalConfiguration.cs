// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
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
        private readonly List<Parameter> globalParameters;

        private GlobalConfiguration(ICoreHttpClientConfiguration httpConfiguration, Dictionary<string, object> authManagers, Dictionary<Enum, string> serverUrls,
            Enum defaultServer, List<Parameter> globalParameters, List<Parameter> globalRuntimeParameters, HttpCallBack apiCallback)
        {
            this.httpConfiguration = httpConfiguration;
            this.serverUrls = serverUrls;
            this.defaultServer = defaultServer;
            this.globalParameters = globalParameters;
            ApiCallback = apiCallback;
            AuthManagers = authManagers;
            GlobalRuntimeParameters = globalRuntimeParameters;
        }
        internal Dictionary<string, object> AuthManagers { get; private set; }
        internal List<Parameter> GlobalRuntimeParameters { get; private set; }
        internal HttpCallBack ApiCallback { get; private set; }

        public RequestBuilder GlobalRequestBuilder(Enum server = null)
        {
            RequestBuilder requestBuilder = new RequestBuilder(this)
                .ServerUrl(serverUrls[server ?? defaultServer]);
            globalParameters.ForEach(param => param.Apply(requestBuilder));
            return requestBuilder;
        }

        internal HttpClientWrapper HttpClient()
        {
            return new HttpClientWrapper(httpConfiguration);
        }

        public class Builder
        {
            private ICoreHttpClientConfiguration httpConfiguration;
            private Dictionary<string, object> authManagers = new Dictionary<string, object>();
            private Dictionary<Enum, string> serverUrls = new Dictionary<Enum, string>();
            private Enum defaultServer;
            private List<Parameter> globalParameters = new List<Parameter>();
            private List<Parameter> globalRuntimeParameters = new List<Parameter>();
            private HttpCallBack apiCallback;

            public Builder HttpConfiguration(ICoreHttpClientConfiguration httpConfiguration)
            {
                this.httpConfiguration = httpConfiguration;
                return this;
            }

            public Builder AuthManagers(Dictionary<string, object> authManagers)
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

            public Builder HeaderParam(Action<HeaderParam> headerAction)
            {
                var header = new HeaderParam();
                headerAction(header);
                globalParameters.Add(header);
                return this;
            }

            public Builder TemplateParam(Action<TemplateParam> templateAction)
            {
                var template = new TemplateParam();
                templateAction(template);
                globalParameters.Add(template);
                return this;
            }

            public Builder RuntimeHeaderParam(Action<HeaderParam> headerAction)
            {
                var header = new HeaderParam();
                headerAction(header);
                globalRuntimeParameters.Add(header);
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
                globalParameters.Add(new HeaderParam().Init("user-agent", userAgent));
                return this;
            }

            public GlobalConfiguration Build() => new GlobalConfiguration(httpConfiguration, authManagers, serverUrls, defaultServer,
                    globalParameters, globalRuntimeParameters, apiCallback);

        }
    }
}
