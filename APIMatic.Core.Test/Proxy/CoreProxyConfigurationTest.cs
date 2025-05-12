using NUnit.Framework;
using APIMatic.Core.Proxy;

namespace APIMatic.Core.Test.Proxy
{
    public class CoreProxyConfigurationTest
    {
        [Test]
        public void Constructor_WithValidInput_ShouldSetAllPropertiesCorrectly()
        {
            // Act
            var proxyConfig = new CoreProxyConfiguration(
                "http://proxy.example.com", 8080, "user1", "pass123", true
            );

            // Assert
            Assert.AreEqual("http://proxy.example.com", proxyConfig.Address);
            Assert.AreEqual(8080, proxyConfig.Port);
            Assert.AreEqual("user1", proxyConfig.User);
            Assert.AreEqual("pass123", proxyConfig.Pass);
            Assert.IsTrue(proxyConfig.Tunnel);
        }

        [Test]
        public void CopyConstructor_WithValidProxy_ShouldCopyAllProperties()
        {
            // Arrange
            var original = new CoreProxyConfiguration(
                "http://proxy.example.com", 8888, "user2", "secret", false
            );

            // Act
            var copy = new CoreProxyConfiguration(original);

            // Assert
            Assert.AreEqual(original.Address, copy.Address);
            Assert.AreEqual(original.Port, copy.Port);
            Assert.AreEqual(original.User, copy.User);
            Assert.AreEqual(original.Pass, copy.Pass);
            Assert.AreEqual(original.Tunnel, copy.Tunnel);
        }

        [Test]
        public void CopyConstructor_WithNullProxy_ShouldSetDefaults()
        {
            // Act
            var copy = new CoreProxyConfiguration(null);

            // Assert
            Assert.IsNull(copy.Address);
            Assert.AreEqual(0, copy.Port);
            Assert.IsNull(copy.User);
            Assert.IsNull(copy.Pass);
            Assert.IsFalse(copy.Tunnel);
        }
    }
}
