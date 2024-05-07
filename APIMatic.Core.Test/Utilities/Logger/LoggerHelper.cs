using System.Linq;
using APIMatic.Core.Test.MockTypes.Utilities;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

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
            var loggingConfiguration = new SdkLoggingConfiguration { MaskSensitiveHeaders = false };
            return loggingConfiguration;
        }

        public static void AssertLogs(TestLogger logger, LogLevel logLevel, string expectedMessage, int times)
        {
            var filteredMessages = logger.LoggedMessages.Where(logEntry =>
                logEntry.Message.Contains(expectedMessage) && logEntry.LogLevel.Equals(logLevel));

            if (times == 0)
            {
                Assert.IsFalse(filteredMessages.Any());
            }
            else
            {
                Assert.AreEqual(times, filteredMessages.Count());
            }
        }
    }
}
