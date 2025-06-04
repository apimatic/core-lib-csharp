using System.Collections.Generic;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.MockTypes.Models.Pagination
{
    public class TransactionsLinked
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsLinked"/> class.
        /// </summary>
        /// <param name="data">data.</param>
        /// <param name="links">links.</param>
        public TransactionsLinked(
            List<Transaction> data = null,
            Links links = null)
        {
            this.Data = data;
            this.Links = links;
        }

        /// <summary>
        /// Gets or sets Data.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<Transaction> Data { get; set; }

        /// <summary>
        /// Gets or sets Links.
        /// </summary>
        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public Links Links { get; set; }
    }

    public class Links
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Links"/> class.
        /// </summary>
        /// <param name="first">first.</param>
        /// <param name="last">last.</param>
        /// <param name="prev">prev.</param>
        /// <param name="next">next.</param>
        public Links(
            string first = null,
            string last = null,
            string prev = null,
            string next = null)
        {
            this.First = first;
            this.Last = last;
            this.Prev = prev;
            this.Next = next;
        }

        /// <summary>
        /// Gets or sets First.
        /// </summary>
        [JsonProperty("first", NullValueHandling = NullValueHandling.Ignore)]
        public string First { get; set; }

        /// <summary>
        /// Gets or sets Last.
        /// </summary>
        [JsonProperty("last", NullValueHandling = NullValueHandling.Ignore)]
        public string Last { get; set; }

        /// <summary>
        /// Gets or sets Prev.
        /// </summary>
        [JsonProperty("prev", NullValueHandling = NullValueHandling.Ignore)]
        public string Prev { get; set; }

        /// <summary>
        /// Gets or sets Next.
        /// </summary>
        [JsonProperty("next", NullValueHandling = NullValueHandling.Ignore)]
        public string Next { get; set; }
    }
}
