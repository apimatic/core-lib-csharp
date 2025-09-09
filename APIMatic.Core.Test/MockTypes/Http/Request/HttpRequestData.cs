using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using APIMatic.Core.Http.Abstractions;

namespace APIMatic.Core.Test.MockTypes.Http.Request
{
    public class HttpRequestData : IHttpRequestData
    {
        public string Method { get; }
        public Uri Url { get; }
        public IReadOnlyDictionary<string, string[]> Headers { get; }
        public Stream Body { get; set; }
        public IReadOnlyDictionary<string, string[]> Query { get; }
        public IReadOnlyDictionary<string, string> Cookies { get; }
        public string Protocol { get; }
        public string ContentType { get; }
        public long? ContentLength { get; }

        public HttpRequestData(
            IDictionary<string, string[]> headers,
            Stream body)
        {
            Headers = new ReadOnlyDictionary<string, string[]>(headers);
            Body = body;
        }
    }
}
