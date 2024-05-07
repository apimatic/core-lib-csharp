using System;
using System.Collections.Generic;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for logging HTTP responses.
    /// </summary>
    public class ResponseLoggingConfiguration : HttpLoggingConfiguration, ICloneable
    {
        public object Clone()
        {
            return new ResponseLoggingConfiguration
            {
                Body = this.Body,
                Headers = this.Headers,
                HeadersToInclude = new List<string>(this.HeadersToInclude).AsReadOnly(),
                HeadersToExclude = new List<string>(this.HeadersToExclude).AsReadOnly(),
                HeadersToUnmask = new List<string>(this.HeadersToUnmask).AsReadOnly()
            };
        }
    }
}
