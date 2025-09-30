using System;

namespace APIMatic.Core.Security.Cryptography
{
    /// <summary>
    /// Base64Url digest codec implementation.
    /// </summary>
    public class Base64UrlDigestCodec : IDigestCodec
    {
        /// <summary>
        /// Decodes a Base64Url string back into a byte array.
        /// </summary>
        /// <param name="encoded">The Base64Url string to decode.</param>
        /// <returns>The decoded byte array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the input is null.</exception>
        /// <exception cref="FormatException">Thrown when the input is not a valid Base64Url string.</exception>
        public byte[] Decode(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
                throw new ArgumentException("Input cannot be null or empty", nameof(encoded));
            
            // Restore padding and standard Base64 characters
            var base64 = encoded.Replace('-', '+').Replace('_', '/');
            
            // Add padding if necessary
            switch (base64.Length % 4)
            {
                case 2: 
                    base64 += "=="; 
                    break;
                case 3: 
                    base64 += "="; 
                    break;
            }
            
            return Convert.FromBase64String(base64);
        }
    }
}
