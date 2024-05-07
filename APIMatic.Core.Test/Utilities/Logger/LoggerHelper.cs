using System;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace APIMatic.Core.Test.Utilities.Logger
{
    internal static class LoggerHelper
    {
        public static SdkLoggingConfiguration GetLoggingConfiguration(ILogger logger,
            RequestLoggingConfiguration requestLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration
            {
                Logger = logger,
                MaskSensitiveHeaders = true,
                RequestLoggingConfiguration = requestLoggingConfiguration
            };
            return loggingConfiguration;
        }

        public static SdkLoggingConfiguration GetLoggingConfiguration(ILogger logger,
            ResponseLoggingConfiguration responseLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration
            {
                Logger = logger, ResponseLoggingConfiguration = responseLoggingConfiguration
            };
            return loggingConfiguration;
        }

        public static SdkLoggingConfiguration GetLoggingConfigurationWithError(ILogger logger,
            RequestLoggingConfiguration requestLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration
            {
                Logger = logger,
                LogLevel = LogLevel.Error,
                RequestLoggingConfiguration = requestLoggingConfiguration
            };
            return loggingConfiguration;
        }

        public static SdkLoggingConfiguration GetLoggingConfigurationWithoutMask(ILogger logger,
            ResponseLoggingConfiguration responseLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration
            {
                Logger = logger,
                MaskSensitiveHeaders = false,
                ResponseLoggingConfiguration = responseLoggingConfiguration
            };
            return loggingConfiguration;
        }

        public static SdkLoggingConfiguration GetLoggingConfigurationWithNoneLogger()
        {
            var loggingConfiguration = new SdkLoggingConfiguration
            {
                MaskSensitiveHeaders = false
            };
            return loggingConfiguration;
        }

        public static void AssertLogs(Mock<ILogger> logger, LogLevel logLevel, string expectedMessage, int times)
        {
            logger.Verify(l => l.Log(
                    logLevel, It.IsAny<EventId>(), It.Is<object>(state => state.ToString().Equals(expectedMessage)),
                    null, (Func<object, Exception, string>)It.IsAny<object>()),
                Times.Exactly(times));
        }
    }
}
