using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Logger.Configuration
{
    [TestFixture]
    public class LoggingConfigurationTest
    {
        [Test]
        public void SdkLoggingConfiguration_ToString()
        {
            // Arrange
            var sdkLoggingConfiguration = new SdkLoggingConfiguration.Builder().Logger(NullLogger.Instance)
                .LogLevel(LogLevel.Error).MaskSensitiveHeaders(false).Build();

            // Act
            var actual = sdkLoggingConfiguration.ToString();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(
                "SdkLoggingConfiguration: Microsoft.Extensions.Logging.Abstractions.NullLogger , Error , False ," +
                " False , RequestConfiguration: False , False ,  ,  ,  , False  , ResponseConfiguration: False ," +
                " False ,  ,  ,   ", actual);
        }

        [Test]
        public void SdkLoggingConfiguration_ToBuilder()
        {
            // Arrange
            var sdkLoggingConfiguration = new SdkLoggingConfiguration.Builder().Logger(NullLogger.Instance)
                .LogLevel(LogLevel.Error).MaskSensitiveHeaders(false).Build();

            // Act
            var actual = sdkLoggingConfiguration.ToBuilder().LogLevel(LogLevel.Information).MaskSensitiveHeaders(true)
                .Build();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.MaskSensitiveHeaders);
            Assert.AreEqual(LogLevel.Information, actual.LogLevel);
            Assert.AreEqual(NullLogger.Instance, actual.Logger);
        }

        [Test]
        public void RequestLoggingConfiguration_ToString()
        {
            // Arrange
            var requestLoggingConfiguration = new RequestLoggingConfiguration.Builder()
                .Body(true)
                .Headers(true)
                .IncludeQueryInPath(true)
                .IncludeHeaders("Authorization", "Content-Type")
                .UnmaskHeaders("Authorization")
                .Build();

            // Act
            var actual = requestLoggingConfiguration.ToString();

            // Assert
            Assert.IsNotEmpty(actual);
            Assert.AreEqual(
                "RequestConfiguration: True , True , Authorization , Content-Type ,  , Authorization , True ", actual);
        }

        [Test]
        public void RequestLoggingConfiguration_ToBuilder()
        {
            // Arrange
            var requestLoggingConfiguration = new RequestLoggingConfiguration.Builder()
                .Body(true)
                .IncludeQueryInPath(true)
                .Build();

            // Act
            var actual = requestLoggingConfiguration.ToBuilder().Headers(true).Build();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Body);
            Assert.IsTrue(actual.Headers);
            Assert.IsTrue(actual.IncludeQueryInPath);
        }

        [Test]
        public void ResponseLoggingConfiguration_ToString()
        {
            // Arrange
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder()
                .Body(true)
                .Headers(true)
                .IncludeHeaders("Authorization", "Content-Type")
                .UnmaskHeaders("Authorization")
                .Build();

            // Act
            var actual = responseLoggingConfiguration.ToString();

            // Assert
            Assert.IsNotEmpty(actual);
            Assert.AreEqual("ResponseConfiguration: True , True , Authorization , Content-Type ,  , Authorization ",
                actual);
        }

        [Test]
        public void ResponseLoggingConfiguration_ToBuilder()
        {
            // Arrange
            var responseLoggingConfiguration = new ResponseLoggingConfiguration.Builder()
                .Body(true)
                .Build();

            // Act
            var actual = responseLoggingConfiguration.ToBuilder().Headers(true).Build();

            // Assert
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.Body);
            Assert.IsTrue(actual.Headers);
        }
    }
}
