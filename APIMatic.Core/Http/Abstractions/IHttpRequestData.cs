using System;
using System.Collections.Generic;
using System.IO;

namespace APIMatic.Core.Http.Abstractions
{
    /// <summary>
    /// Represents the contract for HTTP request data, including method, URL, headers, body, query parameters, cookies, protocol, content type, and content length.
    /// </summary>
    public interface IHttpRequestData
    {
        /// <summary>
        /// Gets the HTTP method (e.g., GET, POST, PUT, DELETE).
        /// </summary>
        string Method { get; }

        /// <summary>
        /// Gets the request URL.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Gets the collection of HTTP headers.
        /// </summary>
        IReadOnlyDictionary<string, string[]> Headers { get; }

        /// <summary>
        /// Gets the request body as a stream.
        /// </summary>
        Stream Body { get; }

        /// <summary>
        /// Gets the collection of query parameters.
        /// </summary>
        /// <remarks>
        /// Caller owns disposal.
        /// </remarks>
        IReadOnlyDictionary<string, string[]> Query { get; }

        /// <summary>
        /// Gets the collection of cookies.
        /// </summary>
        IReadOnlyDictionary<string, string> Cookies { get; }

        /// <summary>
        /// Gets the HTTP protocol version (e.g., "HTTP/1.1").
        /// </summary>
        string Protocol { get; }

        /// <summary>
        /// Gets the content type of the request (e.g., "application/json").
        /// </summary>
        string ContentType { get; }

        /// <summary>
        /// Gets the content length of the request body, if known.
        /// </summary>
        long? ContentLength { get; }
    }
}