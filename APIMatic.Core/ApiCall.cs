// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Http.Client.Configuration;
using APIMatic.Core.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core
{
    class ApiCall
    {
        private GlobalConfiguration globalConfiguration;
        private Enum server;
        private RequestBuilder requestBuilder;
        private ArrayDeserialization arrayDeserialization = ArrayDeserialization.Indexed;

        public ApiCall(GlobalConfiguration configuration) => globalConfiguration = configuration;

        public ApiCall Server(Enum server)
        {
            this.server = server;
            return this;
        }

        public ApiCall ArrayDeserlization(ArrayDeserialization arrayDeserialization)
        {
            this.arrayDeserialization = arrayDeserialization;
            return this;
        }

        public ApiCall RequestBuilder(Action<RequestBuilder> requestBuilderAction)
        {
            requestBuilder = globalConfiguration.GlobalRequestBuilder(server);
            requestBuilder.ArrayDeserialization = arrayDeserialization;
            requestBuilderAction(requestBuilder);
            return this;
        }
        // response hanlder
        // execute
        // execute async
    }
}
