using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Http.Abstractions;
using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Security.Abstractions
{
    /// <summary>
    /// Defines a contract for verifying the signature of an HTTP request.
    /// </summary>
    public interface ISignatureVerifier
    {
        /// <summary>
        /// Verifies the signature of the specified HTTP request.
        /// </summary>
        /// <param name="request">The HTTP request data to verify.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// VerificationResult containing the outcome of the verification process.
        /// </returns>
        Task<VerificationResult> VerifyAsync(IHttpRequestData request, CancellationToken cancellationToken = default);
    }
}