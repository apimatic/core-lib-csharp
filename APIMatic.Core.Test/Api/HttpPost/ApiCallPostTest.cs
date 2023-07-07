using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Api.HttpPost
{
    [TestFixture]
    internal class ApiCallPostTest : ApiCallTest
    {
        [Test]
        public void ApiCall_PostBody_OKResponse()
        {
            //Arrange
            var text = "Post body response.";
            var url = "/apicall/body-post/200";

            var expected = new ServerResponse()
            {
                Message = text,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(text, req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("text/plain", req.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual("utf-8", req.Content.Headers.ContentType.CharSet);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(text))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }

        [Test]
        public void ApiCall_PostMultipleBody_OKResponse()
        {
            //Arrange
            var text1 = "Post body response.";
            var text2 = "Second item.";
            var expectedResponse = CoreHelper.JsonSerialize(new Dictionary<string, object>
            {
                { "key1", text1 },
                { "key2", text2 },
            });
            var url = "/apicall/multiple-body-post/200";

            var expected = new ServerResponse()
            {
                Message = expectedResponse,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(expectedResponse, req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("application/json", req.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual("utf-8", req.Content.Headers.ContentType.CharSet);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup("key1", text1))
                        .Body(b => b.Setup("key2", text2))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }

        [Test]
        public void ApiCall_PostBodyNonScalar_OKResponse()
        {
            //Arrange
            int[] arr = { 1, 2, 3 };
            var url = "/apicall/body-post-non-scalar/200";

            var expected = new ServerResponse()
            {
                Message = CoreHelper.JsonSerialize(arr),
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(CoreHelper.JsonSerialize(arr), req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("application/json", req.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual("utf-8", req.Content.Headers.ContentType.CharSet);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(arr))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(expected.Message, actual.Data.Message);
        }

        [Test]
        public void ApiCall_PostBodyWithContentType_OKResponse()
        {
            //Arrange
            var bodyObject = new Dictionary<string, object>
            {
                { "name", "PostBodyWithContentType" }
            };
            var url = "/apicall/body-post-with-content/200";

            var expected = new ServerResponse()
            {
                Message = CoreHelper.JsonSerialize(bodyObject),
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(CoreHelper.JsonSerialize(bodyObject), req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("application/json", req.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual("utf-8", req.Content.Headers.ContentType.CharSet);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(bodyObject))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(expected.Message, actual.Data.Message);
        }

        [Test]
        public void ApiCall_PostBodyWithEmptyContentType_OKResponse()
        {
            //Arrange
            var text = "{\"name\":\"PostBodyWithEmptyContentType\"}";
            var url = "/apicall/body-post-with-empty-content/200";

            var expected = new ServerResponse()
            {
                Message = text,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(text, req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("text/plain", req.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual("utf-8", req.Content.Headers.ContentType.CharSet);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Header(h => h.Setup("content-type", string.Empty))
                        .Body(b => b.Setup(text))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }

        [Test]
        public void ApiCall_PostBinaryBody_OKResponse()
        {
            //Arrange
            var data = "Test binary data.";
            var binaryData = Encoding.UTF8.GetBytes(data);
            var url = "/apicall/post-binary-body/200";

            var expected = new ServerResponse()
            {
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(data, req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("application/octet-stream", req.Content.Headers.ContentType.MediaType);
                    Assert.IsNull(req.Content.Headers.ContentType.CharSet);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(binaryData))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
        }

        [Test]
        public void ApiCall_PostParameters_UnValidated()
        {
            //Arrange
            var url = "/apicall/form-post/unvalidated";

            var expected = new ServerResponse()
            {
                Message = "Passed",
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url)).Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Form(f => f.Setup("non-null-item", null).Required())))
                .ExecuteAsync();

            var ex = Assert.Throws<ArgumentNullException>(() => CoreHelper.RunTask(apiCall));
            Assert.True(ex.Message.Contains("Missing required form field: non-null-item"));
        }

        [Test]
        public void ApiCall_PostParameters_NullKey()
        {
            //Arrange
            var url = "/apicall/form-post/null/key";

            var expected = new ServerResponse()
            {
                Message = "Passed",
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url)).Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Form(f => f.Setup(null, "some value"))))
                .ExecuteAsync();

            var ex = Assert.Throws<ArgumentNullException>(() => CoreHelper.RunTask(apiCall));
            Assert.True(ex.Message.Contains("Missing required `key` for type: form"));
        }

        [Test]
        public void ApiCall_PostParameters_SerializeError()
        {
            //Arrange
            var url = "/apicall/form-post/serialize/error";

            var expected = new ServerResponse()
            {
                Message = "Passed",
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url)).Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Form(f => f.Setup("formKey", "some value").Serializer(v => throw new Exception("issue while serializing")))))
                .ExecuteAsync();

            var ex = Assert.Throws<InvalidOperationException>(() => CoreHelper.RunTask(apiCall));
            Assert.True(ex.Message.Contains("Unable to serialize field: formKey, Due to:\nissue while serializing"));
        }

        [Test]
        public void ApiCall_PostFormData_OKResponse()
        {
            //Arrange
            var text1 = "Post form data.";
            var text2 = "Value in KeyA";
            var text3 = "Value in KeyB";
            var url = "/apicall/form-post/200";
            var expectedFormData = "key+1=Post+form+data.&keyA=Value+in+KeyA&keyB=Value+in+KeyB";

            var expected = new ServerResponse()
            {
                Message = text1 + text2 + text3,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(expectedFormData, req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual("application/x-www-form-urlencoded", req.Content.Headers.ContentType.MediaType);
                    Assert.IsNull(req.Content.Headers.ContentType.CharSet);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Header(h => h.Setup("content-type", "text/plain; charset=utf-8"))
                        .Form(form => form.Setup("key 1", text1))
                        .AdditionalForms(af => af.Setup(new Dictionary<string, object>
                        {
                            { "keyA", text2 },
                            { "keyB", text3 }
                        }))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }

        [Test]
        public void ApiCall_PostQueryData_OKResponse()
        {
            //Arrange
            var text1 = "Post form data.";
            var text2 = "Value in KeyA";
            var text3 = "Value in KeyB";
            var url = "/apicall/query-post/200";
            var queryString = "?key%201=Post%20form%20data.&keyA=Value%20in%20KeyA&keyB=Value%20in%20KeyB";
            var expectedQueryString = "?key 1=Post form data.&keyA=Value in KeyA&keyB=Value in KeyB";

            var expected = new ServerResponse()
            {
                Message = text1 + text2 + text3,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url + queryString))
                .With(req =>
                {
                    Assert.AreEqual(GetCompleteUrl(url + expectedQueryString), req.RequestUri.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Query(q => q.Setup("key 1", text1))
                        .AdditionalQueries(aq => aq.Setup(new Dictionary<string, object>
                        {
                            { "keyA", text2 },
                            { "keyB", text3 }
                        }))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }

        [Test]
        public void ApiCall_PostNullHeaderValue_OKResponse()
        {
            //Arrange
            var text = "Post null header value.";
            var url = "/apicall/null-header-post/200";
            var headerKey = "null-key";

            var expected = new ServerResponse()
            {
                Message = text,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(text, req.Content.ReadAsStringAsync().Result);
                    Assert.IsTrue(req.Headers.Contains(headerKey));
                    Assert.AreEqual(string.Empty, req.Headers.GetValues(headerKey).FirstOrDefault());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(text))
                        .Header(h => h.Setup(headerKey, null))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }
    }
}
