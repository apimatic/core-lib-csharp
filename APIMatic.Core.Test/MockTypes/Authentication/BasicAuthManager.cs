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
    internal class BasicAuthManager : AuthManager, IBasicAuthCredentials
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
                .Header(header => header.Setup("Authorization", GetBasicAuthHeader())));
        }

        /// <summary>
        /// Gets string value for basicAuthUserName.
        /// </summary>
        public string BasicAuthUserName { get; }

        /// <summary>
        /// Gets string value for basicAuthPassword.
        /// </summary>
        public string BasicAuthPassword { get; }

        /// <summary>
        /// Check if credentials match.
        /// </summary>
        /// <param name="basicAuthUserName"> The string value for credentials.</param>
        /// <param name="basicAuthPassword"> The string value for credentials.</param>
        /// <returns> True if credentials matched.</returns>
        public bool Equals(string basicAuthUserName, string basicAuthPassword)
        {
            return basicAuthUserName.Equals(BasicAuthUserName)
                    && basicAuthPassword.Equals(BasicAuthPassword);
        }

        private string GetBasicAuthHeader()
        {
            var authCredentials = BasicAuthUserName + ":" + BasicAuthPassword;
            var data = Encoding.ASCII.GetBytes(authCredentials);
            return "Basic " + Convert.ToBase64String(data);
        }
    }
}
