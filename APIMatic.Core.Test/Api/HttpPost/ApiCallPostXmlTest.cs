using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Api.HttpPost
{
    [TestFixture]
    internal class ApiCallPostXmlTest : ApiCallTest
    {
        [Test]
        public void ApiCall_PostXmlLong_OKResponse()
        {
            //Arrange
            const long body = 100;
            const string url = "/apicall/post-xml-long/200";

            var expected = new ServerResponse()
            {
                Message = XmlUtility.ToXml(body, "long"),
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(expected.Message, req.Content?.ReadAsStringAsync().Result);
                    return true;
                })
            .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
              .RequestBuilder(requestBuilderAction => requestBuilderAction
                  .Setup(HttpMethod.Post, url)
                  .Parameters(p => p
                      .Body(b => b.Setup(body)))
                  .XmlBodySerializer(xmlSerializer => XmlUtility.ToXml(xmlSerializer, "long")))
              .ResponseHandler(responseHandlerAction => responseHandlerAction
                  .Deserializer(response => CoreHelper.JsonDeserialize<ServerResponse>(response)))
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
        public void ApiCall_PostXmlString_OKResponse()
        {
            //Arrange
            var body = "This is a xml body.";
            var url = "/apicall/post-xml-string/200";

            var expected = new ServerResponse()
            {
                Message = XmlUtility.ToXml(body, "string"),
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(expected.Message, req.Content?.ReadAsStringAsync().Result);
                    return true;
                })
            .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
              .RequestBuilder(requestBuilderAction => requestBuilderAction
                  .Setup(HttpMethod.Post, url)
                  .Parameters(p => p
                      .Body(b => b.Setup(body)))
                  .XmlBodySerializer(xmlSerializer => XmlUtility.ToXml(xmlSerializer, "string")))
              .ResponseHandler(responseHandlerAction => responseHandlerAction
                  .Deserializer(response => CoreHelper.JsonDeserialize<ServerResponse>(response)))
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
