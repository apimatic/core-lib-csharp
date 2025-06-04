// <copyright file="AdditionalQueryParams.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Used to add additional query params to a request
    /// </summary>
    public class AdditionalQueryParams : MultipleParams
    {
        internal AdditionalQueryParams() => typeName = "additional query";

        public override MultipleParams Setup(string key, object value)
        {
            parameters.Query(q => q.Setup(key, value));
            return this;
        }
    }
}
