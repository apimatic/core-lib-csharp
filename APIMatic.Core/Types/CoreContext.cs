using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Types
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return $" Request = {this.Request}, " +
                $" Response = {this.Response}";
        }
    }
}
