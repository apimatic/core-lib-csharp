using System;
using APIMatic.Core.Utilities.Converters;

namespace APIMatic.Core.Test.MockTypes.Convertors
{
    /// <summary>
    /// Extends from JsonConverter, allows the use of a custom converter.
    /// </summary>
    public class MapDateTimeConverter : CoreMapConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapDateTimeConverter"/>
        /// class.
        /// </summary>
        public MapDateTimeConverter()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDateTimeConverter"/>
        /// class.
        /// </summary>
        /// <param name="converter">converter.</param>
        public MapDateTimeConverter(Type converter)
            : base(converter) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDateTimeConverter"/>
        /// class.
        /// </summary>
        /// <param name="converter">converter.</param>
        /// <param name="format">format.</param>
        public MapDateTimeConverter(Type converter, string format)
            : base(converter, format) { }
    }
}
