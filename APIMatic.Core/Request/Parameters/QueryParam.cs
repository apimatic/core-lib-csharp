// <copyright file="QueryParam.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Query parameter class used to send a request parameter in query
    /// </summary>
    public class QueryParam : Parameter
    {
        internal QueryParam() => typeName = "query";

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated) return;
            requestBuilder.queryParameters[key] = value;
        }
    }
}
