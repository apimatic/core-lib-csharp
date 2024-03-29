﻿// <copyright file="MultipartFileContent.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using APIMatic.Core.Types.Sdk;

namespace APIMatic.Core.Types
{
    /// <summary>
    /// MultipartFileContent.
    /// </summary>
    internal class MultipartFileContent : MultipartContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartFileContent"/> class.
        /// </summary>
        /// <param name="file">file.</param>
        /// <param name="headers">headers.</param>
        public MultipartFileContent(
            CoreFileStreamInfo file,
            IReadOnlyDictionary<string,
            IReadOnlyCollection<string>> headers)
            : base(headers)
        {
            File = file;
        }

        /// <summary>
        /// Gets file.
        /// </summary>
        public CoreFileStreamInfo File { get; }

        /// <summary>
        /// Rewind the stream.
        /// </summary>
        public override void Rewind()
        {
            File.FileStream.Position = 0;
        }

        /// <summary>
        /// ToHttpContent.
        /// </summary>
        /// <param name="contentDispositionName">contentDispositionName.</param>
        /// <returns>HttpContent.</returns>
        public override HttpContent ToHttpContent(string contentDispositionName)
        {
            var streamContent = new StreamContent(File.FileStream);
            SetHeaders(contentDispositionName, streamContent.Headers);

            return streamContent;
        }

        /// <summary>
        /// SetHeaders.
        /// </summary>
        /// <param name="contentDispositionName">contentDispositionName.</param>
        /// <param name="headers">headers.</param>
        protected override void SetHeaders(
            string contentDispositionName,
            HttpContentHeaders headers)
        {
            base.SetHeaders(contentDispositionName, headers);

            headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = contentDispositionName,
                FileName = string.IsNullOrWhiteSpace(File.FileName) ? "file" : File.FileName,
            };

            if (!string.IsNullOrEmpty(File.ContentType))
            {
                headers.ContentType = new MediaTypeHeaderValue(File.ContentType);
            }
            else if (!Headers.ContainsKey("content-type"))
            {
                headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
        }
    }
}
