// <copyright file="HttpClientWrapper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using APIMatic.Core.Http.Client.Configuration;
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
            && contentType != ContentType.FORM_URL_ENCODED.GetValue();

        private void AddEncodingHeaders(Dictionary<string, IReadOnlyCollection<string>> headers)
        {
            if (encodingHeaders.TryGetValue("content-type", out var contentType))
            {
                headers["content-type"] = new[] { contentType };
            }
        }

        private IEnumerable<KeyValuePair<string, object>> PrepareFormParameters(ArraySerialization arraySerialization)
        {
            var multipartHeaders = new Dictionary<string, IReadOnlyCollection<string>>(StringComparer.OrdinalIgnoreCase);
            if (value is CoreFileStreamInfo file)
            {
                var defaultFileContentType = string.IsNullOrEmpty(file.ContentType) ? ContentType.BINARY.GetValue() : file.ContentType;
                multipartHeaders["content-type"] = new[] { defaultFileContentType };
                AddEncodingHeaders(multipartHeaders);
                return new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(key, new MultipartFileContent(file, multipartHeaders))
                };
            }
            if (IsMultipart())
            {
                AddEncodingHeaders(multipartHeaders);
                return new List<KeyValuePair<string, object>>
                {
                    new KeyValuePair<string, object>(key, new MultipartByteArrayContent(Encoding.ASCII.GetBytes(CoreHelper.JsonSerialize(value)), multipartHeaders))
                };
            }
            return CoreHelper.PrepareFormFieldsFromObject(key, value, arraySerializationFormat: arraySerialization).Where(kv => kv.Value != null);
        }

        internal override void Apply(RequestBuilder requestBuilder)
        {
            if (!validated || value == null)
            {
                return;
            }
            requestBuilder.formParameters.AddRange(PrepareFormParameters(requestBuilder.ArraySerialization));
        }
    }
}
