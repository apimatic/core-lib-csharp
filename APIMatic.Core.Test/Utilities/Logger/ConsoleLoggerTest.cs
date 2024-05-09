using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using APIMatic.Core.Utilities.Logger;
using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;

namespace APIMatic.Core.Test.Utilities.Logger
{
    [TestFixture]
    public class ConsoleLoggerTests
    {
        private ILogger _logger;
        private EventId _eventId;
        private readonly string _state = "Test log message";

        private string Formatter(string s, Exception ex) => $"{s} {ex?.Message}";

        [SetUp]
        public void SetUp()
        {
            _logger = SdkLoggingConfiguration.Console().Logger;
            _eventId = new EventId(100);
        }

        private void AssertLog(LogLevel logLevel, Exception exception, string expectedLogString)
        {
            // Redirect Console output to a StringWriter for testing
            using StringWriter sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);
            try
            {
                // Act
                _logger.Log(logLevel, _eventId, _state, exception, Formatter);

                // Assert
                Assert.AreEqual(expectedLogString, sw.ToString().Trim());
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [TestCase(LogLevel.Trace, "trce: Console[100] Test log message")]
        [TestCase(LogLevel.Debug, "dbug: Console[100] Test log message")]
        [TestCase(LogLevel.Information, "info: Console[100] Test log message")]
        [TestCase(LogLevel.Warning, "warn: Console[100] Test log message")]
        [TestCase(LogLevel.Error, "fail: Console[100] Test log message Test exception System.Exception: Test exception",
            "Test exception")]
        [TestCase(LogLevel.Critical, "crit: Console[100] Test log message")]
        [TestCase(LogLevel.None, "")]
        public void Log_WritesToConsole(LogLevel logLevel, string expectedLogString, string exceptionMessage = null)
        {
            // Arrange
            Exception exception = null;
            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                exception = new Exception(exceptionMessage);
            }

            // Act & Assert
            AssertLog(logLevel, exception, expectedLogString);
        }

        [TestCase(LogLevel.Information, "")]
        public void Log_WritesToConsole_Null(LogLevel logLevel, string expectedLogString)
        {
            // Redirect Console output to a StringWriter for testing
            using StringWriter sw = new StringWriter();
            var originalOut = Console.Out;
            string Func(string s, Exception exception) => null;
            Console.SetOut(sw);
            try
            {
                // Act
                _logger.Log(logLevel, _eventId, null, null, (Func<string, Exception, string>)Func);

                // Assert
                Assert.AreEqual(expectedLogString, sw.ToString().Trim());
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Test]
        public void GetLogLevelString_InvalidLogLevel_ThrowsArgumentOutOfRangeException()
        {
            var getLogLevelString =
                typeof(ConsoleLogger).GetMethod("GetLogLevelString", BindingFlags.NonPublic | BindingFlags.Static);
            object[] parameters = { LogLevel.None };

            // Act
            var ex = Assert.Throws<TargetInvocationException>(
                () => getLogLevelString?.Invoke(ConsoleLogger.Instance, parameters));
            
            // Assert
            Assert.That(ex?.InnerException, Is.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void IsEnabled_ReturnsFalseForDisabledLevel()
        {
            // Act & Assert
            Assert.IsFalse(_logger.IsEnabled(LogLevel.None));
        }

        [Test]
        public void BeginScope_ReturnsDisposableObject()
        {
            // Act
            using var scope = _logger.BeginScope<object>(null);

            // Assert
            Assert.IsNotNull(scope);
            Assert.IsInstanceOf<IDisposable>(scope);
        }
    }
}
