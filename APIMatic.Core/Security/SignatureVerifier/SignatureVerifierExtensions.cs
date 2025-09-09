namespace APIMatic.Core.Security.SignatureVerifier
{
    internal static class SignatureVerifierExtensions
    {
        /// <summary>
        /// Performs a secure comparison of two byte arrays to prevent timing attacks.
        /// </summary>
        /// <param name="a">First byte array.</param>
        /// <param name="b">Second byte array.</param>
        /// <returns>True if arrays are equal, false otherwise.</returns>
        public static bool ConstantTimeEquals(this byte[] a, byte[] b)
        {
            if (a == null && b == null)
                return true;

            if (a == null || b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            var result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}
