using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Test.MockTypes.Models.Containers;
using APIMatic.Core.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Api.HttpPost
{
    [TestFixture]
    internal class ApiCallPostContainerTest : ApiCallTest
    {
        [Test]
        public void ApiCall_PostModelAsContainerData_OKResponse()
        {
            var oneOfAtomOrbit = CustomOneOfContainer.FromAtom(new Atom()
            {
                Name = "Hydrogen",
                NumberOfProtons = 1,
                NumberOfElectrons = 1,
            });
            var expected = new ServerResponse()
            {
                Passed = true,
            };
            var url = "/apicall/post-container-model/200";
            var contentType = "application/json; charset=utf-8";

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(CoreHelper.JsonSerialize(oneOfAtomOrbit), req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(contentType, req.Content.Headers.ContentType.ToString());
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(oneOfAtomOrbit))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.AreEqual(expected, actual.Data);
        }

        [Test]
        public void ApiCall_PostStringAsContainerData_OKResponse()
        {
            var anyOfStringDouble = NativeAnyOfContainer.FromMString("some string as request body");
            var expected = new ServerResponse()
            {
                Passed = true,
            };
            var url = "/apicall/post-container-string/200";
            var contentType = "text/plain; charset=utf-8";

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(anyOfStringDouble.ToString(), req.Content.ReadAsStringAsync().Result);
                    Assert.AreEqual(contentType, req.Content.Headers.ContentType.ToString());
                    Assert.AreEqual("application/json", req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .Setup(HttpMethod.Post, url)
                    .Parameters(p => p
                        .Body(b => b.Setup(anyOfStringDouble))))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.AreEqual(expected, actual.Data);
        }
    }
}
