﻿// <copyright file="MultipleParams.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Request.Parameters
{
    public abstract class MultipleParams : Parameter
    {
        private bool multipleValidate;
        protected readonly Builder parameters = new Builder();

        public new abstract MultipleParams Setup(string key, object value);

        public MultipleParams Setup(Dictionary<string, object> multipleParams)
        {
            multipleParams?.ToList().ForEach(keyValPair => Setup(keyValPair.Key, keyValPair.Value));
            return this;
        }

        internal override void Validate()
        {
            if (multipleValidate)
            {
                return;
            }
            parameters.Validate();
            multipleValidate = true;
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!multipleValidate)
            {
                return;
            }
            parameters.Apply(requestBuilder);
        }
    }
}
