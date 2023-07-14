// <copyright file="Helium.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    /// <summary>
    /// Helium.
    /// </summary>
    public class Helium
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Helium"/> class.
        /// </summary>
        public Helium()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Helium"/> class.
        /// </summary>
        /// <param name="numberOfElectrons">NumberOfElectrons.</param>
        /// <param name="numberOfProtons">NumberOfProtons.</param>
        public Helium(
            int numberOfElectrons,
            int? numberOfProtons = null)
        {
            NumberOfElectrons = numberOfElectrons;
            NumberOfProtons = numberOfProtons;
        }

        /// <summary>
        /// Gets or sets NumberOfElectrons.
        /// </summary>
        [JsonProperty("NumberOfElectrons")]
        [JsonRequired]
        public int NumberOfElectrons { get; set; }

        /// <summary>
        /// Gets or sets NumberOfProtons.
        /// </summary>
        [JsonProperty("NumberOfProtons", NullValueHandling = NullValueHandling.Ignore)]
        [JsonRequired]
        public int? NumberOfProtons { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var toStringOutput = new List<string>();

            ToString(toStringOutput);

            return $"Helium : ({string.Join(", ", toStringOutput)})";
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
            return obj is Helium other && NumberOfElectrons.Equals(other.NumberOfElectrons) &&
                ((NumberOfProtons == null && other.NumberOfProtons == null) || (NumberOfProtons?.Equals(other.NumberOfProtons) == true));
        }

        /// <summary>
        /// ToString overload.
        /// </summary>
        /// <param name="toStringOutput">List of strings.</param>
        protected void ToString(List<string> toStringOutput)
        {
            toStringOutput.Add($"this.NumberOfElectrons = {NumberOfElectrons}");
            toStringOutput.Add($"this.NumberOfProtons = {(NumberOfProtons == null ? "null" : NumberOfProtons.ToString())}");
        }
    }
}
