using System;
using APIMatic.Core.Security.Cryptography;
using APIMatic.Core.Types;
using NUnit.Framework;

namespace APIMatic.Core.Test.Security.Cryptography
{
    public class DigestCodecTests
    {
        [TestCase(EncodingType.Hex, "4A6F686E", new byte[] { 0x4A, 0x6F, 0x68, 0x6E })]
        [TestCase(EncodingType.Base64, "SGVsbG8=", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
        [TestCase(EncodingType.Base64Url, "SGVsbG8", new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F })]
        [TestCase(EncodingType.Base64Url, "SG", new byte[] { 0x48 })]
        public void DigestCodec_Decode_Success(EncodingType encodingType, string input, byte[] expected)
        {
            var codec = DigestCodecFactory.Create(encodingType);
            var result = codec.Decode(input);
            Assert.AreEqual(expected, result);
        }
        
        [TestCase(EncodingType.Hex, "")]
        [TestCase(EncodingType.Hex, null)]
        [TestCase(EncodingType.Hex, "ABC")]
        [TestCase(EncodingType.Base64, "")]
        [TestCase(EncodingType.Base64, null)]
        [TestCase(EncodingType.Base64Url, "")]
        [TestCase(EncodingType.Base64Url, null)]
        public void DigestCodecIncorrectInput_Decode_DigestCodec_Create_Exception(EncodingType encodingType, string input)
        {
            var codec = DigestCodecFactory.Create(encodingType);
            Assert.Throws<ArgumentException>(() => codec.Decode(input));
        }
        
        [TestCase(-1)]
        public void DigestCodec_Create_Exception(int invalidValue)
        {
            var encodingType = (EncodingType)invalidValue;
            Assert.Throws<ArgumentOutOfRangeException>(() => DigestCodecFactory.Create(encodingType));
        }
    }
}
