// <copyright file="GlobalConfiguration.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using APIMatic.Core.Authentication;
using APIMatic.Core.Http;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Request;
using APIMatic.Core.Request.Parameters;
using APIMatic.Core.Types;
using APIMatic.Core.Utilities.Logger.Configuration;

namespace APIMatic.Core
{
    /// <summary>
    /// Carries the common configuration that will be applicable to all the ApiCalls
    /// </summary>
    public class GlobalConfiguration
    {
        private readonly Dictionary<Enum, string> _serverUrls;
        private readonly Enum _defaultServer;
        private readonly Parameter.Builder _parameters;

        internal SdkLoggingConfiguration SdkLoggingConfiguration { get; private set; }

        internal Dictionary<string, AuthManager> AuthManagers { get; private set; }
        internal Parameter.Builder RuntimeParameters { get; private set; }
        internal HttpCallBack ApiCallback { get; private set; }
        internal HttpClientWrapper HttpClient { get; private set; }

        private GlobalConfiguration(ICoreHttpClientConfiguration httpConfiguration, Dictionary<string, AuthManager> authManagers,
            Dictionary<Enum, string> serverUrls, Enum defaultServer, Parameter.Builder parameters,
            Parameter.Builder runtimeParameters, HttpCallBack apiCallback, SdkLoggingConfiguration sdkLoggingConfiguration)
        {
            _serverUrls = serverUrls;
            _defaultServer = defaultServer;
            _parameters = parameters;
            ApiCallback = apiCallback;
            SdkLoggingConfiguration = sdkLoggingConfiguration;
            AuthManagers = authManagers;
            RuntimeParameters = runtimeParameters;
            HttpClient = new HttpClientWrapper(httpConfiguration);
        }

        /// <summary>
        /// Returns the server url associated with a particular server
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public string ServerUrl(Enum server = null) => GlobalRequestBuilder(server).QueryUrl.ToString();

        /// <summary>
        /// Returns the global request builder after applying the global configurations
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public RequestBuilder GlobalRequestBuilder(Enum server = null)
        {
            RequestBuilder requestBuilder = new RequestBuilder(this);
            requestBuilder.QueryUrl.Append(_serverUrls[server ?? _defaultServer]);
            _parameters.Validate().Apply(requestBuilder);
            return requestBuilder;
        }

        /// <summary>
        /// Initializes the immutable instance of GlobalConfiguration class
        /// </summary>
        public class Builder
        {
            private ICoreHttpClientConfiguration httpConfiguration;
            private Dictionary<string, AuthManager> authManagers = new Dictionary<string, AuthManager>();
            private Dictionary<Enum, string> serverUrls = new Dictionary<Enum, string>();
            private Enum defaultServer;
            private readonly Parameter.Builder parameters = new Parameter.Builder();
            private readonly Parameter.Builder runtimeParameters = new Parameter.Builder();
            private HttpCallBack apiCallback;
            private SdkLoggingConfiguration sdkLoggingConfiguration;

            /// <summary>
            /// Required: Configures the http configurations
            /// </summary>
            /// <param name="httpConfiguration"></param>
            /// <returns></returns>
            public Builder HttpConfiguration(ICoreHttpClientConfiguration httpConfiguration)
            {
                this.httpConfiguration = httpConfiguration;
                return this;
            }

            /// <summary>
            /// Configures the dictionary of auth managers
            /// </summary>
            /// <param name="authManagers"></param>
            /// <returns></returns>
            public Builder AuthManagers(Dictionary<string, AuthManager> authManagers)
            {
                this.authManagers = authManagers;
                return this;
            }

            /// <summary>
            /// Required: Configures the server urls and select a default one
            /// </summary>
            /// <param name="serverUrls"></param>
            /// <param name="defaultServer"></param>
            /// <returns></returns>
            public Builder ServerUrls(Dictionary<Enum, string> serverUrls, Enum defaultServer)
            {
                this.serverUrls = serverUrls;
                this.defaultServer = defaultServer;
                return this;
            }

            /// <summary>
            /// Configures the common parameters for all the API calls
            /// </summary>
            /// <param name="_params"></param>
            /// <returns></returns>
            public Builder Parameters(Action<Parameter.Builder> _params)
            {
                _params(parameters);
                return this;
            }

            /// <summary>
            /// Configures the common parameters which will applied with highest priority
            /// </summary>
            /// <param name="_params"></param>
            /// <returns></returns>
            public Builder RuntimeParameters(Action<Parameter.Builder> _params)
            {
                _params(runtimeParameters);
                return this;
            }

            /// <summary>
            /// Sets the HttpCallback
            /// </summary>
            /// <param name="apiCallback"></param>
            /// <returns></returns>
            public Builder ApiCallback(HttpCallBack apiCallback)
            {
                this.apiCallback = apiCallback;
                return this;
            }

            /// <summary>
            /// Sets the logging configuration for the SDK.
            /// </summary>
            /// <param name="sdkLoggingConfiguration">The logging configuration to be set for the SDK.</param>
            /// <returns>A reference to the current builder instance.</returns>
            public Builder LoggingConfig(SdkLoggingConfiguration sdkLoggingConfiguration)
            {
                this.sdkLoggingConfiguration = sdkLoggingConfiguration;
                return this;
            }

            /// <summary>
            /// Sets the user-agent header and configure values and place holders in it
            /// </summary>
            /// <param name="userAgent"></param>
            /// <param name="userAgentConfig"></param>
            /// <returns></returns>
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

            /// <summary>
            /// This applies all the configuration and build an instance of GlobalConfiguration
            /// </summary>
            /// <returns></returns>
            public GlobalConfiguration Build() => new GlobalConfiguration(httpConfiguration, authManagers, serverUrls, defaultServer,
                    parameters, runtimeParameters, apiCallback, sdkLoggingConfiguration);

        }
    }
}
