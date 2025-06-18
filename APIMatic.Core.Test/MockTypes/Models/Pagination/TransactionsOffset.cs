using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models.Pagination
{
    public class TransactionsOffset
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsOffset"/> class.
        /// </summary>
        /// <param name="data">data.</param>
        public TransactionsOffset(
            List<Transaction> data = null)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets Data.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<Transaction> Data { get; set; }
    }
}
