﻿// <copyright file="CoreCustomDateTimeConverter.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using Newtonsoft.Json.Converters;

namespace APIMatic.Core.Utilities.Date
{
    /// <summary>
    /// Extends from IsoDateTimeConverter to allow a custom DateTime format.
    /// </summary>
    public class CoreCustomDateTimeConverter : IsoDateTimeConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreCustomDateTimeConverter"/>
        /// class.
        /// </summary>
        /// <param name="format">format.</param>
        public CoreCustomDateTimeConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
