using APIMatic.Core.Utilities.Logger.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Logger.Configuration
{
    [TestFixture]
    public class SdkLoggingConfigurationTest
    {
        [Test]
        public void SdkLoggingConfiguration_Clone_ShouldProduceDeepCopy()
        {
            // Arrange
            var originalConfig = new SdkLoggingConfiguration
            {
                Logger = NullLogger.Instance,
                LogLevel = LogLevel.Debug,
                MaskSensitiveHeaders = false,
                RequestLoggingConfiguration = new RequestLoggingConfiguration(),
                ResponseLoggingConfiguration = new ResponseLoggingConfiguration()
            };

            // Act
            var clonedConfig = (SdkLoggingConfiguration)originalConfig.Clone();

            // Assert
            Assert.IsNotNull(clonedConfig);
            Assert.AreNotSame(originalConfig, clonedConfig);
            Assert.AreEqual(originalConfig.Logger, clonedConfig.Logger);
            Assert.AreEqual(originalConfig.LogLevel, clonedConfig.LogLevel);
            Assert.AreEqual(originalConfig.MaskSensitiveHeaders, clonedConfig.MaskSensitiveHeaders);
            Assert.AreNotSame(originalConfig.RequestLoggingConfiguration, clonedConfig.RequestLoggingConfiguration);
            Assert.AreNotSame(originalConfig.ResponseLoggingConfiguration, clonedConfig.ResponseLoggingConfiguration);
        }
    }
}
