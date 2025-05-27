// <copyright file="AdditionalHeaderParams.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Used to add additional header params to a request
    /// </summary>
    public class AdditionalHeaderParams : MultipleParams
    {
        internal AdditionalHeaderParams() => typeName = "additional header";

        public override MultipleParams Setup(string key, object value)
        {
            parameters.Header(h => h.Setup(key, value));
            return this;
        }

        public override Parameter Clone()
        {
            var clone = new AdditionalHeaderParams
            {
                key = this.key,
                value = this.value,
                validated = this.validated,
                typeName = this.typeName
            };

            this.parameters.Clone(clone.parameters);
            return clone;
        }
    }
}
