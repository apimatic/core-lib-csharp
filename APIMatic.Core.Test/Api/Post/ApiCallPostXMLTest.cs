using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading;
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
    internal class ApiCallPostXMLTest : ApiCallTest
    {
        [Test]
        public void ApiCall_PostXmlLong_OKResponse()
        {
            //Arrange
            long body = 100;
            var url = "/apicall/post-xml-long/200";

            var expected = new ServerResponse()
            {
                Message = XmlUtility.ToXml(body, "long"),
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(expected.Message, req.Content.ReadAsStringAsync().Result);
                    return true;
                })
            .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
              .RequestBuilder(requestBuilderAction => requestBuilderAction
                  .Setup(HttpMethod.Post, url)
                  .XmlBodySerializer(serializer => XmlUtility.ToXml(body, "long")))
              .ResponseHandler(responseHandlerAction => responseHandlerAction
                  .Deserializer(response => CoreHelper.JsonDeserialize<ServerResponse>(response)))
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
        public void ApiCall_PostXmlString_OKResponse()
        {
            //Arrange
            string body = "This is a xml body.";
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
                    Assert.AreEqual(expected.Message, req.Content.ReadAsStringAsync().Result);
                    return true;
                })
            .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
              .RequestBuilder(requestBuilderAction => requestBuilderAction
                  .Setup(HttpMethod.Post, url)
                  .XmlBodySerializer(serializer => XmlUtility.ToXml(body, "string")))
              .ResponseHandler(responseHandlerAction => responseHandlerAction
                  .Deserializer(response => CoreHelper.JsonDeserialize<ServerResponse>(response)))
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
