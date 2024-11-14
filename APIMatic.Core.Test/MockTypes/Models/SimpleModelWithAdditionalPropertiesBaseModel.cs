using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    public class SimpleModelWithAdditionalPropertiesBaseModel : AdditionalPropertiesBaseModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleModelWithAdditionalPropertiesBaseModel"/> class.
        /// </summary>
        /// <param name="requiredProperty">requiredProperty.</param>
        public SimpleModelWithAdditionalPropertiesBaseModel(
            string requiredProperty)
        {
            this.RequiredProperty = requiredProperty;
        }

        /// <summary>
        /// The required property
        /// </summary>
        [JsonProperty("requiredProperty")]
        public string RequiredProperty { get; set; }
    }

    /// <summary>
    /// BaseModel.
    /// </summary>
    public class AdditionalPropertiesBaseModel
    {
        /// <summary>
        /// Gets or sets a dictionary holding all the additional properties.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }
    }
}
