using System.Collections.Generic;
using System.Net.Http;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities.Logger;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Logger
{
    [TestFixture]
    public class RequestTest
    {
        private Mock<ILogger> _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger>();
        }

        [Test]
        public void LogRequest_NotConfigured_NoLogSent()
        {
            // Arrange
            var loggingConfiguration = LoggerHelper.GetLoggingConfigurationWithNoneLogger();
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource", null, null, null);

            // Act
            sdkLogger.LogRequest(request);

            // Assert
            LoggerHelper.AssertLogs(_logger, It.IsAny<LogLevel>(), It.IsAny<string>(), 0);
        }

        [Test]
        public void LogRequest_Configured_LogsRequest_WithError()
        {
            // Arrange
            var requestLoggingConfiguration = new RequestLoggingConfiguration();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfigurationWithError(_logger.Object, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource",
                new Dictionary<string, string>(), null, null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Error,
                "Request POST https://example.com/api/resource (null)", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithQueryUrl()
        {
            // Arrange
            var requestLoggingConfiguration =
                new RequestLoggingConfiguration { IncludeQueryInPath = true };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(_logger.Object, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post,
                "https://example.com/api/resource?param1=value1&param2=value2", new Dictionary<string, string>(), null,
                null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Information,
                "Request POST https://example.com/api/resource?param1=value1&param2=value2 (null)", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithOutQueryUrl()
        {
            // Arrange
            var requestLoggingConfiguration = new RequestLoggingConfiguration();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(_logger.Object, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post,
                null, new Dictionary<string, string>(), null,
                null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Request POST (null) (null)", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithBody()
        {
            // Arrange
            var requestLoggingConfiguration = new RequestLoggingConfiguration { Body = true };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(_logger.Object, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource",
                new Dictionary<string, string>(), "{'key': 'value'}", null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Information,
                "Request POST https://example.com/api/resource (null)", 1);
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Request Body {'key': 'value'}", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithHeaders()
        {
            // Arrange
            var requestLoggingConfiguration = new RequestLoggingConfiguration { Headers = true };
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(_logger.Object, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource", headers, null, null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(_logger, LogLevel.Information,
                "Request POST https://example.com/api/resource application/json", 1);
            LoggerHelper.AssertLogs(_logger, LogLevel.Information, "Request Headers [Content-Type, application/json]",
                1);
        }
    }
}
