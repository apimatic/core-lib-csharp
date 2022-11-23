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
        private Func<object, object> valueSerializer = value => value;
        protected bool validated = false;
        protected string typeName;

        private string GetName() => key == "" ? typeName : key;

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

        public Parameter Serializer(Func<object, string> valueSerializer)
        {
            this.valueSerializer = valueSerializer;
            return this;
        }

        internal virtual void Validate()
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
            try
            {
                value = valueSerializer(value);
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException($"Unable to serialize field: {GetName()}, Due to:\n{exp.Message}");
            }

            validated = true;
        }

        internal abstract void Apply(RequestBuilder requestBuilder);

        public class Builder
        {
            private readonly List<Parameter> parameters = new List<Parameter>();

            internal Builder() { }

            public Builder Template(Action<TemplateParam> action)
            {
                var template = new TemplateParam();
                action(template);
                parameters.Add(template);
                return this;
            }

            public Builder Header(Action<HeaderParam> action)
            {
                var header = new HeaderParam();
                action(header);
                parameters.Add(header);
                return this;
            }

            public Builder AdditionalHeaders(Action<AdditionalHeaderParams> action)
            {
                var headers = new AdditionalHeaderParams();
                action(headers);
                parameters.Add(headers);
                return this;
            }

            public Builder Query(Action<QueryParam> action)
            {
                var query = new QueryParam();
                action(query);
                parameters.Add(query);
                return this;
            }

            public Builder AdditionalQueries(Action<AdditionalQueryParams> action)
            {
                var queries = new AdditionalQueryParams();
                action(queries);
                parameters.Add(queries);
                return this;
            }

            public Builder Form(Action<FormParam> action)
            {
                var form = new FormParam();
                action(form);
                parameters.Add(form);
                return this;
            }

            public Builder AdditionalForms(Action<AdditionalFormParams> action)
            {
                var forms = new AdditionalFormParams();
                action(forms);
                parameters.Add(forms);
                return this;
            }

            public Builder Body(Action<BodyParam> action)
            {
                var body = new BodyParam();
                action(body);
                parameters.Add(body);
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
