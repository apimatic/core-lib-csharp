// <copyright file="BasicAuthManager.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Text;
using APIMatic.Core.Authentication;

namespace APIMatic.Core.Test.MockTypes.Authentication
{
    /// <summary>
    /// BasicAuthManager Class.
    /// </summary>
    public class BasicAuthManager : AuthManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IrisBasicAuthManager"/> class.
        /// </summary>
        /// <param name="username"> Username.</param>
        /// <param name="password"> Password.</param>
        public BasicAuthManager(string username, string password)
        {
            BasicAuthUserName = username;
            BasicAuthPassword = password;
            Parameters(paramBuilder => paramBuilder
                .Header(header => header.Setup("Authorization", GetBasicAuthHeader()).Required()));
        }

        /// <summary>
        /// Gets string value for basicAuthUserName.
        /// </summary>
        public string BasicAuthUserName { get; }

        /// <summary>
        /// Gets string value for basicAuthPassword.
        /// </summary>
        public string BasicAuthPassword { get; }

        public string GetBasicAuthHeader()
        {
            if (BasicAuthUserName == null || BasicAuthPassword == null)
            {
                return null;
            }
            var authCredentials = BasicAuthUserName + ":" + BasicAuthPassword;
            var data = Encoding.ASCII.GetBytes(authCredentials);
            return "Basic " + Convert.ToBase64String(data);
        }
    }
}
