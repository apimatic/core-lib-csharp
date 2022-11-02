// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace APIMatic.Core.Request.Parameters
{
    public class BodyParam : Parameter
    {
        internal BodyParam() => typeName = "body";

        public Parameter Setup(object value)
        {
            Setup("", value);
            return this;
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            if (key == "")
            {
                requestBuilder.body = value;
                return;
            }
            requestBuilder.bodyParameters.Add(key, value);

        }
    }
}
