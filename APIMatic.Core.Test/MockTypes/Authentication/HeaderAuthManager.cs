// <copyright file="HeaderAuthManager.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Authentication;

namespace APIMatic.Core.Test.MockTypes.Authentication
{
    /// <summary>
    /// QueryAuthManager Class.
    /// </summary>
    public class HeaderAuthManager : AuthManager
    {
        public HeaderAuthManager(string apiKey, string token)
        {
            Parameters(paramBuilder => paramBuilder
                .Header(header => header.Setup("API-KEY", apiKey).Required())
                .Header(header => header.Setup("TOKEN", token).Required()));
        }
    }
}
