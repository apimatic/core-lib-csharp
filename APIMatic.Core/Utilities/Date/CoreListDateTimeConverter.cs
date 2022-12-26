// <copyright file="CoreListDateTimeConverter.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace APIMatic.Core.Utilities.Date
{

    /// <summary>
    /// Extends from JsonConverter, allows the use of a custom converter.
    /// </summary>
    public class CoreListDateTimeConverter : JsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreListDateTimeConverter"/>
        /// class.
        /// </summary>
        public CoreListDateTimeConverter()
        {
            Converter = new IsoDateTimeConverter();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreListDateTimeConverter"/>
        /// class.
        /// </summary>
        /// <param name="converter">converter.</param>
        public CoreListDateTimeConverter(Type converter)
        {
            Converter = (JsonConverter)Activator.CreateInstance(converter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreListDateTimeConverter"/>
        /// class.
        /// </summary>
        /// <param name="converter">converter.</param>
        /// <param name="format">format.</param>
        public CoreListDateTimeConverter(Type converter, string format)
        {
            Converter = (JsonConverter)Activator.CreateInstance(converter, format);
        }

        /// <summary>
        /// Gets or sets the JsonConverter.
        /// </summary>
        public JsonConverter Converter { get; set; }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Converters.Clear();
            serializer.Converters.Add(Converter);
            serializer.Serialize(writer, value);
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            serializer.Converters.Clear();
            serializer.Converters.Add(Converter);
            return serializer.Deserialize(reader, objectType);
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<DateTime>) || objectType == typeof(DateTime) || objectType == typeof(List<DateTimeOffset>) || objectType == typeof(DateTimeOffset);
        }
    }
}
