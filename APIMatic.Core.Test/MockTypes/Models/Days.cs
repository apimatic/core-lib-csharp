using System.Runtime.Serialization;
using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Days
    {
        [EnumMember(Value = "Monday")]
        Monday,

        [EnumMember(Value = "Tuesday")]
        Tuesday,

        [EnumMember(Value = "Wednesday")]
        Wednesday,

        [EnumMember(Value = "Thursday")]
        Thursday,

        [EnumMember(Value = "Friday")]
        Friday,

        [EnumMember(Value = "Saturday")]
        Saturday,

        [EnumMember(Value = "Sunday")]
        Sunday
    }
}
