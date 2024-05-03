using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    /// <summary>
    /// Represents the configuration settings for SDK logging.
    /// </summary>
    public class SdkLoggingConfiguration : ISdkLoggingConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdkLoggingConfiguration"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for SDK logging.</param>
        /// <param name="logLevel">The log level for SDK logging, or null if not explicitly set.</param>
        /// <param name="maskSensitiveHeaders">A value indicating whether sensitive headers should be masked in the
        /// log output.</param>
        /// <param name="requestLoggingConfiguration">The configuration settings for logging HTTP requests.</param>
        /// <param name="responseLoggingConfiguration">The configuration settings for logging HTTP responses.</param>
        private SdkLoggingConfiguration(ILogger logger, LogLevel? logLevel, bool maskSensitiveHeaders,
            IRequestLoggingConfiguration requestLoggingConfiguration,
            IResponseLoggingConfiguration responseLoggingConfiguration)
        {
            Logger = logger;
            LogLevel = logLevel;
            MaskSensitiveHeaders = maskSensitiveHeaders;
            RequestLoggingConfiguration = requestLoggingConfiguration;
            ResponseLoggingConfiguration = responseLoggingConfiguration;
        }

        /// <inheritdoc/>
        public ILogger Logger { get; }

        /// <inheritdoc/>
        public LogLevel? LogLevel { get; }

        /// <inheritdoc/>
        public bool MaskSensitiveHeaders { get; }

        /// <inheritdoc/>
        public IRequestLoggingConfiguration RequestLoggingConfiguration { get; }

        /// <inheritdoc/>
        public IResponseLoggingConfiguration ResponseLoggingConfiguration { get; }

        /// <inheritdoc/>
        public bool IsConfigured => Logger != NullLogger.Instance;

        /// <inheritdoc/>
        public override string ToString()
        {
            return "SdkLoggingConfiguration: " +
                   $"{Logger} , " +
                   $"{LogLevel} , " +
                   $"{IsConfigured} , " +
                   $"{MaskSensitiveHeaders} , " +
                   $"{RequestLoggingConfiguration} , " +
                   $"{ResponseLoggingConfiguration} ";
        }

        /// <summary>
        /// Returns a builder instance to modify this <see cref="SdkLoggingConfiguration"/>.
        /// </summary>
        /// <returns>A builder instance to modify this <see cref="SdkLoggingConfiguration"/>.</returns>
        public Builder ToBuilder()
        {
            var builder = new Builder()
                .Logger(Logger)
                .LogLevel(LogLevel)
                .MaskSensitiveHeaders(MaskSensitiveHeaders)
                .RequestLoggingConfiguration(((RequestLoggingConfiguration)RequestLoggingConfiguration).ToBuilder())
                .ResponseLoggingConfiguration(((ResponseLoggingConfiguration)ResponseLoggingConfiguration).ToBuilder());

            return builder;
        }

        /// <summary>
        /// Represents a builder for constructing <see cref="SdkLoggingConfiguration"/>.
        /// </summary>
        public class Builder
        {
            private ILogger _logger = NullLogger.Instance;
            private LogLevel? _logLevel;
            private bool _maskSensitiveHeaders = true;

            private RequestLoggingConfiguration.Builder _requestLoggingConfiguration =
                new RequestLoggingConfiguration.Builder();

            private ResponseLoggingConfiguration.Builder _responseLoggingConfiguration =
                new ResponseLoggingConfiguration.Builder();

            /// <summary>
            /// Sets the logger for SDK logging.
            /// </summary>
            /// <param name="logger">The logger instance.</param>
            /// <returns>The current builder instance.</returns>
            public Builder Logger(ILogger logger)
            {
                _logger = logger;
                return this;
            }

            /// <summary>
            /// Sets the log level for SDK logging.
            /// </summary>
            /// <param name="logLevel">The log level.</param>
            /// <returns>The current builder instance.</returns>
            public Builder LogLevel(LogLevel? logLevel)
            {
                _logLevel = logLevel;
                return this;
            }

            /// <summary>
            /// Sets whether sensitive headers should be masked in the log output.
            /// </summary>
            /// <param name="maskSensitiveHeaders">A value indicating whether to mask sensitive headers.</param>
            /// <returns>The current builder instance.</returns>
            public Builder MaskSensitiveHeaders(bool maskSensitiveHeaders)
            {
                _maskSensitiveHeaders = maskSensitiveHeaders;
                return this;
            }

            /// <summary>
            /// Sets the configuration for logging HTTP requests.
            /// </summary>
            /// <param name="requestLoggingConfiguration">The request logging configuration.</param>
            /// <returns>The current builder instance.</returns>
            public Builder RequestLoggingConfiguration(RequestLoggingConfiguration.Builder requestLoggingConfiguration)
            {
                _requestLoggingConfiguration = requestLoggingConfiguration;
                return this;
            }

            /// <summary>
            /// Sets the configuration for logging HTTP responses.
            /// </summary>
            /// <param name="responseLoggingConfiguration">The response logging configuration.</param>
            /// <returns>The current builder instance.</returns>
            public Builder ResponseLoggingConfiguration(
                ResponseLoggingConfiguration.Builder responseLoggingConfiguration)
            {
                _responseLoggingConfiguration = responseLoggingConfiguration;
                return this;
            }

            /// <summary>
            /// Constructs an instance of <see cref="SdkLoggingConfiguration"/> based on the builder configuration.
            /// </summary>
            /// <returns>An instance of <see cref="SdkLoggingConfiguration"/>.</returns>
            public SdkLoggingConfiguration Build()
            {
                return new SdkLoggingConfiguration(_logger, _logLevel, _maskSensitiveHeaders,
                    _requestLoggingConfiguration.Build(), _responseLoggingConfiguration.Build());
            }
        }
    }
}
