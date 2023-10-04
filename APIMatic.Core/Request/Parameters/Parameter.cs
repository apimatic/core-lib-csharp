﻿// <copyright file="Parameter.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;

namespace APIMatic.Core.Request.Parameters
{
    /// <summary>
    /// Base class for all the request parameters, holding the common functionality
    /// </summary>
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
                throw new ArgumentNullException(null, $"Missing required `key` for type: {typeName}");
            }
            if (requiredValueMissing)
            {
                throw new ArgumentNullException(null, $"Missing required {typeName} field: {GetName()}");
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

        /// <summary>
        /// Used to build a request by creating and adding multiple parameters into the RequestBuilder
        /// </summary>
        public class Builder
        {
            private readonly List<Parameter> parameters = new List<Parameter>();

            internal Builder() { }

            /// <summary>
            /// Creates and configure a TemplateParam using its Action
            /// </summary>
            /// <param name="_template"></param>
            /// <returns></returns>
            public Builder Template(Action<TemplateParam> _template)
            {
                var template = new TemplateParam();
                _template(template);
                parameters.Add(template);
                return this;
            }

            /// <summary>
            /// Creates and configure a HeaderParam using its Action
            /// </summary>
            /// <param name="_header"></param>
            /// <returns></returns>
            public Builder Header(Action<HeaderParam> _header)
            {
                var header = new HeaderParam();
                _header(header);
                parameters.Add(header);
                return this;
            }

            /// <summary>
            /// Creates and configure AdditionalHeaderParams using its Action
            /// </summary>
            /// <param name="_headers"></param>
            /// <returns></returns>

            public Builder AdditionalHeaders(Action<AdditionalHeaderParams> _headers)
            {
                var headers = new AdditionalHeaderParams();
                _headers(headers);
                parameters.Add(headers);
                return this;
            }

            /// <summary>
            /// Creates and configure a QueryParam using its Action
            /// </summary>
            /// <param name="_query"></param>
            /// <returns></returns>
            public Builder Query(Action<QueryParam> _query)
            {
                var query = new QueryParam();
                _query(query);
                parameters.Add(query);
                return this;
            }

            /// <summary>
            /// Creates and configure AdditionalQueryParams using its Action
            /// </summary>
            /// <param name="_queries"></param>
            /// <returns></returns>
            public Builder AdditionalQueries(Action<AdditionalQueryParams> _queries)
            {
                var queries = new AdditionalQueryParams();
                _queries(queries);
                parameters.Add(queries);
                return this;
            }

            /// <summary>
            /// Creates and configure a FormParam using its Action
            /// </summary>
            /// <param name="_form"></param>
            /// <returns></returns>
            public Builder Form(Action<FormParam> _form)
            {
                var form = new FormParam();
                _form(form);
                parameters.Add(form);
                return this;
            }

            /// <summary>
            /// Creates and configure a AdditionalFormParams using its Action
            /// </summary>
            /// <param name="_forms"></param>
            /// <returns></returns>
            public Builder AdditionalForms(Action<AdditionalFormParams> _forms)
            {
                var forms = new AdditionalFormParams();
                _forms(forms);
                parameters.Add(forms);
                return this;
            }

            /// <summary>
            /// Creates and configure a BodyParam using its Action
            /// </summary>
            /// <param name="_body"></param>
            /// <returns></returns>
            public Builder Body(Action<BodyParam> _body)
            {
                var body = new BodyParam();
                _body(body);
                parameters.Add(body);
                return this;
            }

            /// <summary>
            /// Validates all the added parameters
            /// </summary>
            /// <returns></returns>
            internal Builder Validate()
            {
                parameters.ForEach(p => p.Validate());
                return this;
            }

            /// <summary>
            /// Adds all the parameters into the RequestBuilder
            /// </summary>
            /// <returns></returns>
            internal void Apply(RequestBuilder requestBuilder)
            {
                parameters.ForEach(p => p.Apply(requestBuilder));
            }
        }
    }
}
