// <copyright file="IAuthentication.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Authentication
{
    /// <summary>
    /// IAuthentication adds the authenticaion layer to the HTTP Calls.
    /// <summary>
    public interface IAuthentication
    {
        /// <summary>
        /// Add authentication information to the HTTP Request.
        /// </summary>
        /// <param name="httpRequest">The http request object on which authentication will be applied.</param>
        /// <returns>HttpRequest.</returns>
        CoreRequest Apply(CoreRequest httpRequest);

        /// <summary>
        /// Validates the authentication parameters for the HTTP Request.
        /// </summary>
        void Validate();
    }
}
