using System;
using System.Collections.Generic;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for logging HTTP requests.
    /// </summary>
    public class RequestLoggingConfiguration : HttpLoggingConfiguration, ICloneable
    {
        /// <summary>
        /// Gets or sets a value indicating whether to include the query string in the logged path of HTTP requests.
        /// </summary>
        public bool IncludeQueryInPath { get; set; }

        public object Clone()
        {
            return new RequestLoggingConfiguration
            {
                Body = this.Body,
                Headers = this.Headers,
                IncludeQueryInPath = this.IncludeQueryInPath,
                HeadersToInclude = new List<string>(this.HeadersToInclude).AsReadOnly(),
                HeadersToExclude = new List<string>(this.HeadersToExclude).AsReadOnly(),
                HeadersToUnmask = new List<string>(this.HeadersToUnmask).AsReadOnly()
            };
        }
    }
}
