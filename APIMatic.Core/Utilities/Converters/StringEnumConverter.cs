namespace APIMatic.Core.Utilities.Converters
{
    public class StringEnumConverter : Newtonsoft.Json.Converters.StringEnumConverter
    {
        public StringEnumConverter() : base(null, false) { }
    }
}
