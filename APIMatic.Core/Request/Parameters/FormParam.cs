// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;

namespace APIMatic.Core.Request.Parameters
{
    public class FormParam : Parameter
    {
        internal FormParam() => typeName = "form";

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            requestBuilder.formParameters.Add(new KeyValuePair<string, object>(key, value));
        }
    }
}
