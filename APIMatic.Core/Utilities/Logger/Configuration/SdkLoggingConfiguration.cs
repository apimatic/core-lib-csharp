using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for SDK logging.
    /// </summary>
    public class SdkLoggingConfiguration : ICloneable
    {
        /// <summary>
        /// Gets or sets the logger instance used for logging SDK messages.
        /// </summary>
        public ILogger Logger { get; set; } = NullLogger.Instance;

        /// <summary>
        /// Gets or sets the log level for SDK logging.
        /// </summary>
        public LogLevel? LogLevel { get; set; }

        /// <summary>
        /// Gets or sets whether sensitive headers should be masked in logs.
        /// </summary>
        public bool MaskSensitiveHeaders { get; set; } = true;

        /// <summary>
        /// Gets or sets the configuration for request logging.
        /// </summary>
        public RequestLoggingConfiguration RequestLoggingConfiguration { get; set; } =
            new RequestLoggingConfiguration();

        /// <summary>
        /// Gets or sets the configuration for response logging.
        /// </summary>
        public ResponseLoggingConfiguration ResponseLoggingConfiguration { get; set; } =
            new ResponseLoggingConfiguration();

        /// <summary>
        /// Gets a value indicating whether the logging configuration is fully set up.
        /// </summary>
        public bool IsConfigured => Logger != NullLogger.Instance;

        /// <inheritdoc/>
        public object Clone()
        {
            return new SdkLoggingConfiguration
            {
                Logger = this.Logger,
                LogLevel = this.LogLevel,
                MaskSensitiveHeaders = this.MaskSensitiveHeaders,
                RequestLoggingConfiguration = (RequestLoggingConfiguration)this.RequestLoggingConfiguration.Clone(),
                ResponseLoggingConfiguration = (ResponseLoggingConfiguration)this.ResponseLoggingConfiguration.Clone()
            };
        }
    }
}
