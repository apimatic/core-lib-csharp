// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Request.Parameters
{
    public class TemplateParam : Parameter
    {
        internal TemplateParam() => typeName = "template";

        private string GetReplacerValueForCollection(ICollection<object> collection) =>
            string.Join("/", collection.Select(item => GetReplacerValue(item)));

        private string GetReplacerValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            if (value is ICollection<object> collection)
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
            requestBuilder.queryUrl.Replace(string.Format("{{{0}}}", key), replacerValue);
        }
    }
}
