using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Days
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
