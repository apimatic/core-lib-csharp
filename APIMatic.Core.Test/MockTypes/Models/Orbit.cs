using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace APIMatic.Core.Test.MockTypes.Models
{
    /// <summary>
    /// Orbit.
    /// </summary>
    public class Orbit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Orbit"/> class.
        /// </summary>
        public Orbit()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Orbit"/> class.
        /// </summary>
        /// <param name="numberOfElectrons">NumberOfElectrons.</param>
        /// <param name="numberOfShells">NumberOfShells.</param>
        public Orbit(
            int numberOfElectrons,
            string numberOfShells = null)
        {
            NumberOfElectrons = numberOfElectrons;
            NumberOfShells = numberOfShells;
        }

        /// <summary>
        /// Gets or sets NumberOfElectrons.
        /// </summary>
        [JsonProperty("NumberOfElectrons")]
        [JsonRequired]
        public int NumberOfElectrons { get; set; }

        /// <summary>
        /// Gets or sets NumberOfShells.
        /// </summary>
        [JsonConverter(typeof(JsonStringConverter), true)]
        [JsonProperty("NumberOfShells")]
        [JsonRequired]
        public string NumberOfShells { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var toStringOutput = new List<string>();

            ToString(toStringOutput);

            return $"Orbit : ({string.Join(", ", toStringOutput)})";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj == this)
            {
                return true;
            }
            return obj is Orbit other && NumberOfElectrons.Equals(other.NumberOfElectrons) &&
                ((NumberOfShells == null && other.NumberOfShells == null) || (NumberOfShells?.Equals(other.NumberOfShells) == true));
        }

        /// <summary>
        /// ToString overload.
        /// </summary>
        /// <param name="toStringOutput">List of strings.</param>
        protected void ToString(List<string> toStringOutput)
        {
            toStringOutput.Add($"this.NumberOfElectrons = {NumberOfElectrons}");
            toStringOutput.Add($"this.NumberOfShells = {NumberOfShells ?? "null"}");
        }
    }
}
