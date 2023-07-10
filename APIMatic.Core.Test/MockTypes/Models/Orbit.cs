using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Test.MockTypes.Models
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

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
            int? numberOfShells = null)
        {
            this.NumberOfElectrons = numberOfElectrons;
            this.NumberOfShells = numberOfShells;
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
        [JsonProperty("NumberOfShells", NullValueHandling = NullValueHandling.Ignore)]
        [JsonRequired]
        public int? NumberOfShells { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var toStringOutput = new List<string>();

            this.ToString(toStringOutput);

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
            return obj is Orbit other && this.NumberOfElectrons.Equals(other.NumberOfElectrons) &&
                ((this.NumberOfShells == null && other.NumberOfShells == null) || (this.NumberOfShells?.Equals(other.NumberOfShells) == true));
        }

        /// <summary>
        /// ToString overload.
        /// </summary>
        /// <param name="toStringOutput">List of strings.</param>
        protected void ToString(List<string> toStringOutput)
        {
            toStringOutput.Add($"this.NumberOfElectrons = {this.NumberOfElectrons}");
            toStringOutput.Add($"this.NumberOfShells = {(this.NumberOfShells == null ? "null" : this.NumberOfShells.ToString())}");
        }
    }
}
