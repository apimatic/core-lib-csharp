using System;
using System.Security.Cryptography;
using APIMatic.Core.Types.Sdk;
using NUnit.Framework;

namespace APIMatic.Core.Test.Security
{
    [TestFixture]
    public class HmacFactoryTests
    {
        [Test]
        public void HmacAlgorithmSha256_HmacFactoryCreate_ReturnsHMACSHA256()
        {
            var key = new byte[] { 1, 2, 3 };
            var hmac = HmacFactory.Create(HmacAlgorithm.Sha256, key);
            Assert.IsInstanceOf<HMACSHA256>(hmac);
        }

        [Test]
        public void HmacAlgorithmSha512_HmacFactoryCreate_ReturnsHMACSHA512()
        {
            var key = new byte[] { 4, 5, 6 };
            var hmac = HmacFactory.Create(HmacAlgorithm.Sha512, key);
            Assert.IsInstanceOf<HMACSHA512>(hmac);
        }

        [Test]
        public void UnsupportedAlgorithm_HmacFactoryCreate_ThrowsNotSupportedException()
        {
            var key = new byte[] { 7, 8, 9 };
            const HmacAlgorithm invalidAlgorithm = (HmacAlgorithm)999;
            Assert.Throws<NotSupportedException>(() => HmacFactory.Create(invalidAlgorithm, key));
        }
    }
}
