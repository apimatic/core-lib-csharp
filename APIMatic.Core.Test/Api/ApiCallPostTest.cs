using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Api
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
            handlerMock.When($"{LazyGlobalConfiguration.Value.ServerUrl()}{url}")
                    .WithContent(inputString)
                    .WithHeaders(new Dictionary<string, string>
                    {
                        { "content-type", "text/plain; charset=utf-8" },
                        { "accept", "application/json" }
                    })
                    .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .Server(MockServer.Server1)
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
            handlerMock.When($"{LazyGlobalConfiguration.Value.ServerUrl()}{url}")
                    .WithFormData("key 1", inputString)
                    .WithHeaders(new Dictionary<string, string>
                    {
                        { "content-type", "application/x-www-form-urlencoded" },
                        { "accept", "application/json" }
                    })
                    .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .Server(MockServer.Server1)
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Header( h => h.Setup("content-type", "text/plain; charset=utf-8"))
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
