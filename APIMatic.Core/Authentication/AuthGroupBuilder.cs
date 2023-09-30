// <copyright file="AuthManager.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using APIMatic.Core.Request;

namespace APIMatic.Core.Authentication
{
    public class AuthGroupBuilder : AuthManager
    {
        private readonly Dictionary<string, AuthManager> authManagersMap;
        private readonly List<AuthManager> authManagers = new List<AuthManager>();
        private bool isAndGroup = false;

        internal AuthGroupBuilder(Dictionary<string, AuthManager> authManagers)
        {
            authManagersMap = authManagers ?? new Dictionary<string, AuthManager>();
        }

        internal AuthGroupBuilder SetAsAndGroup()
        {
            isAndGroup = true;
            return this;
        }

        public AuthGroupBuilder Add(string authName)
        {
            if (authManagersMap.TryGetValue(authName, out AuthManager authManager))
            {
                authManagers.Add(authManager);
            }
            return this;
        }

        public AuthGroupBuilder AddAndGroup(Action<AuthGroupBuilder> applyAuth)
        {
            var authGroup = new AuthGroupBuilder(authManagersMap);
            applyAuth(authGroup);
            authManagers.Add(authGroup.SetAsAndGroup());
            return this;
        }

        public AuthGroupBuilder AddOrGroup(Action<AuthGroupBuilder> applyAuth)
        {
            var authGroup = new AuthGroupBuilder(authManagersMap);
            applyAuth(authGroup);
            authManagers.Add(authGroup);
            return this;
        }

        /// <summary>
        /// Add authentication group information to the RequestBuilder.
        /// </summary>
        /// <param name="requestBuilder">The RequestBuilder object on which authentication will be applied.</param>
        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!authManagers.Any())
            {
                return;
            }
            var errors = new List<string>();
            foreach (var authManager in authManagers)
            {
                try
                {
                    authManager.Apply(requestBuilder);
                }
                catch (ArgumentNullException ex)
                {
                    if (isAndGroup)
                    {
                        // throw exception if unable to apply any authentication in AND group
                        throw;
                    }
                    errors.Add(ex.Message);
                }
            }

            if (errors.Count == authManagers.Count)
            {
                // throw exception if unable to apply all authentications in OR group
                throw new ArgumentNullException("Authentication", $"Missing required auth credentials:\n-> {string.Join("\n-> ", errors)}");
            }
        }
    }
}
