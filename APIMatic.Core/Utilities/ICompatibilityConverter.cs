using APIMatic.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Utilities
{
    public interface ICompatibilityConverter<Request, Response, Context>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context: CoreContext<Request, Response>
    {
        Request CreateHttpRequest(CoreRequest request);
        Response CreateHttpResponse(CoreResponse response);
        Context CreateHttpContext(CoreRequest request, CoreResponse response);
    }
}
