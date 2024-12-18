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
using APIMatic.Core.Test.MockTypes.Utilities;
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
        
        [Test]
        public void AddHeaders_ShouldAddHeaders_WhenHeadersNotNull()
        {
            // Arrange
            var coreRequest = new HttpRequest(HttpMethod.Get, "https://myurl.com");
            var headersToAdd = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Authorization", "Bearer token" }
            };

            // Act
            var result = coreRequest.AddHeaders(headersToAdd);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("application/json", result["Content-Type"]);
            Assert.AreEqual("Bearer token", result["Authorization"]);
        }
        
        [Test]
        public void AddQueryParameters_ShouldAddQueryParameters_WhenQueryParametersAreNotNull()
        {
            // Arrange
            var coreRequest = new HttpRequest(HttpMethod.Get, "https://myurl.com");
            var queryParametersToAdd = new Dictionary<string, object>
            {
                { "search", "test" },
                { "limit", 10 }
            };

            // Act
            coreRequest.AddQueryParameters(queryParametersToAdd);

            // Assert
            Assert.AreEqual(2, coreRequest.QueryParameters.Count);
            Assert.AreEqual("test", coreRequest.QueryParameters["search"]);
            Assert.AreEqual(10, coreRequest.QueryParameters["limit"]);
        }
    }
}
