using System;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace APIMatic.Core.Test.Utilities.Logger
{
    internal static class LoggerHelper
    {
        public static SdkLoggingConfiguration GetLoggingConfiguration(ILogger logger,
            RequestLoggingConfiguration.Builder requestLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration.Builder()
                .Logger(logger)
                .MaskSensitiveHeaders(true)
                .RequestLoggingConfiguration(requestLoggingConfiguration).Build();
            return loggingConfiguration;
        }

        public static SdkLoggingConfiguration GetLoggingConfiguration(ILogger logger,
            ResponseLoggingConfiguration.Builder responseLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration.Builder()
                .Logger(logger)
                .ResponseLoggingConfiguration(responseLoggingConfiguration).Build();
            return loggingConfiguration;
        }
        
        public static SdkLoggingConfiguration GetLoggingConfigurationWithError(ILogger logger,
            RequestLoggingConfiguration.Builder requestLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration.Builder()
                .Logger(logger)
                .LogLevel(LogLevel.Error)
                .RequestLoggingConfiguration(requestLoggingConfiguration).Build();
            return loggingConfiguration;
        }

        public static SdkLoggingConfiguration GetLoggingConfigurationWithoutMask(ILogger logger,
            ResponseLoggingConfiguration.Builder responseLoggingConfiguration)
        {
            var loggingConfiguration = new SdkLoggingConfiguration.Builder()
                .Logger(logger)
                .MaskSensitiveHeaders(false)
                .ResponseLoggingConfiguration(responseLoggingConfiguration).Build();
            return loggingConfiguration;
        }
        
        public static SdkLoggingConfiguration GetLoggingConfigurationWithNoneLogger()
        {
            var loggingConfiguration = new SdkLoggingConfiguration.Builder()
                .Logger(NullLogger.Instance)
                .MaskSensitiveHeaders(false).Build();
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
