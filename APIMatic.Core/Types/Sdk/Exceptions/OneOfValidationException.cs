using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Types.Sdk.Exceptions
{
    public class OneOfValidationException : Exception
    {
        public OneOfValidationException(string type1, string type2, JToken json)
            : base($"There are more than one matching types i.e. {type1} and {type2} on: {json}")
        {
        }

        public OneOfValidationException(List<string> types, JToken json)
            : base($"We could not match any acceptable type from {string.Join(", ", types)} on: {json}")
        {
        }
    }

}
