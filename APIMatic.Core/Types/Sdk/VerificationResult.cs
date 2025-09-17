using System.Collections.Generic;

namespace APIMatic.Core.Types.Sdk
{
    /// <summary>
    /// Represents the result of an operation that can either succeed
    /// or fail with an error message.
    /// </summary>
    public class VerificationResult
    {
        public bool IsSuccess => Errors == null;

        public IReadOnlyCollection<string> Errors { get; }
        
        protected VerificationResult(string[] error)
        {
            Errors = error;
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static VerificationResult Success() =>
            new VerificationResult(null);

        /// <summary>
        /// Creates a failed result with the given error message.
        /// </summary>
        public static VerificationResult Failure(string[] error) =>
            new VerificationResult(error);
    }
}