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
            if (value is DateTime dateTime)
            {
                return dateTime.ToString(CoreHelper.DateTimeFormat);
            }
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.ToString(CoreHelper.DateTimeFormat);
            }
            return value.ToString();
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated)
            {
                return;
            }
            string replacerValue = Uri.EscapeUriString(GetReplacerValue(value));
            requestBuilder.QueryUrl.Replace(string.Format("{{{0}}}", key), replacerValue);
        }
    }
}
