using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WorkingDays
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }

    [JsonConverter(typeof(UnknownEnumConverter<StringEnumConverter>), nameof(_Unknown))]
    public enum WorkingDaysAllowAdditionalValues
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        _Unknown
    }
}
