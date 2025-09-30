using System;
using APIMatic.Core.Types;

namespace APIMatic.Core.Security.Cryptography
{
    /// <summary>
    /// Factory class for creating digest codecs based on encoding type.
    /// </summary>
    public static class DigestCodecFactory
    {
        /// <summary>
        /// Creates a digest codec for the specified encoding type.
        /// </summary>
        /// <param name="encodingType">The encoding type to use.</param>
        /// <returns>A digest codec instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported encoding type is specified.</exception>
        public static IDigestCodec Create(EncodingType encodingType)
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
