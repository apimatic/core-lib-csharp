using System;
using System.Collections.Generic;
using System.Text;

namespace APIMatic.Core.Request.Parameters
{
    public class TemplateParam : Parameter
    {
        internal TemplateParam() { }

        public TemplateParam Init(string key, object value)
        {
            return this;
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
        }
    }
}
