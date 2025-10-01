using APIMatic.Core.Security.SignatureVerifier;
using NUnit.Framework;

namespace APIMatic.Core.Test.Security.SignatureVerifier
{
    [TestFixture]
    public class SignatureVerificationExtensionsTests
    {
        [TestCase(null, null, true)]
        [TestCase(null, new byte[] { 1, 2, 3 }, false)]
        [TestCase(new byte[] { 1, 2, 3 }, null, false)]
        [TestCase(new byte[] { 1, 2, 3 }, new byte[] { 1, 2 }, false)]
        [TestCase(new byte[] { }, new byte[] { }, true)]
        [TestCase(new byte[] { 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4 }, true)]
        [TestCase(new byte[] { 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 5 }, false)]
        public void ConstantTimeEquals_VariousInputs_ReturnsExpected(byte[] a, byte[] b, bool expected)
        {
            Assert.AreEqual(expected, SignatureVerifierExtensions.FixedTimeEquals(a, b));
        }
    }
}
