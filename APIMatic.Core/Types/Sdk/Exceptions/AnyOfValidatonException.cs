using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Types.Sdk.Exceptions
{
    public class AnyOfValidationException : Exception
    {
        public AnyOfValidationException(List<string> types, JToken json)
            : base($"We could not match any acceptable type from {string.Join(", ", types)} on: {json}")
        {
        }
    }

}
