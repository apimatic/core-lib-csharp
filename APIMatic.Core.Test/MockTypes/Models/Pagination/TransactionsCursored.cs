using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models.Pagination
{
    public class TransactionsCursored
    {
        private string nextCursor;
        private readonly Dictionary<string, bool> shouldSerialize = new Dictionary<string, bool>
        {
            { "nextCursor", false },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsCursored"/> class.
        /// </summary>
        /// <param name="data">data.</param>
        /// <param name="nextCursor">nextCursor.</param>
        public TransactionsCursored(
            List<Transaction> data = null,
            string nextCursor = null)
        {
            this.Data = data;

            if (nextCursor != null)
            {
                this.NextCursor = nextCursor;
            }
        }

        /// <summary>
        /// Gets or sets Data.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<Transaction> Data { get; set; }

        /// <summary>
        /// Cursor for the next page of results.
        /// </summary>
        [JsonProperty("nextCursor")]
        public string NextCursor
        {
            get
            {
                return this.nextCursor;
            }

            set
            {
                this.shouldSerialize["nextCursor"] = true;
                this.nextCursor = value;
            }
        }

        /// <summary>
        /// Marks the field to not be serialized.
        /// </summary>
        public void UnsetNextCursor()
        {
            this.shouldSerialize["nextCursor"] = false;
        }

        /// <summary>
        /// Checks if the field should be serialized or not.
        /// </summary>
        /// <returns>A boolean weather the field should be serialized or not.</returns>
        public bool ShouldSerializeNextCursor()
        {
            return this.shouldSerialize["nextCursor"];
        }
    }
}
