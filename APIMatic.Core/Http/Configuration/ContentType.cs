// <copyright file="ContentType.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Runtime.Serialization;
using APIMatic.Core.Utilities;
using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;

namespace APIMatic.Core.Http.Configuration
{
    /// <summary>
    /// This enumeration has ContentType Values.
    /// </summary>
    /// 
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum ContentType
    {
        [EnumMember(Value = "application/json")]
        JSON,

        [EnumMember(Value = "application/json; charset=utf-8")]
        JSON_UTF8,

        [EnumMember(Value = "application/xml")]
        XML,

        [EnumMember(Value = "text/plain; charset=utf-8")]
        SCALAR,

        [EnumMember(Value = "application/octet-stream")]
        BINARY,

        [EnumMember(Value = "application/x-www-form-urlencoded")]
        FORM_URL_ENCODED
    }


    /// <summary>
    /// ContentType enumeration extention class.
    /// </summary>
    internal static class ContentTypeExtensions
    {
        /// <summary>
        /// Return the value of the content type enum member.
        /// </summary>
        public static string GetValue(this ContentType contentType) => CoreHelper.JsonSerialize(contentType).Substring(1).TrimEnd('"');
    }
}
