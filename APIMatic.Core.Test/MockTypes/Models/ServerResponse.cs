using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models
{
    internal class ServerResponse : BaseModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerResponse"/> class.
        /// </summary>
        public ServerResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerResponse"/> class.
        /// </summary>
        /// <param name="passed">passed.</param>
        /// <param name="message">Message.</param>
        /// <param name="input">input.</param>
        public ServerResponse(
            bool passed,
            string message = null,
            object input = null)
        {
            Passed = passed;
            Message = message;
            Input = input;
        }

        /// <summary>
        /// Gets or sets Passed.
        /// </summary>
        [JsonProperty("passed")]
        public bool Passed { get; set; }

        /// <summary>
        /// Gets or sets Message.
        /// </summary>
        [JsonProperty("Message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets Input.
        /// </summary>
        [JsonProperty("input", NullValueHandling = NullValueHandling.Ignore)]
        public object Input { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            var toStringOutput = new List<string>();

            ToString(toStringOutput);

            return $"ServerResponse : ({string.Join(", ", toStringOutput)})";
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

            return obj is ServerResponse other &&
                Passed.Equals(other.Passed) &&
                ((Message == null && other.Message == null) || (Message?.Equals(other.Message) == true)) &&
                ((Input == null && other.Input == null) || (Input?.Equals(other.Input) == true));
        }


        /// <summary>
        /// ToString overload.
        /// </summary>
        /// <param name="toStringOutput">List of strings.</param>
        protected new void ToString(List<string> toStringOutput)
        {
            toStringOutput.Add($"this.Passed = {Passed}");
            toStringOutput.Add($"this.Message = {(Message == null ? "null" : Message == string.Empty ? "" : Message)}");
            toStringOutput.Add($"Input = {(Input == null ? "null" : Input.ToString())}");

            base.ToString(toStringOutput);
        }
    }
}
