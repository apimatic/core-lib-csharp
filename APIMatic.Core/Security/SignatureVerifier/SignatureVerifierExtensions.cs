using System;
using System.Runtime.CompilerServices;

namespace APIMatic.Core.Security.SignatureVerifier
{
    internal static class SignatureVerifierExtensions
    {
        /// <summary>
        /// Performs a secure comparison of two byte arrays to prevent timing attacks.
        /// </summary>
        /// <param name="left">First byte array.</param>
        /// <param name="right">Second byte array.</param>
        /// <returns>True if arrays are equal, false otherwise.</returns>
        /// <remarks>
        /// This implementation is copied from CryptographicOperations in System.Security.Cryptography
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool FixedTimeEquals(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right)
        {
            // NoOptimization because we want this method to be exactly as non-short-circuiting
            // as written.
            //
            // NoInlining because the NoOptimization would get lost if the method got inlined.
 
            if (left.Length != right.Length)
            {
                return false;
            }
 
            int length = left.Length;
            int accum = 0;
 
            for (int i = 0; i < length; i++)
            {
                accum |= left[i] - right[i];
            }
 
            return accum == 0;
        }
    }
}
