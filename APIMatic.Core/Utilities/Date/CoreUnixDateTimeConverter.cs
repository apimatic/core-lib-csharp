﻿// <copyright file="CoreUnixDateTimeConverter.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIMatic.Core.Utilities.Date
{
    /// <summary>
    /// Extends from DateTimeConverterBase, uses unix DateTime format.
    /// </summary>
    public class CoreUnixDateTimeConverter : DateTimeConverterBase
    {
        /// <summary>
        /// Gets or sets DateTimeStyles.
        /// </summary>
        public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.RoundtripKind;

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double text;
            if (value is DateTime dateTime)
            {
                if ((DateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (DateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTime = dateTime.ToUniversalTime();
                }

                text = dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
            else
            {
                throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime.");
            }

            writer.WriteValue(text);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(double.Parse(reader.Value.ToString()));
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            throw new JsonSerializationException("Unexpected token");
        }
    }
}
