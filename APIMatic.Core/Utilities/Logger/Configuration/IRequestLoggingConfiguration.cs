namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents configuration settings for logging HTTP requests.
    /// </summary>
    public interface IRequestLoggingConfiguration : IHttpLoggingConfiguration
    {
        /// <summary>
        /// Gets a value indicating whether to include the query string in the logged path of HTTP requests.
        /// </summary>
        bool IncludeQueryInPath { get; }
    }
}
