using APIMatic.Core.Utilities.Converters.Interfaces;

namespace APIMatic.Core.Utilities.Converters
{
    public class StringEnumConverter : Newtonsoft.Json.Converters.StringEnumConverter, IEnumConverter
    {
        public StringEnumConverter() : base(null, false) { }
    }
}
