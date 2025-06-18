using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    public class Person
    {
        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("age")]
        public int Age { get; init; }
    }
}
