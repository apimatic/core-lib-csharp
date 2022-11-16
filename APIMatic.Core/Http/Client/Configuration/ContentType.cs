// <copyright file="ArrayDeserialization.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Runtime.Serialization;
using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIMatic.Core.Http.Client.Configuration
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

        [EnumMember(Value = "application/xml")]
        XML,

        [EnumMember(Value = "text/plain; charset=utf-8")]
        SCALAR,

        [EnumMember(Value = "application/octect-stream")]
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
        public static string GetValue(this ContentType contentType) => CoreHelper.JsonSerialize(contentType);
    }
}
