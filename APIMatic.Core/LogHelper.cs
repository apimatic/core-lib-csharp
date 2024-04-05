using System;
using APIMatic.Core.Types.Sdk;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace APIMatic.Core
{
    public class LogHelper
    {
        private readonly ILogger _logger;
        private readonly LogLevel? level;

        public LogHelper(ILogger logger, LogLevel? level)
        {
            _logger = logger;
            this.level = level;
        }

        public void LogRequest(CoreRequest request)
        {
            // Method
            //     Url
            // Protocol (http/https)
            // TryCount
            //     Content Length
            //     Request Headers
            //     Request Body (configurable and give some warning in logs if enabled)
            var localLogLevel = level ?? LogLevel.Information;
            _logger.Log(level, "Request {HttpMethod} {Url} ", request.HttpMethod, request.QueryUrl);
                
        }
        

        public void LogResponse(CoreResponse response)
        {
            // Content Length
            // Response headers
            // Url
            //     Duration
            // Response Body (configurable and give some warning in logs if enabled)
            _logger.LogInformation("Response {HttpStatusCode} {Length}", response.StatusCode,
                response.Body.Length);
        }

        public IDisposable Scope() => _logger.BeginScope("some message", "property");
    }
}
