// <copyright file="IAuthentication.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using APIMatic.Core.Request;
using APIMatic.Core.Request.Parameters;

namespace APIMatic.Core.Authentication
{
    /// <summary>
    /// IAuthentication adds the authenticaion layer to the HTTP Calls.
    /// <summary>
    public class AuthManager
    {
        private readonly Parameter.Builder parameters = new Parameter.Builder();

        /// <summary>
        /// Sets the authorization parameters using <see cref="Parameter.Builder"/>
        /// </summary>
        /// <param name="action">The action to be applied on Parameter.Builder</param>
        /// <returns></returns>
        protected AuthManager Parameters(Action<Parameter.Builder> action)
        {
            action(parameters);
            return this;
        }

        /// <summary>
        /// Validates the authentication parameters for the HTTP Request.
        /// </summary>
        public AuthManager Validate()
        {
            parameters.Validate();
            return this;
        }

        /// <summary>
        /// Add authentication information to the HTTP Request.
        /// </summary>
        /// <param name="requestBuilder">The http request object on which authentication will be applied.</param>
        /// <returns>HttpRequest.</returns>
        internal void Apply(RequestBuilder requestBuilder)
        {
            parameters.Apply(requestBuilder);
        }
    }
}
