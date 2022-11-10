// <copyright file="CoreContext.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>

namespace APIMatic.Core.Types.Sdk
{
    public class CoreContext<Req, Res>
        where Req : CoreRequest
        where Res : CoreResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreContext"/> class.
        /// </summary>
        /// <param name="request">The http request in the current context.</param>
        /// <param name="response">The http response in the current context.</param>
        public CoreContext(Req request, Res response) => (Request, Response) = (request, response);

        /// <summary>
        /// Gets the http request in the current context.
        /// </summary>
        public Req Request { get; }

        /// <summary>
        /// Gets the http response in the current context.
        /// </summary>
        public Res Response { get; }

        internal bool IsFailure()
        {
            return (Response.StatusCode < 200) || (Response.StatusCode > 208);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $" Request = {Request}, Response = {Response}";
        }
    }
}
