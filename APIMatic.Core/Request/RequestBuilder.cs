using APIMatic.Core.Types;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace APIMatic.Core.Request
{
    public class RequestBuilder
    {
        private readonly GlobalConfiguration configuration;
        internal StringBuilder serverUrl = new StringBuilder();
        internal HttpMethod httpMethod;
        internal Dictionary<string, string> headers;
        internal object body;
        internal List<KeyValuePair<string, object>> formParameters;
        internal Dictionary<string, object> queryParameters;

        internal RequestBuilder(GlobalConfiguration configuration) => this.configuration = configuration;

        internal RequestBuilder ServerUrl(string serverUrl)
        {
            this.serverUrl.Append(serverUrl);
            return this;
        }

        public RequestBuilder Init(HttpMethod httpMethod, string path)
        {
            this.httpMethod = httpMethod;
            serverUrl.Append(path);
            return this;
        }

        public CoreRequest Build()
        {
            configuration.GlobalRuntimeParameters.ForEach(param => param.Apply(this));

            return new CoreRequest(httpMethod, serverUrl.ToString(), headers, body,
                formParameters, null, null, queryParameters);
        }
    }
}
