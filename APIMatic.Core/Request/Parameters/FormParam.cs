// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Request.Parameters
{
    public class FormParam : Parameter
    {
        private readonly Dictionary<string, string> encodingHeaders = new Dictionary<string, string>();
        internal FormParam() => typeName = "form";

        public FormParam EncodingHeader(string key, string value)
        {
            encodingHeaders[key.ToLower()] = value;
            return this;
        }

        private bool IsMultipart() => encodingHeaders.TryGetValue("content-type", out var contentType)
            && contentType != "application/x-www-form-urlencoded";

        private void AddEncodingHeaders(Dictionary<string, IReadOnlyCollection<string>> headers)
        {
            if (encodingHeaders.TryGetValue("content-type", out var contentType))
            {
                headers["content-type"] = new[] { contentType };
            }
        }

        private object PrepareFormParameterValue(object formParam)
        {
            var multipartHeaders = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase);
            if (formParam is CoreFileStreamInfo file)
            {
                var defaultFileContentType = string.IsNullOrEmpty(file.ContentType) ? "application/octect-stream" : file.ContentType;
                multipartHeaders["content-type"] = new[] { defaultFileContentType };
                AddEncodingHeaders(multipartHeaders);
                return new MultipartFileContent(file, multipartHeaders);
            }
            if (IsMultipart())
            {
                AddEncodingHeaders(multipartHeaders);
                return new MultipartByteArrayContent(Encoding.ASCII.GetBytes(CoreHelper.JsonSerialize(formParam)), multipartHeaders);
            }
            // handle object and list in form parameter cases here
            return formParam;
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated || value == null)
            {
                return;
            }
            requestBuilder.formParameters.Add(new KeyValuePair<string, object>(key, PrepareFormParameterValue(value)));
        }
    }
}
