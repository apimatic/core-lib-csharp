﻿// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Request.Parameters
{
    public class HeaderParam : Parameter
    {
        internal HeaderParam() => typeName = "header";

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            requestBuilder.headers.Add(key, value.ToString());
        }
    }
}