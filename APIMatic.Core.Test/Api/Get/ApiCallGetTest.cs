using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using System.Collections.Generic;

namespace APIMatic.Core.Test.Api.Get
{
    [TestFixture]
    public class ApiCallGetTest : ApiCallTest
    {
        [Test]
        public void ApiCall_Get_OKResponse()
        {
            //Arrange
            var inputString = "Get response.";
            var url = "/apicall/get/200";

            var expected = new ServerResponse()
            {
                Message = inputString,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data, expected);
        }

        [Test]
        public void ApiCall_GetWithContext_OKResponse()
        {
            //Arrange
            var inputString = "Get response with context.";
            var url = "/apicall/get-with-context/200";

            var expected = new ServerResponse()
            {
                Message = inputString,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ContextAdder((response, context) =>
                        {
                            response.Input = content;
                            return response;
                        }))
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
