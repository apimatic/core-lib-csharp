using System;
using APIMatic.Core.Types;

namespace APIMatic.Core.Security.Cryptography
{
    /// <summary>
    /// Abstract class for encoding and decoding digest values.
    /// </summary>
    internal abstract class DigestCodec
    {
        /// <summary>
        /// Decodes a string representation back into a byte array.
        /// </summary>
        /// <param name="encoded">The encoded string to decode.</param>
        /// <returns>The decoded byte array.</returns>
        public abstract byte[] Decode(string encoded);
        
        /// <summary>
        /// Creates a digest codec for the specified encoding type.
        /// </summary>
        /// <param name="encodingType">The encoding type to use.</param>
        /// <returns>A digest codec instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported encoding type is specified.</exception>
        public static DigestCodec Create(EncodingType encodingType)
        {
            return encodingType switch
            {
                EncodingType.Hex => new HexDigestCodec(),
                EncodingType.Base64 => new Base64DigestCodec(),
                EncodingType.Base64Url => new Base64UrlDigestCodec(),
                _ => throw new NotSupportedException($"Unsupported encoding type: {encodingType}")
            };
        }
    }
}
