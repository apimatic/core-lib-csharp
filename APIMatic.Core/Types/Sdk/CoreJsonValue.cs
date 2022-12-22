// <copyright file="CoreJsonValue.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using Newtonsoft.Json;

namespace APIMatic.Core.Types.Sdk
{
    /// <summary>
    /// This is the base class for JsonValue
    /// </summary>
    public class CoreJsonValue
    {
        private readonly object value;

        protected CoreJsonValue(object value)
        {
            this.value = value;
        }

        /// <summary>
        /// Getter for the stored object.
        /// </summary>
        public object GetStoredObject() => value;

        /// <summary>
        /// Converts current value into string.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(value, Formatting.None);
        }
    }
}
