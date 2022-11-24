using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using APIMatic.Core.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using static System.Net.Mime.MediaTypeNames;

namespace APIMatic.Core.Test.Api.Post
{
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
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }

        [Test]
        public void ApiCall_PostBodyWithContentType_OKResponse()
        {
            //Arrange
            var text = "{\"name\":\"PostBodyWithContentType\"}";
            var url = "/apicall/body-post-with-content/200";

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
                        .Header(h => h.Setup("content-type", "application/json; charset=utf-8"))
                        .Body(b => b.Setup(text))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
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
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
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
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(actual.Data);
        }

        [Test]
        public void ApiCall_PostFormData_OKResponse()
        {
            //Arrange
            var text = "Post form data.";
            var url = "/apicall/form-post/200";
            var expectedFormData = "key+1=Post+form+data.";

            var expected = new ServerResponse()
            {
                Message = text,
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
                        .Form(form => form
                            .Setup("key 1", text))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }
    }
}
