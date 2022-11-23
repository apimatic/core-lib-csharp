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

namespace APIMatic.Core.Test.Api.Post
{
    internal class ApiCallPostTest : ApiCallTest
    {
        [Test]
        public void ApiCall_PostBody_OKResponse()
        {
            //Arrange
            var inputString = "Post body response.";
            var url = "/apicall/body-post/200";

            var expected = new ServerResponse()
            {
                Message = inputString,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .WithContent(inputString)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "content-type", "text/plain; charset=utf-8" },
                    { "accept", "application/json" }
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(inputString))))
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
            var inputString = "{\"name\":\"PostBodyWithContentType\"}";
            var url = "/apicall/body-post-with-content/200";

            var expected = new ServerResponse()
            {
                Message = inputString,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .WithContent(inputString)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "content-type", "application/json; charset=utf-8" },
                    { "accept", "application/json" }
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Header(h => h.Setup("content-type", "application/json; charset=utf-8"))
                        .Body(b => b.Setup(inputString))))
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
            var inputString = "{\"name\":\"PostBodyWithEmptyContentType\"}";
            var url = "/apicall/body-post-with-empty-content/200";

            var expected = new ServerResponse()
            {
                Message = inputString,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .WithContent(inputString)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "content-type", "text/plain; charset=utf-8" },
                    { "accept", "application/json" }
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Header(h => h.Setup("content-type", string.Empty))
                        .Body(b => b.Setup(inputString))))
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
                .With(req => req.Content.Headers.ContentType.MediaType.Equals("application/octet-stream"))
                .WithContent(data)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "accept", "application/json" }
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
            var inputString = "Post form data.";
            var url = "/apicall/form-post/200";

            var expected = new ServerResponse()
            {
                Message = inputString,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .WithFormData("key 1", inputString)
                .WithHeaders(new Dictionary<string, string>
                {
                    { "content-type", "application/x-www-form-urlencoded" },
                    { "accept", "application/json" }
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Header(h => h.Setup("content-type", "text/plain; charset=utf-8"))
                        .Form(form => form
                            .Setup("key 1", inputString))))
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
