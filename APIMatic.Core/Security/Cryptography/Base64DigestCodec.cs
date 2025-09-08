using System;

namespace APIMatic.Core.Security.Cryptography
{
    /// <summary>
    /// Base64 digest codec implementation.
    /// </summary>
    public class Base64DigestCodec : IDigestCodec
    {
        /// <summary>
        /// Decodes a Base64 string back into a byte array.
        /// </summary>
        /// <param name="encoded">The Base64 string to decode.</param>
        /// <returns>The decoded byte array.</returns>
        /// <exception cref="FormatException">Thrown when the input is not a valid Base64 string.</exception>
        public byte[] Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
                throw new ArgumentException("Input cannot be null or empty", nameof(encoded));
            
            return Convert.FromBase64String(encoded);
        }
    }
}
