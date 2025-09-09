namespace APIMatic.Core.Security.Cryptography
{
    /// <summary>
    /// Interface for encoding and decoding digest values.
    /// </summary>
    public interface IDigestCodec
    {
        /// <summary>
        /// Decodes a string representation back into a byte array.
        /// </summary>
        /// <param name="encoded">The encoded string to decode.</param>
        /// <returns>The decoded byte array.</returns>
        byte[] Decode(string encoded);
    }
}
