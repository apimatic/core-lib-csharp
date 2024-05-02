using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger.Configuration
{
    public class SdkLoggingConfiguration : ISdkLoggingConfiguration
    {
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

        public ILogger Logger { get; }

        public LogLevel? LogLevel { get; }

        public bool MaskSensitiveHeaders { get; }

        public IRequestLoggingConfiguration RequestLoggingConfiguration { get; }

        public IResponseLoggingConfiguration ResponseLoggingConfiguration { get; }

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

        public class Builder
        {
            private ILogger _logger = NullLogger.Instance;
            private LogLevel? _logLevel;
            private bool _maskSensitiveHeaders = true;

            private RequestLoggingConfiguration.Builder _requestLoggingConfiguration =
                new RequestLoggingConfiguration.Builder();

            private ResponseLoggingConfiguration.Builder _responseLoggingConfiguration =
                new ResponseLoggingConfiguration.Builder();

            public Builder Logger(ILogger logger)
            {
                _logger = logger;
                return this;
            }

            public Builder LogLevel(LogLevel? logLevel)
            {
                _logLevel = logLevel;
                return this;
            }

            public Builder MaskSensitiveHeaders(bool maskSensitiveHeaders)
            {
                _maskSensitiveHeaders = maskSensitiveHeaders;
                return this;
            }

            public Builder RequestLoggingConfiguration(RequestLoggingConfiguration.Builder requestLoggingConfiguration)
            {
                _requestLoggingConfiguration = requestLoggingConfiguration;
                return this;
            }

            public Builder ResponseLoggingConfiguration(
                ResponseLoggingConfiguration.Builder responseLoggingConfiguration)
            {
                _responseLoggingConfiguration = responseLoggingConfiguration;
                return this;
            }

            public SdkLoggingConfiguration Build()
            {
                return new SdkLoggingConfiguration(_logger, _logLevel, _maskSensitiveHeaders,
                    _requestLoggingConfiguration.Build(), _responseLoggingConfiguration.Build());
            }
        }
    }
}
