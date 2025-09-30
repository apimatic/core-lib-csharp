using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Http.Abstractions;
using APIMatic.Core.Security.Abstractions;
using APIMatic.Core.Security.Cryptography;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Security.SignatureVerifier
{
    /// <summary>
    /// HMAC-based signature verifier for HTTP requests.
    /// </summary>
    public class HmacSignatureVerifier : ISignatureVerifier
    {
        /// <summary>
        /// Name of the header carrying the provided signature (case-insensitive lookup).
        /// </summary>
        private readonly string _signatureHeader;

        /// <summary>
        /// Optional template for the expected signature value, where `{digest}` is replaced
        /// by the encoded digest. If omitted, the expected signature is the encoded digest itself.
        /// </summary>
        private readonly string _signatureValueTemplate;

        /// <summary>
        /// Resolves the request data into a byte array for signature computation.
        /// </summary>
        private readonly Func<IHttpRequestData, CancellationToken, Task<byte[]>> _requestSignatureTemplateResolverAsync;

        /// <summary>
        /// The HMAC algorithm used for signature computation.
        /// </summary>
        private readonly HmacAlgorithm _signatureAlgorithm;

        /// <summary>
        /// The secret key, encoded as a byte array, used for HMAC operations.
        /// </summary>
        private readonly byte[] _encodedSecretKey;

        /// <summary>
        /// Codec used for encoding and decoding digests based on the specified encoding type.
        /// </summary>
        private readonly DigestCodec _digestCodec;
        
        private const string DigestPlaceHolder = "{digest}";

        /// <summary>
        /// Initializes a new instance of the HmacSignatureVerifier class.
        /// </summary>
        /// <param name="secretKey">The secret key for HMAC computation.</param>
        /// <param name="signatureHeader">The name of the header containing the signature.</param>
        /// <param name="requestSignatureTemplateResolverAsync">Optional custom resolver for extracting data to sign.</param>
        /// <param name="hashAlgorithm">Optional HMAC algorithm.</param>
        /// <param name="digestEncoding">The encoding type for the signature.</param>
        /// <param name="signatureValueTemplate">Template for signature format.</param>
        public HmacSignatureVerifier(
            string secretKey,
            string signatureHeader,
            EncodingType digestEncoding,
            Func<IHttpRequestData, CancellationToken, Task<byte[]>> requestSignatureTemplateResolverAsync = null,
            HmacAlgorithm hashAlgorithm = HmacAlgorithm.Sha256,
            string signatureValueTemplate = "{digest}")
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentNullException(nameof(secretKey), "Secret key cannot be null or Empty.");

            if (string.IsNullOrWhiteSpace(signatureHeader))
                throw new ArgumentNullException(nameof(signatureHeader), "Signature header cannot be null or Empty.");

            _encodedSecretKey = Encoding.UTF8.GetBytes(secretKey);
            _signatureHeader = signatureHeader;
            _signatureAlgorithm = hashAlgorithm;
            _digestCodec = DigestCodec.Create(digestEncoding);
            _signatureValueTemplate = signatureValueTemplate;
            _requestSignatureTemplateResolverAsync = requestSignatureTemplateResolverAsync ??
                                                     (async (request, cancellationToken) =>
                                                         await request.ReadBodyStreamToByteArrayAsync(cancellationToken)
                                                             .ConfigureAwait(false));
        }
        
        /// <summary>
        /// Verifies the HMAC signature of the specified HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request data to verify.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>
        /// A <see cref="VerificationResult"/> indicating whether the signature is valid or the reason for failure.
        /// </returns>
        public async Task<VerificationResult> VerifyAsync(IHttpRequestData request,
            CancellationToken cancellationToken = default)
        {
            // Case-insensitive header lookup
            var headerEntry = request.Headers.FirstOrDefault(h =>
                string.Equals(h.Key, _signatureHeader, StringComparison.OrdinalIgnoreCase));

            if (headerEntry.Key == null)
            {
                return VerificationResult.Failure(new[] { $"Signature header '{_signatureHeader}' is missing." });
            }

            if (!TryExtractSignature(headerEntry.Value.FirstOrDefault(), out var providedSignature))
            {
                return VerificationResult.Failure(new[] { $"Malformed signature header '{_signatureHeader}' value." });
            }

            var resolvedTemplateBytes = await _requestSignatureTemplateResolverAsync.Invoke(request, cancellationToken)
                .ConfigureAwait(false);

            using (var hmac = HmacFactory.Create(_signatureAlgorithm, _encodedSecretKey))
            {
                var computedHash = hmac.ComputeHash(resolvedTemplateBytes);

                return SignatureVerifierExtensions.FixedTimeEquals(computedHash, providedSignature)
                    ? VerificationResult.Success()
                    : VerificationResult.Failure(new[] { "Signature verification failed." });
            }
        }

        /// <summary>
        /// Extracts the digest value from the signature header according to the template.
        /// </summary>
        /// <param name="signatureValue">The signature header value.</param>
        /// <param name="signatureValueTemplate"> The Signature value template.</param>
        /// <returns>The extracted digest string.</returns>
        private static string ExtractDigestFromTemplate(string signatureValue, string signatureValueTemplate)
        {
            if (string.IsNullOrEmpty(signatureValue))
                return string.Empty;

            // If template is just "{digest}", return the signature as-is
            if (signatureValueTemplate == DigestPlaceHolder)
                return signatureValue;

            // Extract digest from template
            var digestIndex = signatureValueTemplate.IndexOf(DigestPlaceHolder, StringComparison.Ordinal);
            if (digestIndex == -1)
                return string.Empty;

            var prefix = signatureValueTemplate[..digestIndex];
            var suffix = signatureValueTemplate[(digestIndex + DigestPlaceHolder.Length)..];

            if (!signatureValue.StartsWith(prefix) || !signatureValue.EndsWith(suffix))
                return string.Empty;

            return signatureValue.Substring(prefix.Length, signatureValue.Length - prefix.Length - suffix.Length);
        }

        /// <summary>
        /// Attempts to extract and decode the signature from the header values.
        /// </summary>
        /// <param name="signatureValue">The signature header value.</param>
        /// <param name="signature">The decoded signature bytes.</param>
        /// <returns>True if extraction and decoding succeeded, false otherwise.</returns>
        private bool TryExtractSignature(string signatureValue, out byte[] signature)
        {
            signature = null;
            var digest = ExtractDigestFromTemplate(signatureValue, _signatureValueTemplate);

            if (string.IsNullOrEmpty(digest))
                return false;

            try
            {
                signature = _digestCodec.Decode(digest);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
