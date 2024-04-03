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
        [EnumMember(Value = "Mon")]
        Monday,

        [EnumMember(Value = "Tue")]
        Tuesday,

        [EnumMember(Value = "Wed")]
        Wednesday,

        [EnumMember(Value = "Thu")]
        Thursday,

        [EnumMember(Value = "Fri")]
        Friday,

        [EnumMember(Value = "Sat")]
        Saturday,

        [EnumMember(Value = "Sun")]
        Sunday,
        _Unknown
    }
}
