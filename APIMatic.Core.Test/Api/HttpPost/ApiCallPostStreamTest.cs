using System.IO;
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
    internal class ApiCallPostStreamTest : ApiCallTest
    {
        [Test]
        public void ApiCall_PostBodyStream_OKResponse()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data."));
            //Arrange
            var url = "/apicall/post-stream-body/200";
            var contentType = "application/octet-stream";

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
                    Assert.AreEqual(contentType, req.Content.Headers.ContentType.MediaType);
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(memStream))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(actual.Data);
        }

        [Test]
        public void ApiCall_PostBodyStreamWithWrongContentType_OKResponse()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data."));
            //Arrange
            var url = "/apicall/post-stream-body-wrong-content/200";

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
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Header(h => h.Setup("content-type", "application/"))
                        .Body(b => b.Setup(memStream))))
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
