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
    [TestFixture]
    internal class ApiCallPostFileTest : ApiCallTest
    {
        [Test]
        public void ApiCall_PostFileData_OKResponse()
        {
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data.")))
            {
                //Arrange
                var url = "/apicall/post-file/200";
                var streamName = "test-stream.file";
                var contentType = "application/octet-stream";

                var coreFileStreamInfo = new CoreFileStreamInfo(memStream, streamName);

                var expected = new ServerResponse()
                {
                    Passed = true,
                };

                var reader = new StreamReader(memStream);
                var text = reader.ReadToEnd();

                var content = JsonContent.Create(expected);
                handlerMock.When(GetCompleteUrl(url))
                    .With(req =>
                    {
                        Assert.AreEqual(text, req.Content.ReadAsStringAsync().Result);
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        Assert.AreEqual(contentType, req.Content.Headers.ContentType.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, content);

                var apiCall = CreateApiCall<ServerResponse>()
                    .RequestBuilder(requestBuilderAction => requestBuilderAction
                        .Setup(HttpMethod.Post, url)
                        .Parameters(p => p
                            .Body(b => b.Setup(coreFileStreamInfo))))
                    .ExecuteAsync();

                // Act
                var actual = CoreHelper.RunTask(apiCall);

                // Assert
                Assert.NotNull(actual);
                Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
                Assert.NotNull(actual.Data);
            }
        }

        [Test]
        public void ApiCall_PostFileDataWithFileContentType_OKResponse()
        {
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data.")))
            {
                //Arrange
                var url = "/apicall/post-file-with-file-content/200";
                var streamName = "test-stream.file";
                var contentType = "application/octet-stream";

                var coreFileStreamInfo = new CoreFileStreamInfo(memStream, streamName, contentType);

                var expected = new ServerResponse()
                {
                    Passed = true,
                };

                var reader = new StreamReader(memStream);
                var text = reader.ReadToEnd();

                var content = JsonContent.Create(expected);
                handlerMock.When(GetCompleteUrl(url))
                    .With(req =>
                    {
                        Assert.AreEqual(text, req.Content.ReadAsStringAsync().Result);
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        Assert.AreEqual(contentType, req.Content.Headers.ContentType.ToString());
                        return true;
                    })
                    .WithHeaders(new Dictionary<string, string>
                    {
                        { "content-type", contentType },
                        { "accept", "application/json" }
                    })
                    .Respond(HttpStatusCode.OK, content);

                var apiCall = CreateApiCall<ServerResponse>()
                    .RequestBuilder(requestBuilderAction => requestBuilderAction
                        .Setup(HttpMethod.Post, url)
                        .Parameters(p => p
                            .Body(b => b.Setup(coreFileStreamInfo))))
                    .ExecuteAsync();

                // Act
                var actual = CoreHelper.RunTask(apiCall);

                // Assert
                Assert.NotNull(actual);
                Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
                Assert.NotNull(actual.Data);
            }
        }

        [Test]
        public void ApiCall_PostFileDataWithContentType_OKResponse()
        {
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data.")))
            {
                //Arrange
                var url = "/apicall/post-file-with-content/200";
                var streamName = "test-stream.file";

                var coreFileStreamInfo = new CoreFileStreamInfo(memStream, streamName);

                var expected = new ServerResponse()
                {
                    Passed = true,
                };

                var reader = new StreamReader(memStream);
                var text = reader.ReadToEnd();
                var contentType = "application/octet-stream";

                var content = JsonContent.Create(expected);
                handlerMock.When(GetCompleteUrl(url))
                    .With(req =>
                    {
                        Assert.AreEqual(text, req.Content.ReadAsStringAsync().Result);
                        Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                        Assert.AreEqual(contentType, req.Content.Headers.ContentType.ToString());
                        return true;
                    })
                    .Respond(HttpStatusCode.OK, content);

                var apiCall = CreateApiCall<ServerResponse>()
                    .RequestBuilder(requestBuilderAction => requestBuilderAction
                        .Setup(HttpMethod.Post, url)
                        .Parameters(p => p
                            .Header(h => h.Setup("content-type", "application/octet-stream"))
                            .Body(b => b.Setup(coreFileStreamInfo))))
                    .ExecuteAsync();

                // Act
                var actual = CoreHelper.RunTask(apiCall);

                // Assert
                Assert.NotNull(actual);
                Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
                Assert.NotNull(actual.Data);
            }
        }

        [Test]
        public void ApiCall_PostMultipartForm_OKResponse()
        {
            var stringContent = "Test memory stream data.";
            using (var memStream = new MemoryStream(Encoding.UTF8.GetBytes(stringContent)))
            {
                //Arrange
                var url = "/apicall/post-multipart-form/200";
                var streamName = "test-stream.file";
                var contentType = "application/octet-stream";

                var coreFileStreamInfo = new CoreFileStreamInfo(memStream, streamName, contentType);

                var expected = new ServerResponse()
                {
                    Passed = true,
                };

                var content = JsonContent.Create(expected);
                handlerMock.When(GetCompleteUrl(url))
                    .With(req =>
                    {
                        Assert.AreEqual("multipart/form-data", req.Content.Headers.ContentType.MediaType);
                        var reqestContents = req.Content as MultipartFormDataContent;
                        var firstMultipartContent = reqestContents.Cast<HttpContent>().ElementAt(0);
                        var secondMultipartContent = reqestContents.Cast<HttpContent>().ElementAt(1);
                        Assert.AreEqual(stringContent, firstMultipartContent.ReadAsStringAsync().Result);
                        Assert.AreEqual(stringContent, secondMultipartContent.ReadAsStringAsync().Result);
                        return true;
                    })
                    .WithHeaders(new Dictionary<string, string>
                    {
                        { "accept", "application/json" }
                    })
                    .Respond(HttpStatusCode.OK, content);

                var apiCall = CreateApiCall<ServerResponse>()
                    .RequestBuilder(requestBuilderAction => requestBuilderAction
                        .Setup(HttpMethod.Post, url)
                        .Parameters(p => p
                            .Form(f => f.Setup("Key1", stringContent))
                            .Form(f => f.Setup("file1", coreFileStreamInfo))))
                    .ExecuteAsync();

                // Act
                var actual = CoreHelper.RunTask(apiCall);

                // Assert
                Assert.NotNull(actual);
                Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
                Assert.NotNull(actual.Data);
            }
        }
    }
}
