using System.Collections.Generic;
using System.Net.Http;
using APIMatic.Core.Test.MockTypes.Utilities;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities.Logger;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
            var logger = new TestLogger();
            var loggingConfiguration = LoggerHelper.GetSdkLoggingConfiguration(logger: NullLogger.Instance);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource", null, null, null);

            // Act
            sdkLogger.LogRequest(request);

            // Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Trace, string.Empty, 0);
        }

        [Test]
        public void LogRequest_Configured_LogsRequest_WithError()
        {
            // Arrange
            var logger = new TestLogger();
            var requestLoggingConfiguration = LoggerHelper.GetRequestLoggingConfiguration();
            var loggingConfiguration = LoggerHelper.GetSdkLoggingConfiguration(logger: logger, logLevel: LogLevel.Error,
                requestLoggingConfiguration: requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource",
                new Dictionary<string, string>(), null, null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Error,
                "Request POST https://example.com/api/resource (null)", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithQueryUrl()
        {
            // Arrange
            var logger = new TestLogger();
            var requestLoggingConfiguration = LoggerHelper.GetRequestLoggingConfiguration(includeQueryInPath: true);
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
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
            var logger = new TestLogger();
            var requestLoggingConfiguration = LoggerHelper.GetRequestLoggingConfiguration();
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post,
                null, new Dictionary<string, string>(), null,
                null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Request POST (null) (null)", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithBody()
        {
            // Arrange
            var logger = new TestLogger();
            var requestLoggingConfiguration = LoggerHelper.GetRequestLoggingConfiguration(body: true);
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
            var request = new CoreRequest(HttpMethod.Post, "https://example.com/api/resource",
                new Dictionary<string, string>(), "{'key': 'value'}", null);

            // Act
            sdkLogger.LogRequest(request);

            //Assert
            LoggerHelper.AssertLogs(logger, LogLevel.Information,
                "Request POST https://example.com/api/resource (null)", 1);
            LoggerHelper.AssertLogs(logger, LogLevel.Information, "Request Body {'key': 'value'}", 1);
        }

        [Test]
        public void LogRequest_Configured_LogsRequestWithHeaders()
        {
            // Arrange
            var logger = new TestLogger();
            var requestLoggingConfiguration = LoggerHelper.GetRequestLoggingConfiguration(headers: true);
            var loggingConfiguration =
                LoggerHelper.GetLoggingConfiguration(logger, requestLoggingConfiguration);
            var sdkLogger = new SdkLogger(loggingConfiguration);
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
