// <copyright file="IHttpClient.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Threading;
using System.Threading.Tasks;
using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Http.Client
{
    /// <summary>
    /// IHttpClient.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// Event raised before an Http request is sent over the network
        /// This event can be used for logging, request modification, appending
        /// additional headers etc.
        /// </summary>
        event OnBeforeHttpRequestEventHandler OnBeforeHttpRequestEvent;

        /// <summary>
        /// Event raised after an Http response is recieved from the network.
        /// This event can be used for logging, response modification, extracting
        /// additional information etc.
        /// </summary>
        event OnAfterHttpResponseEventHandler OnAfterHttpResponseEvent;

        /// <summary>
        /// Execute a given HttpRequest to get async string response back.
        /// </summary>
        /// <param name="request">The given HttpRequest to execute.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <param name="retryConfiguration">The <see cref="RetryConfiguration"/> for request.</param>
        /// <returns> HttpResponse containing raw information.</returns>
        Task<CoreResponse> ExecuteAsync(CoreRequest request, CancellationToken cancellationToken = default);
    }
}
