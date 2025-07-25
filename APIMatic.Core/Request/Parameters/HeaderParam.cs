﻿// <copyright file="HeaderParam.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Header parameter class used to send a request parameter in header
    /// </summary>
    public class HeaderParam : Parameter
    {
        internal HeaderParam() => typeName = "header";

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated) return;
            requestBuilder.headersParameters[key] = value;
        }
    }
}
