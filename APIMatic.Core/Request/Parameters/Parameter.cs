using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Request.Parameters
{
    public abstract class Parameter
    {
        internal abstract void Apply(RequestBuilder requestBuilder);
    }
}
