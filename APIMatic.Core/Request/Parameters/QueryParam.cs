// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace APIMatic.Core.Request.Parameters
{
    public class QueryParam : Parameter
    {
        internal QueryParam() => typeName = "query";

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            requestBuilder.queryParameters.Add(key, value);
        }
    }
}
