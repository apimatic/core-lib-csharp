// <copyright file="TemplateParam.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections;
using System.Collections.Generic;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Template parameter class used to send a request parameter in template
    /// </summary>
    public class TemplateParam : Parameter
    {
        internal TemplateParam() => typeName = "template";

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            CoreHelper.TryGetInnerValueForContainer(value, out var innerValue);
            requestBuilder.templateParameters.Add(key, innerValue ?? value);
        }
    }
}
    
