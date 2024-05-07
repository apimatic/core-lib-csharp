using System.Collections.Generic;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities.Logger;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Logger
{
    [TestFixture]
    public class FilterHeadersTest
    {
        [Test]
        public void LogResponse_Configured_LogsResponseWithSensitiveInformation()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration { Headers = true };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }, { "Authorization", "8491ea71-a6e9-499e-84e2-a00946b1995e" }
            };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) application/json", 1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Response Headers [Content-Type, application/json], [Authorization, **Redacted**]", 1);
        }

        [Test]
        public void LogResponse_Configured_LogsResponseWithIncludeHeaders()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration
            {
                Headers = true, HeadersToInclude = new List<string> { "content-type" }
            };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }, { "Authorization", "8491ea71-a6e9-499e-84e2-a00946b1995e" }
            };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) application/json",
                1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Response Headers [Content-Type, application/json]", 1);
        }

        [Test]
        public void LogResponse_Configured_LogsResponseWithExcludeHeaders()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration
            {
                Headers = true, HeadersToExclude = new List<string> { "authorization" }
            };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }, { "Authorization", "8491ea71-a6e9-499e-84e2-a00946b1995e" }
            };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) application/json",
                1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Response Headers [Content-Type, application/json]", 1);
        }

        [Test]
        public void LogResponse_Configured_LogsResponseWithUnmask()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration
            {
                Headers = true,
                HeadersToInclude = new List<string> { "authorization" },
                HeadersToUnmask = new List<string> { "authorization" }
            };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }, { "Authorization", "8491ea71-a6e9-499e-84e2-a00946b1995e" }
            };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) application/json",
                1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Response Headers [Authorization, 8491ea71-a6e9-499e-84e2-a00946b1995e]", 1);
        }

        [Test]
        public void LogResponse_Configured_LogsResponseWithGlobalUnmask()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var responseLoggingConfiguration = new ResponseLoggingConfiguration { Headers = true };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfigurationWithoutMask(logger.Object, responseLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" }, { "Authorization", "8491ea71-a6e9-499e-84e2-a00946b1995e" }
            };
            var response = new CoreResponse(200, headers, null, null);

            // Act
            sdkLogger.LogResponse(response);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Response 200 (null) application/json",
                1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Response Headers [Content-Type, application/json], [Authorization, 8491ea71-a6e9-499e-84e2-a00946b1995e]",
                1);
        }
    }
}
