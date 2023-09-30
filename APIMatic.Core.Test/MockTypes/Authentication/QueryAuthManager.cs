// <copyright file="QueryAuthManager.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Authentication;

namespace APIMatic.Core.Test.MockTypes.Authentication
{
    /// <summary>
    /// QueryAuthManager Class.
    /// </summary>
    public class QueryAuthManager : AuthManager
    {
        public QueryAuthManager(string apiKey, string token)
        {
            Parameters(paramBuilder => paramBuilder
                .Query(query => query.Setup("API-KEY", apiKey).Required())
                .Query(query => query.Setup("TOKEN", token).Required()));
        }
    }
}
