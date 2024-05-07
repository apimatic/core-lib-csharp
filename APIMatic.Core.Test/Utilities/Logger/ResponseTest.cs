using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Utilities;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities.Logger;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
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
            var logger = new TestLogger();
            var loggingConfiguration = LoggerHelper.GetLoggingConfigurationWithNoneLogger();
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, "{\"message\":\"Success\"}");

            // Act
            sdkLogger.LogResponse(response);

            // Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Trace, string.Empty, 0);
        }

        [Test]
        public void LogResponse_Configured_LogsResponse()
        {
            // Arrange
            var logger = new TestLogger();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
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
            var logger = new TestLogger();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration { Body = true };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var response = new CoreResponse(200, new Dictionary<string, string>(), null, "{\"message\":\"Success\"}");

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) (null)", 1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response Body {\"message\":\"Success\"}", 1);
        }

        [Test]
        public void LogResponse_Configured_LogsResponseWithHeaders()
        {
            // Arrange
            var logger = new TestLogger();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration { Headers = true };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) application/json", 1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response Headers [Content-Type, application/json]",
                1);
        }
    }
}
