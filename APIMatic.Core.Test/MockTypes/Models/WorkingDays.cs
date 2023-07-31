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
}
