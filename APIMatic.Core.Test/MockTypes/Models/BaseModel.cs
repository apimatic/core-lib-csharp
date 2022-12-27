using System.Collections.Generic;
using System.Text.Json.Serialization;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Test.MockTypes.Models
{
    /// <summary>
    /// BaseModel.
    /// </summary>
    internal class BaseModel
    {
        /// <summary>
        /// Gets or sets a dictionary holding all the additional properties.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalProperties { get; set; }

        /// <summary>
        /// ToString overload.
        /// </summary>
        /// <param name="toStringOutput">List of strings.</param>
        protected void ToString(List<string> toStringOutput)
        {
            toStringOutput.Add($"Additional Properties: {CoreHelper.JsonSerialize(AdditionalProperties)}");
        }
    }
}
