using System;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace APIMatic.Core.Test.Utilities.Logger
{
    internal static class LoggerHelper
    {
        public static ISdkLoggingConfiguration GetLoggingConfiguration(ILogger logger,
            IRequestLoggingConfiguration requestLoggingConfiguration)
        {
            var loggingConfiguration = new Mock<ISdkLoggingConfiguration>();
            loggingConfiguration.SetupGet(c => c.IsConfigured).Returns(true);
            loggingConfiguration.SetupGet(c => c.Logger).Returns(logger);
            loggingConfiguration.SetupGet(c => c.MaskSensitiveHeaders).Returns(true);
            loggingConfiguration.SetupGet(c => c.RequestLoggingConfiguration).Returns(requestLoggingConfiguration);
            return loggingConfiguration.Object;
        }
        
        public static ISdkLoggingConfiguration GetLoggingConfiguration(ILogger logger,
            IResponseLoggingConfiguration responseLoggingConfiguration)
        {
            var loggingConfiguration = new Mock<ISdkLoggingConfiguration>();
            loggingConfiguration.SetupGet(c => c.IsConfigured).Returns(true);
            loggingConfiguration.SetupGet(c => c.Logger).Returns(logger);
            loggingConfiguration.SetupGet(c => c.MaskSensitiveHeaders).Returns(true);
            loggingConfiguration.SetupGet(c => c.ResponseLoggingConfiguration).Returns(responseLoggingConfiguration);
            return loggingConfiguration.Object;
        }

        public static ISdkLoggingConfiguration GetLoggingConfigurationWithoutMask(ILogger logger,
            IResponseLoggingConfiguration responseLoggingConfiguration)
        {
            var loggingConfiguration = new Mock<ISdkLoggingConfiguration>();
            loggingConfiguration.SetupGet(c => c.IsConfigured).Returns(true);
            loggingConfiguration.SetupGet(c => c.Logger).Returns(logger);
            loggingConfiguration.SetupGet(c => c.MaskSensitiveHeaders).Returns(false);
            loggingConfiguration.SetupGet(c => c.ResponseLoggingConfiguration).Returns(responseLoggingConfiguration);
            return loggingConfiguration.Object;
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
