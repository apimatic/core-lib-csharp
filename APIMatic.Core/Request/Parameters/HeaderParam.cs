using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Request.Parameters
{
    public class HeaderParam : Parameter
    {
        internal HeaderParam() { }
        public HeaderParam Init(string key, object value)
        {

            return this;
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
        }
    }
}
