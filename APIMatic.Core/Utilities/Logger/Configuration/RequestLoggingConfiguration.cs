namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for logging HTTP requests.
    /// </summary>
    public class RequestLoggingConfiguration : HttpLoggingConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether to include the query string in the logged path of HTTP requests.
        /// </summary>
        public bool IncludeQueryInPath { get; set; }
    }
}
