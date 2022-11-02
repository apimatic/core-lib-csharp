// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Request.Parameters
{
    public abstract class MultipleParams : Parameter
    {
        protected readonly Builder parameters = new Builder();

        public new abstract MultipleParams Setup(string key, object value);

        public MultipleParams Setup(Dictionary<string, object> multipleParams)
        {
            multipleParams.ToList().ForEach(keyValPair => Setup(keyValPair.Key, keyValPair.Value));
            return this;
        }

        internal override void Validate()
        {
            if (validated)
            {
                return;
            }
            parameters.Validate();
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            parameters.Apply(requestBuilder);
        }
    }
}
