// <copyright file="BodyParam.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;

namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Body parameter class used to send a request parameter in body
    /// </summary>
    public class BodyParam : Parameter
    {
        internal BodyParam() => typeName = "body";
        protected Type valueType;

        public Parameter Setup(object value)
        {
            Setup("", value);
            valueType = value?.GetType();
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
                requestBuilder.bodyType = valueType;
                return;
            }
            requestBuilder.bodyParameters[key] = value;
        }
    }
}
