// <copyright file="QueryAuthentication.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using APIMatic.Core.Types;

namespace APIMatic.Core.Authentication
{
    /// <summary>
    /// QueryAuthentication is an implementation of <see cref="IAuthentication"/>
    /// that supports HTTP authentication through HTTP query parameters.
    /// </summary>
    public class QueryAuthentication : IAuthentication
    {
        private IDictionary<string, object> _authenticationParameters = new Dictionary<string, object>();

        public QueryAuthentication(IDictionary<string, object> authenticationParameters)
        {
            _authenticationParameters = authenticationParameters;
        }

        /// <summary>
        /// Add the query authentication information to the HTTP Request.
        /// </summary>
        /// <param name="request">The <see cref="CoreRequest"/> object on which authentication will be applied.</param>
        /// <returns></returns>
        public CoreRequest Apply(CoreRequest request)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate the query authentication parameters for the HTTP Request.
        /// </summary>
        public void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
