// <copyright file="AuthManager.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using APIMatic.Core.Request;
using APIMatic.Core.Request.Parameters;

namespace APIMatic.Core.Authentication
{
    /// <summary>
    /// AuthManager adds the authentication layer to the HTTP Calls.
    /// <summary>
    public class AuthManager
    {
        private readonly Parameter.Builder parameters = new Parameter.Builder();

        /// <summary>
        /// Sets the authorization parameters using <see cref="Parameter.Builder"/>
        /// </summary>
        /// <param name="_params">The action to be applied on Parameter.Builder</param>
        /// <returns></returns>
        protected AuthManager Parameters(Action<Parameter.Builder> _params)
        {
            _params(parameters);
            return this;
        }

        /// <summary>
        /// Validates the authentication parameters for the HTTP Request.
        /// </summary>
        public virtual void Validate()
        {
            parameters.Validate();
        }

        /// <summary>
        /// Add authentication information to the HTTP Request.
        /// </summary>
        /// <param name="requestBuilder">The http request object on which authentication will be applied.</param>
        /// <returns>HttpRequest.</returns>
        internal void Apply(RequestBuilder requestBuilder)
        {
            Validate();
            parameters.Apply(requestBuilder);
        }
    }
}
