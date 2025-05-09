using NUnit.Framework;
using APIMatic.Core.Proxy;

namespace APIMatic.Core.Test.Proxy
{
    public class CoreProxyConfigurationTest
    {
        private class MockProxyConfiguration : ICoreProxyConfiguration
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string User { get; set; }
            public string Pass { get; set; }
            public bool Tunnel { get; set; }
        }

        [Test]
        public void Constructor_ValidInput_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var mockProxy = new MockProxyConfiguration
            {
                Address = "http://proxy.example.com",
                Port = 8080,
                User = "user1",
                Pass = "pass123",
                Tunnel = true
            };

            // Act
            var proxyConfig = new CoreProxyConfiguration(mockProxy);

            // Assert
            Assert.AreEqual("http://proxy.example.com", proxyConfig.Address);
            Assert.AreEqual(8080, proxyConfig.Port);
            Assert.AreEqual("user1", proxyConfig.User);
            Assert.AreEqual("pass123", proxyConfig.Pass);
            Assert.IsTrue(proxyConfig.Tunnel);
        }

        [Test]
        public void Constructor_NullInput_ShouldSetDefaultValues()
        {
            // Act
            var proxyConfig = new CoreProxyConfiguration(null);

            // Assert
            Assert.IsNull(proxyConfig.Address);
            Assert.AreEqual(0, proxyConfig.Port);
            Assert.IsNull(proxyConfig.User);
            Assert.IsNull(proxyConfig.Pass);
            Assert.IsFalse(proxyConfig.Tunnel);
        }
    }
}
