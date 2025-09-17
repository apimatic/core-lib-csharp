using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Http.Abstractions;

namespace APIMatic.Core.Utilities
{
    /// <summary>
    /// Extension methods for IHttpRequestData.
    /// </summary>
    internal static class IHttpRequestDataExtensions
    {
        /// <summary>
        /// Reads the request body stream and converts it to a byte array.
        /// </summary>
        /// <param name="requestData">The HTTP request data.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A byte array containing the request body data.</returns>
        internal static async Task<byte[]> ReadBodyStreamToByteArrayAsync(this IHttpRequestData requestData,
            CancellationToken cancellationToken = default)
        {
            if (requestData.Body == null)
                return Array.Empty<byte>();

            if (requestData.Body.CanSeek)
                requestData.Body.Position = 0;

            using (var memoryStream = new MemoryStream())
            {
                await requestData.Body.CopyToAsync(memoryStream, 81920, cancellationToken).ConfigureAwait(false);
                return memoryStream.ToArray();
            }
        }
    }
}