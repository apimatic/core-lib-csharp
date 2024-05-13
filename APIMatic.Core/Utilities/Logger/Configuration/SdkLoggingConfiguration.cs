using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for SDK logging.
    /// </summary>
    public class SdkLoggingConfiguration
    {
        /// <summary>
        /// Gets or sets the logger instance used for logging SDK messages.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets or sets the log level for SDK logging.
        /// </summary>
        public LogLevel? LogLevel { get; }

        /// <summary>
        /// Gets or sets whether sensitive headers should be masked in logs.
        /// </summary>
        public bool MaskSensitiveHeaders { get; } 

        /// <summary>
        /// Gets or sets the configuration for request logging.
        /// </summary>
        public RequestLoggingConfiguration RequestLoggingConfiguration { get; }

        /// <summary>
        /// Gets or sets the configuration for response logging.
        /// </summary>
        public ResponseLoggingConfiguration ResponseLoggingConfiguration { get; }

        /// <summary>
        /// Gets a value indicating whether the logging configuration is fully set up.
        /// </summary>
        public bool IsConfigured => Logger != NullLogger.Instance;

        private SdkLoggingConfiguration(ILogger logger, LogLevel? logLevel, bool maskSensitiveHeaders,
            RequestLoggingConfiguration requestLoggingConfiguration,
            ResponseLoggingConfiguration responseLoggingConfiguration)
        {
            Logger = logger;
            LogLevel = logLevel;
            MaskSensitiveHeaders = maskSensitiveHeaders;
            RequestLoggingConfiguration = requestLoggingConfiguration;
            ResponseLoggingConfiguration = responseLoggingConfiguration;
        }
        

        public static SdkLoggingConfiguration Default() =>
            new SdkLoggingConfiguration(NullLogger.Instance, null, true,
                RequestLoggingConfiguration.Default(),
                ResponseLoggingConfiguration.Default());

        public static SdkLoggingConfiguration Console() =>
            new SdkLoggingConfiguration(ConsoleLogger.Instance, null, true,
                RequestLoggingConfiguration.Default(),
                ResponseLoggingConfiguration.Default());


        public static SdkLoggingConfiguration Builder(ILogger logger, LogLevel? logLevel, bool maskSensitiveHeaders,
            RequestLoggingConfiguration requestLoggingConfiguration,
            ResponseLoggingConfiguration responseLoggingConfiguration)
        {
            return new SdkLoggingConfiguration(
                logger ?? ConsoleLogger.Instance,
                logLevel,
                maskSensitiveHeaders,
                requestLoggingConfiguration ?? RequestLoggingConfiguration.Default(),
                responseLoggingConfiguration ?? ResponseLoggingConfiguration.Default());
        }
    }
}
