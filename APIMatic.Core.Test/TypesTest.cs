using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Http;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Test.Utilities;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;
using Newtonsoft.Json;
using NUnit.Framework;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class TypesTest : TestBase
    {
        [Test]
        public void MultipartContent_AdditionalHeaders()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data."));
            var file = new CoreFileStreamInfo(memStream, "test - stream.file");
            Dictionary<string, IReadOnlyCollection<string>> multipartHeaders = new(StringComparer.OrdinalIgnoreCase)
            {
                { "myHeader", new[] { "personalHeaderValue" } }
            };
            var fileContent = new MultipartFileContent(file, multipartHeaders);
            var content = fileContent.ToHttpContent("file");
            Assert.AreEqual("Test memory stream data.", Encoding.UTF8.GetString(content.ReadAsByteArrayAsync().Result));
            Assert.AreEqual("form-data; name=file; filename=\"test - stream.file\"", content.Headers.ContentDisposition.ToString());
            Assert.AreEqual("application/octet-stream", content.Headers.ContentType.ToString());
            Assert.AreEqual("personalHeaderValue", content.Headers.GetValues("myHeader").First());
        }

        [Test]
        public void ApiException_CheckResponseCode_WithoutResponse()
        {
            HttpContext context = null;
            var exception = new ApiException("This is an exception", context);

            Assert.AreEqual(-1, exception.ResponseCode);

            context = new HttpContext(new HttpRequest(HttpMethod.Get, "https://myurl.com"), null);
            exception = new ApiException("This is an exception", context);

            Assert.AreEqual(-1, exception.ResponseCode);
        }

        [Test]
        public void ApiException_CheckResponseCode_WithResponseOfEmptyStream()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes(""));
            var response = new HttpResponse(200, new Dictionary<string, string>(), memStream, "Test body");
            var context = new HttpContext(new HttpRequest(HttpMethod.Get, "https://myurl.com"), response);
            var exception = new ApiException("This is an exception", context);

            Assert.AreEqual(200, exception.ResponseCode);
        }

        [Test]
        public void HttpContext_String_Representation()
        {
            var response = new HttpResponse(200, new Dictionary<string, string>(), new MemoryStream(Encoding.UTF8.GetBytes("")), "Test body");
            var request = new HttpRequest(HttpMethod.Get, "https://myurl.com");

            request.AddHeaders(new Dictionary<string, string>
            {
                { "keyA1", "value A1"}
            });
            request.AddHeaders(new Dictionary<string, string>
            {
                { "keyA2", "value A2"}
            });
            request.AddQueryParameters(new Dictionary<string, object>
            {
                { "queryA1", "value A1"},
            });
            request.AddQueryParameters(new Dictionary<string, object>
            {
                { "queryA2", "value A2"}
            });
            var context = new HttpContext(request, response);

            var expected = " Request =  HttpMethod = GET,  QueryUrl = https://myurl.com,  QueryParameters = {\"queryA1\":\"value A1\",\"queryA2\":\"value A2\"},  Headers = {\"keyA1\":\"value A1\",\"keyA2\":\"value A2\"},  FormParameters = ,  Body = ,  Username = ,  Password = , Response =  StatusCode = 200,  Headers = {} RawBody = System.IO.MemoryStream";
            Assert.AreEqual(expected, context.ToString());
        }

        [Test]
        public void JsonObject_String_Representation()
        {
            var jsonObject = JsonObject.FromJsonString(null);
            Assert.AreEqual("null", jsonObject.ToString());

            var expected = "{\"language\":\"csharp\"}";
            jsonObject = JsonObject.FromJsonString(expected);

            var jToken = jsonObject.GetStoredObject();
            Assert.AreEqual(expected, jsonObject.ToString());
            Assert.AreEqual(expected, jToken.ToString(Formatting.None));

            jsonObject = CoreHelper.JsonDeserialize<JsonObject>(expected);
            Assert.AreEqual(expected, CoreHelper.JsonSerialize(jsonObject));
        }

        [Test]
        public void JsonValue_String_Representation()
        {
            var jsonValue = JsonValue.FromArray<string>(null);
            Assert.AreEqual("null", jsonValue.ToString());

            var expected = new List<string> { "java", "csharp" };
            var expectedString = "[\"java\",\"csharp\"]";
            jsonValue = JsonValue.FromArray(expected);

            Assert.AreEqual(expectedString, jsonValue.ToString());
            Assert.AreEqual(expectedString, CoreHelper.JsonSerialize(jsonValue));
            var value = jsonValue.GetStoredObject();
            Assert.AreEqual(expected, value);
            var actualDeserialized = CoreHelper.JsonDeserialize<JsonValue>(expectedString);
            Assert.AreEqual(jsonValue.ToString(), actualDeserialized.ToString());
        }
    }
}
