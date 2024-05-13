using System.Collections.Generic;
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
            return GetSdkLoggingConfiguration(logger: logger, maskSensitiveHeaders: true,
                requestLoggingConfiguration: requestLoggingConfiguration);
        }

        public static SdkLoggingConfiguration GetSdkLoggingConfiguration(ILogger logger = null,
            LogLevel? logLevel = null, bool maskSensitiveHeaders = true,
            RequestLoggingConfiguration requestLoggingConfiguration = null,
            ResponseLoggingConfiguration responseLoggingConfiguration = null) => SdkLoggingConfiguration.Builder(logger,
            logLevel, maskSensitiveHeaders, requestLoggingConfiguration, responseLoggingConfiguration);

        public static RequestLoggingConfiguration GetRequestLoggingConfiguration(bool body = false,
            bool headers = false, bool includeQueryInPath = false, IReadOnlyCollection<string> headersToInclude = null,
            IReadOnlyCollection<string> headersToExclude = null, IReadOnlyCollection<string> headersToUnmask = null) =>
            RequestLoggingConfiguration.Builder(body, headers, includeQueryInPath, headersToInclude, headersToExclude,
                headersToUnmask);

        public static ResponseLoggingConfiguration GetResponseLoggingConfiguration(bool body = false,
            bool headers = false, IReadOnlyCollection<string> headersToInclude = null,
            IReadOnlyCollection<string> headersToExclude = null, IReadOnlyCollection<string> headersToUnmask = null) =>
            ResponseLoggingConfiguration.Builder(body, headers, headersToInclude, headersToExclude, headersToUnmask);

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
