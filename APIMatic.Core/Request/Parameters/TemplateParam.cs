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

        private string GetReplacerValueForCollection(ICollection collection)
        {
            var replacedValues = new List<string>();
            var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                replacedValues.Add(GetReplacerValue(enumerator.Current));
            }
            return string.Join("/", replacedValues);
        }

        private string GetReplacerValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (value is ICollection collection)
            {
                return GetReplacerValueForCollection(collection);
            }

            return CoreHelper.JsonSerialize(value).TrimStart('"').TrimEnd('"');
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            CoreHelper.TryGetInnerValueForContainer(value, out var innerValue);
            string replacerValue = Uri.EscapeUriString(GetReplacerValue(innerValue ?? value));
            requestBuilder.QueryUrl.Replace(string.Format("{{{0}}}", key), replacerValue);
        }

        public override Parameter Clone()
        {
            return new TemplateParam
            {
                key = this.key,
                value = this.value,
                validated = this.validated,
                typeName = this.typeName
            };
        }
    }
}
