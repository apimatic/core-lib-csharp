// <copyright file="Atom.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using APIMatic.Core.Utilities.Converters;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    /// <summary>
    /// Atom.
    /// </summary>
    public class Atom
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Atom"/> class.
        /// </summary>
        public Atom()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Atom"/> class.
        /// </summary>
        /// <param name="numberOfElectrons">NumberOfElectrons.</param>
        /// <param name="numberOfProtons">NumberOfProtons.</param>
        public Atom(
            int numberOfElectrons,
            int numberOfProtons,
            string name = null)
        {
            NumberOfElectrons = numberOfElectrons;
            NumberOfProtons = numberOfProtons;
            Name = name;
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
        [JsonProperty("NumberOfProtons")]
        [JsonRequired]
        public int NumberOfProtons { get; set; }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [JsonConverter(typeof(JsonStringConverter))]
        [JsonProperty("Name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var toStringOutput = new List<string>();

            ToString(toStringOutput);

            return $"Atom : ({string.Join(", ", toStringOutput)})";
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
            return obj is Atom other && NumberOfElectrons.Equals(other.NumberOfElectrons) &&
                NumberOfProtons.Equals(other.NumberOfProtons) &&
                ((Name == null && other.Name == null) || (Name?.Equals(other.Name) == true));
        }

        /// <summary>
        /// ToString overload.
        /// </summary>
        /// <param name="toStringOutput">List of strings.</param>
        protected void ToString(List<string> toStringOutput)
        {
            toStringOutput.Add($"this.NumberOfElectrons = {NumberOfElectrons}");
            toStringOutput.Add($"this.NumberOfProtons = {NumberOfProtons}");
            toStringOutput.Add($"this.Name = {Name ?? "null"}");
        }
    }
}
