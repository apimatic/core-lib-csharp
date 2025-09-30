using System;
using System.Text;

namespace APIMatic.Core.Security.Cryptography
{
    /// <summary>
    /// HexDigestCodec digest codec implementation.
    /// </summary>
    internal class HexDigestCodec : DigestCodec
    {
        /// <summary> 
        /// Decodes a hexadecimal string back into a byte array.
        /// </summary>
        /// <param name="encoded">The hexadecimal string to decode.</param>
        /// <returns>The decoded byte array.</returns>
        /// <exception cref="ArgumentException">Thrown when the input is null, empty, or has invalid length.</exception>
        /// <exception cref="FormatException">Thrown when the input is not a valid hexadecimal string.</exception>
        public override byte[] Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
                throw new ArgumentException("Input cannot be null or empty", nameof(encoded));

            // Remove any whitespace and convert to uppercase for consistency
            encoded = encoded.Replace(" ", "").Replace("-", "").ToUpperInvariant();

            // Hex string must have even length
            if (encoded.Length % 2 != 0)
                throw new FormatException("Hexadecimal string must have even length");

            byte[] bytes = new byte[encoded.Length / 2];
            
            for (int i = 0; i < bytes.Length; i++)
            {
                string hexPair = encoded.Substring(i * 2, 2);
                try
                {
                    bytes[i] = Convert.ToByte(hexPair, 16);
                }
                catch (FormatException)
                {
                    throw new FormatException($"Invalid hexadecimal character in string at position {i * 2}: '{hexPair}'");
                }
            }
            
            return bytes;
        }
    }
}
