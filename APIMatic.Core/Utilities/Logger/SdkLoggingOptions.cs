using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core.Utilities.Logger
{
    public class SdkLoggingOptions
    {
        internal static readonly SdkLoggingOptions Default = new SdkLoggingOptions(NullLogger.Instance);

        public SdkLoggingOptions(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        internal ILogger Logger { get; }
        
        public LogLevel? LogLevel { get; set; }

        public RequestOptions Request { get; } = new RequestOptions();

        public ResponseOptions Response { get; } = new ResponseOptions();

        public abstract class LogBaseOptions
        {
            public bool LogBody { get; set; }

            public bool LogHeaders { get; set; }

            public IReadOnlyCollection<string> HeadersToInclude { get; private set; } = new List<string>();

            public IReadOnlyCollection<string> HeadersToExclude { get; private set; } = new List<string>();

            public void ExcludeHeaders(params string[] excludeHeaders) => HeadersToExclude = excludeHeaders;

            public void IncludeHeaders(params string[] includeHeaders) => HeadersToInclude = includeHeaders;
        }


        public class RequestOptions: LogBaseOptions
        {
            public RequestOptions() => ExcludeHeaders("Authorization");
            public bool IncludeQueryInPath { get; set; }
        }

        public class ResponseOptions: LogBaseOptions
        {
        }

        public void LogBody(bool requestBody = true, bool responseBody = true)
        {
            Request.LogBody = requestBody;
            Response.LogBody = responseBody;
        }
        public void LogHeaders(bool requestHeaders = true, bool responseHeaders = true)
        {
            Request.LogHeaders = requestHeaders;
            Response.LogHeaders = responseHeaders;
        }
    }
}
