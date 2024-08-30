// <copyright file="HttpCallBack.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Types
{
    /// <summary>
    /// HttpCallBack.
    /// </summary>
    public class HttpCallBack
    {
        /// <summary>
        /// Gets http request.
        /// </summary>
        public CoreRequest Request { get; private set; }

        /// <summary>
        /// Gets http response.
        /// </summary>
        public CoreResponse Response { get; private set; }

        /// <summary>
        /// BeforeHttpRequestEventHandler.
        /// </summary>
        /// <param name="request">Http Request.</param>
        public virtual void OnBeforeHttpRequestEventHandler(CoreRequest request)
        {
            Request = request;
        }

        /// <summary>
        /// AfterHttpResponseEventHandler.
        /// </summary>
        /// <param name="source">Http Client.</param>
        /// <param name="response">Http Response.</param>
        public virtual void OnAfterHttpResponseEventHandler(CoreResponse response)
        {
            Response = response;
        }
    }
}
