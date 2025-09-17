using System;
using System.Security.Cryptography;

namespace APIMatic.Core.Types.Sdk
{
    public enum HmacAlgorithm
    {
        Sha256,
        Sha512
    }
    
    internal static class HmacFactory
    {
        public static HMAC Create(HmacAlgorithm algorithm, byte[] keyBytes)
        {
            switch (algorithm)
            {
                case HmacAlgorithm.Sha256:
                    return new HMACSHA256(keyBytes);

                case HmacAlgorithm.Sha512:
                    return new HMACSHA512(keyBytes);

                default:
                    throw new NotSupportedException($"Unsupported HMAC algorithm: {algorithm}");
            }
        }
    }
}
