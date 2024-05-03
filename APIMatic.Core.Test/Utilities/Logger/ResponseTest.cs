using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Logger
{
    [TestFixture]
    public class ResponseTest
    {
        [Test]
        public void LogResponse_NotConfigured_NoLogSent()
        {
            // Arrange
            var loggingConfiguration = new Mock<ISdkLoggingConfiguration>();
            loggingConfiguration.SetupGet(c => c.IsConfigured).Returns(false);
            var logger = new Mock<ILogger>();
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration.Object);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, "{\"message\":\"Success\"}");

            // Act
            sdkLogger.LogResponse(response);

            // Assert
            logger.Verify(l => l.Log(
                    It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), null,
                    (Func<object, Exception, string>)It.IsAny<object>()),
                Times.Never);
        }

        [Test]
        public void LogResponse_Configured_LogsResponse()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder().Build();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) (null)",
                1);
        }
        
        [Test]
        public void LogResponse_Configured_LogsResponseWithBody()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder().Body(true).Build();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, "{\"message\":\"Success\"}");

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) (null)",
                1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response Body {\"message\":\"Success\"}",
                1);
        }
        
        [Test]
        public void LogResponse_Configured_LogsResponseWithHeaders()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder().Headers(true).Build();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) application/json",
                1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response Headers [Content-Type, application/json]",
                1);
        }
    }
}
