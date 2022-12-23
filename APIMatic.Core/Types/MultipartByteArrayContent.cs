// <copyright file="MultipartByteArrayContent.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System.Collections.Generic;
using System.Net.Http;

namespace APIMatic.Core.Types
{
    /// <summary>
    /// MultipartByteArrayContent.
    /// </summary>
    internal class MultipartByteArrayContent : MultipartContent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipartByteArrayContent"/> class.
        /// </summary>
        /// <param name="byteArray"> byteArray.</param>
        /// <param name="headers"> headers.</param>
        public MultipartByteArrayContent(byte[] byteArray, IReadOnlyDictionary<string, IReadOnlyCollection<string>> headers)
            : base(headers)
        {
            ByteArray = byteArray;
        }

        /// <summary>
        /// Gets byte array.
        /// </summary>
        public byte[] ByteArray { get; }

        /// <summary>
        /// Rewind the stream.
        /// </summary>
        public override void Rewind() { }

        /// <summary>
        /// ToHttpContent.
        /// </summary>
        /// <param name="contentDispositionName"> contentDispositionName .</param>
        /// <returns>HttpContent.</returns>
        public override HttpContent ToHttpContent(string contentDispositionName)
        {
            var byteArrayContent = new ByteArrayContent(ByteArray);
            SetHeaders(contentDispositionName, byteArrayContent.Headers);

            return byteArrayContent;
        }
    }
}
