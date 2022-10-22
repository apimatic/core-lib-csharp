using APIMatic.Core.Utilities;
using System.Collections.Generic;
using System.IO;

namespace APIMatic.Core.Types
{
    /// <summary>
    /// CoreResponse.
    /// </summary>
    public class CoreResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponse"/> class.
        /// </summary>
        /// <param name="statusCode">statusCode.</param>
        /// <param name="headers">headers.</param>
        /// <param name="rawBody">rawBody.</param>
        public CoreResponse(int statusCode, Dictionary<string, string> headers, Stream rawBody)
            => (StatusCode, Headers, RawBody) = (statusCode, headers, rawBody);

        /// <summary>
        /// Gets the HTTP Status code of the http response.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the headers of the http response.
        /// </summary>
        public Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the stream of the body.
        /// </summary>
        public Stream RawBody { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $" StatusCode = {StatusCode}, " +
                $" Headers = {CoreHelper.JsonSerialize(Headers)}" +
                $" RawBody = {RawBody}";
        }
    }
}