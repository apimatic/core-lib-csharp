using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Utilities.Logger
{
    internal interface ISdkLogger
    {
        /// <summary>
        /// Logs the details of a request.
        /// </summary>
        /// <param name="request">The request to be logged.</param>
        void LogRequest(CoreRequest request);

        /// <summary>
        /// Logs the details of a response.
        /// </summary>
        /// <param name="response">The response to be logged.</param>
        void LogResponse(CoreResponse response);
    }
}
