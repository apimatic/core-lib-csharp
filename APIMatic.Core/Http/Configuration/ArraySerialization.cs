// <copyright file="ArrayDeserialization.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
namespace APIMatic.Core.Http.Configuration
{
    /// <summary>
    /// This enumeration has ArrayDeserialization format.
    /// </summary>
    public enum ArraySerialization
    {
        /// <summary>
        /// Example: variableName[0] = value1
        /// </summary>
        Indexed = 0,

        /// <summary>
        /// Example: variableName[] = value1
        /// </summary>
        UnIndexed = 1,

        /// <summary>
        /// Example: variableName = value1, variableName = value 2
        /// </summary>
        Plain = 2,

        /// <summary>
        /// Example: variableName = value1,value2
        /// </summary>
        CSV = 3,

        /// <summary>
        /// Example: variableName = value1\tvalue2
        /// </summary>
        TSV = 4,

        /// <summary>
        /// Example: variableName = value1|value2
        /// </summary>
        PSV = 5,

        /// <summary>
        /// Example: Ignore format
        /// </summary>
        None = 6,
    }
}
