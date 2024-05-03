using System;
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
        [Test]
        public void LogRequest_NotConfigured_NoLogSent()
        {
            // Arrange
            var loggingConfiguration = new Mock<ISdkLoggingConfiguration>();
            loggingConfiguration.SetupGet(c => c.IsConfigured).Returns(false);
            var logger = new Mock<ILogger>();
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration.Object);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource", null, null, null);

            // Act
            sdkLogger.LogRequest(request);

            // Assert
            logger.Verify(l => l.Log(
                    It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), null,
                    (Func<object, Exception, string>)It.IsAny<object>()),
                Times.Never);
        }

        [Test]
        public void LogRequest_Configured_LogsRequest()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var requestLoggingConfiguration = new RequestLoggingConfiguration.Builder().Build();
            var loggingConfiguration = LoggerHelper.GetLoggingConfiguration(logger.Object, requestLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource",
                new Dictionary<string, string>(), null, null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Request POST https://example.com/api/resource (null)",
                1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithQueryUrl()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var requestLoggingConfiguration =
                new RequestLoggingConfiguration.Builder().IncludeQueryInPath(true).Build();
            var loggingConfiguration = LoggerHelper.GetLoggingConfiguration(logger.Object, requestLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post,
                "https://example.com/api/resource?param1=value1&param2=value2", new Dictionary<string, string>(), null,
                null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Request POST https://example.com/api/resource?param1=value1&param2=value2 (null)", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithOutQueryUrl()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var requestLoggingConfiguration =
                new RequestLoggingConfiguration.Builder().Build();
            var loggingConfiguration = LoggerHelper.GetLoggingConfiguration(logger.Object, requestLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post,
                null, new Dictionary<string, string>(), null,
                null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Request POST (null) (null)", 1);
        }
        
        [Test]
        public void LogRequest_Configured_LogsRequestWithBody()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var requestLoggingConfiguration = new RequestLoggingConfiguration.Builder().Body(true).Build();
            var loggingConfiguration = LoggerHelper.GetLoggingConfiguration(logger.Object, requestLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource",
                new Dictionary<string, string>(), "{'key': 'value'}", null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Request POST https://example.com/api/resource (null)", 1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Request Body {'key': 'value'}", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithHeaders()
        {
            // Arrange
            var logger = new Mock<ILogger>();
            var requestLoggingConfiguration = new RequestLoggingConfiguration.Builder().Headers(true).Build();
            var loggingConfiguration = LoggerHelper.GetLoggingConfiguration(logger.Object, requestLoggingConfiguration);
            var sdkLogger = new Core.Utilities.Logger.SdkLogger(loggingConfiguration);
            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource", headers, null, null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Request POST https://example.com/api/resource application/json", 1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Request Headers [Content-Type, application/json]",
                1);
        }
    }
}
