using System;
using System.Collections.Generic;
using System.Linq;

namespace APIMatic.Core.Types.Sdk.Exceptions
{
    public class AuthValidationException : ArgumentNullException
    {
        private const string ErrorMessagePrefix = "Following authentication credentials are required:\n-> ";

        public AuthValidationException(List<string> errors)
            : base(null, ErrorMessagePrefix + string.Join("\n-> ", errors.Select(e => e.TrimStart(ErrorMessagePrefix.ToCharArray()))))
        {
        }
    }

}
