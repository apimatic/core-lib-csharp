using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;

namespace APIMatic.Core.Test.Api
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
            handlerMock.When($"{LazyGlobalConfiguration.Value.ServerUrl()}{url}")
                    .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ApiResponse<ServerResponse>, ServerResponse>()
                .Server(MockServer.Server1)
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ReturnTypeCreator((httpResponse, result) => new ApiResponse<ServerResponse>(httpResponse.StatusCode, httpResponse.Headers, result)))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTaskSynchronously(apiCall);

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
            handlerMock.When($"{LazyGlobalConfiguration.Value.ServerUrl()}{url}")
                    .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ApiResponse<ServerResponse>, ServerResponse>()
                .Server(MockServer.Server1)
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ContextAdder((response, context) =>
                        {
                            response.Input = content;
                            return response;
                        })
                    .ReturnTypeCreator((httpResponse, result) => new ApiResponse<ServerResponse>(httpResponse.StatusCode, httpResponse.Headers, result)))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTaskSynchronously(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(actual.StatusCode, (int)HttpStatusCode.OK);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(actual.Data.Message, expected.Message);
        }
    }
}
