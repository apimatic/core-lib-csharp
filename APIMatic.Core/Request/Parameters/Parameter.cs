// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;

namespace APIMatic.Core.Request.Parameters
{
    public abstract class Parameter
    {
        protected string key;
        protected object value;
        private bool requiredValueMissing = false;
        private InvalidOperationException serializationError;
        protected bool validated = false;
        protected string typeName;

        private string GetName()
        {
            return key == "" ? typeName : key;
        }

        public Parameter Setup(string key, object value)
        {
            this.key = key;
            this.value = value;
            return this;
        }

        public Parameter Required()
        {
            if (value == null)
            {
                requiredValueMissing = true;
            }
            return this;
        }

        public Parameter Serializer(Func<object, object> valueSerializer)
        {
            try
            {
                value = valueSerializer(value);
            }
            catch (Exception exp)
            {
                serializationError = new InvalidOperationException($"Unable to serialize field: {GetName()}, Due to:\n{exp.Message}");
            }
            return this;
        }

        internal void Validate()
        {
            if (validated)
            {
                return;
            }
            if (key == null)
            {
                throw new ArgumentNullException($"Missing required `key` for type: {typeName}");
            }
            if (requiredValueMissing)
            {
                throw new ArgumentNullException($"Missing required {typeName} field: {GetName()}");
            }
            if (serializationError != null)
            {
                throw serializationError;
            }

            validated = true;
            return;
        }

        internal abstract void Apply(RequestBuilder requestBuilder);

        public class Builder
        {
            private readonly List<Parameter> parameters = new List<Parameter>();

            internal Builder() { }

            public Builder Header(Action<HeaderParam> action)
            {
                var header = new HeaderParam();
                action(header);
                parameters.Add(header);
                return this;
            }

            public Builder Query(Action<QueryParam> action)
            {
                var query = new QueryParam();
                action(query);
                parameters.Add(query);
                return this;
            }

            public Builder Template(Action<TemplateParam> action)
            {
                var template = new TemplateParam();
                action(template);
                parameters.Add(template);
                return this;
            }

            internal Builder Validate()
            {
                parameters.ForEach(p => p.Validate());
                return this;
            }

            internal void Apply(RequestBuilder requestBuilder)
            {
                parameters.ForEach(p => p.Apply(requestBuilder));
            }
        }
    }
}
