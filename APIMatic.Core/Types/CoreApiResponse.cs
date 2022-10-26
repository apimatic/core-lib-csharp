// <copyright file="CoreApiResponse.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;

namespace APIMatic.Core.Types
{
    /// <summary>
    /// CoreApiResponse Class.
    /// </summary>
    /// <typeparam name="T"> Generic type.</typeparam>
    public class CoreApiResponse<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResponse{T}"/> class.
        /// </summary>
        /// <param name="statusCode">Status code.</param>
        /// <param name="headers">Headers.</param>
        /// <param name="data">Data.</param>
        public CoreApiResponse(int statusCode, Dictionary<string, string> headers, T data)
        {
            this.StatusCode = statusCode;
            this.Headers = headers;
            this.Data = data;
        }

        /// <summary>
        /// Gets the HTTP Status code of the http response.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the headers of the http response.
        /// </summary>
        public Dictionary<string, string> Headers { get; }

        /// <summary>
        /// Gets the deserialized body of the http response.
        /// </summary>
        public T Data { get; }
    }
}
