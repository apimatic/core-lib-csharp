using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace APIMatic.Core.Test.Utilities
{
    internal static class Extensions
    {
        public static Dictionary<string, string> ToQueryStringDictionary(this string data)
        {
            var parsedQueryString = HttpUtility.ParseQueryString(data);
            return parsedQueryString.AllKeys.ToDictionary(k => k, k => parsedQueryString[k]);
        }
        
        public static Dictionary<string, string> ToJsonToDictionary(this string data)
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
        }
    }
}
