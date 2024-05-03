using Microsoft.Extensions.Logging;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents configuration settings for SDK logging.
    /// </summary>
    public interface ISdkLoggingConfiguration
    {
        /// <summary>
        /// Gets the logger instance for SDK logging.
        /// </summary>
        ILogger Logger { get; }
        
        /// <summary>
        /// Gets the log level for SDK logging, or null if not explicitly set.
        /// </summary>
        LogLevel? LogLevel { get; }
        
        /// <summary>
        /// Gets a value indicating whether the SDK logging is configured.
        /// </summary>
        bool IsConfigured { get; }
        
        /// <summary>
        /// Gets a value indicating whether sensitive headers should be masked in the log output.
        /// </summary>
        bool MaskSensitiveHeaders { get; }
        
        /// <summary>
        /// Gets the configuration settings for logging HTTP requests.
        /// </summary>
        IRequestLoggingConfiguration RequestLoggingConfiguration { get; }
        
        /// <summary>
        /// Gets the configuration settings for logging HTTP responses.
        /// </summary>
        IResponseLoggingConfiguration ResponseLoggingConfiguration { get; }
    }
}
