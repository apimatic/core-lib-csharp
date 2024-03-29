﻿// <copyright file="AdditionalFormParams.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Used to add additional form params to a request
    /// </summary>
    public class AdditionalFormParams : MultipleParams
    {
        internal AdditionalFormParams() => typeName = "additional form";

        public override MultipleParams Setup(string key, object value)
        {
            parameters.Form(f => f.Setup(key, value));
            return this;
        }
    }
}
