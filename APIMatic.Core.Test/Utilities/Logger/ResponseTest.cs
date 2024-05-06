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
        private Mock<ILogger> _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger>();
        }
        
        [Test]
        public void LogResponse_NotConfigured_NoLogSent()
        {
            // Arrange
            var loggingConfiguration = LoggerHelper.GetLoggingConfigurationWithNoneLogger();
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, "{\"message\":\"Success\"}");

            // Act
            sdkLogger.LogResponse(response);

            // Assert
            LoggerHelper.AssertLogs(_logger, It.IsAny<LogLevel>(), It.IsAny<string>(), 0);
        }

        [Test]
        public void LogResponse_Configured_LogsResponse()
        {
            // Arrange
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(_logger.Object, responseLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Response 200 (null) (null)",
                1);
        }

        [Test]
        public void LogResponse_Configured_LogsResponseWithBody()
        {
            // Arrange
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder().Body(true);
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(_logger.Object, responseLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, "{\"message\":\"Success\"}");

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Response 200 (null) (null)", 1);
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Response Body {\"message\":\"Success\"}", 1);
        }

        [Test]
        public void LogResponse_Configured_LogsResponseWithHeaders()
        {
            // Arrange
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder().Headers(true);
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(_logger.Object, responseLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Response 200 (null) application/json", 1);
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Response Headers [Content-Type, application/json]",
                1);
        }
    }
}
