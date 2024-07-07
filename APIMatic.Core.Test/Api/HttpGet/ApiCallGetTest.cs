using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Http.Response;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Types;
using APIMatic.Core.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Api.HttpGet
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
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
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
            content.Headers.Add("myHeader", inputString.Length.ToString());
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
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
            Assert.AreEqual("26", actual.Headers["myHeader"]);
            Assert.NotNull(actual.Data);
            Assert.AreEqual(expected.Message, actual.Data.Message);
            Assert.AreEqual(content, actual.Data.Input);
        }

        [Test]
        public void ApiCall_GetWithDisableContentType_OKResponse()
        {
            //Arrange
            var inputString = "Get response with context.";
            var url = "/apicall/get-with-disable-content/200";

            var expected = new ServerResponse()
            {
                Message = inputString,
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.IsNull(req.Content);
                    Assert.AreEqual(0, req.Headers.Accept.Count);
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .DisableContentType()
                    .Setup(HttpMethod.Get, url))
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
        public void ApiCall_GetWithBasicAuthe_OKResponse()
        {
            //Arrange
            var url = "/apicall/get-with-basic-auth/200";
            var basicAuthHeaderParameter = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_basicAuthUserName}:{_basicAuthPassword}"));
            var basicAuthHeaderScheme = "Basic";

            var expected = new ServerResponse()
            {
                Passed = true,
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(basicAuthHeaderParameter, req.Headers.Authorization.Parameter);
                    Assert.AreEqual(basicAuthHeaderScheme, req.Headers.Authorization.Scheme);
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCall<ServerResponse>()
              .RequestBuilder(requestBuilderAction => requestBuilderAction
                  .Setup(HttpMethod.Get, url)
              .WithAuth("basic"))
              .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual((int)HttpStatusCode.OK, actual.StatusCode);
        }

        [Test]
        public void ApiCall_Null_ReturnTypeCreater()
        {
            //Arrange
            var url = "/apicall/null-return-type-creator/get-scalar/200";
            var expected = "This is string response";

            var content = new StringContent(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.IsNull(req.Content);
                    Assert.AreEqual(0, req.Headers.Accept.Count);
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateApiCallWithoutReturnTypeCreator<string>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<InvalidOperationException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Unable to transform System.String into APIMatic.Core.Test.MockTypes.Http.Response.ApiResponse`1[System.String]. ReturnTypeCreator is not provided.", exp.Message);
        }

        [Test]
        public void ApiCall_GetScalar_String()
        {
            //Arrange
            var url = "/apicall/get-scalar-string/200";
            var expected = "This is string response";

            var content = new StringContent(expected.ToString(), Encoding.UTF8, "text/plain");
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.IsNull(req.Content);
                    Assert.AreEqual(0, req.Headers.Accept.Count);
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<string>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApiCall_GetXmlScalar_String()
        {
            //Arrange
            var url = "/apicall/xml/get-scalar-string/200";
            var expected = "This is string response";

            var content = new StringContent(XmlUtility.ToXml(expected), Encoding.UTF8, ContentType.XML.GetValue());
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.XML.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<string>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .XmlResponse()
                    .Deserializer(deserializer => XmlUtility.FromXml<string>(deserializer)))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApiCall_GetScalar_Bool()
        {
            //Arrange
            var url = "/apicall/get-scalar-bool/200";
            var expected = false;

            var content = new StringContent(expected.ToString(), Encoding.UTF8, "text/plain");
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.IsNull(req.Content);
                    Assert.AreEqual(0, req.Headers.Accept.Count);
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<bool>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                  .Deserializer(response => bool.Parse(response)))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApiCall_GetXmlScalar_Bool()
        {
            //Arrange
            var url = "/apicall/xml/get-scalar-bool/200";
            var expected = false;

            var content = new StringContent(XmlUtility.ToXml(expected), Encoding.UTF8, ContentType.XML.GetValue());
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.XML.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<bool>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .XmlResponse()
                    .Deserializer(deserializer => XmlUtility.FromXml<bool>(deserializer)))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApiCall_GetScalar_Date()
        {
            //Arrange
            var url = "/apicall/get-scalar-date/200";
            var expected = new DateTime(2022, 12, 14);

            var content = new StringContent(expected.ToString(), Encoding.UTF8, "text/plain");
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.IsNull(req.Content);
                    Assert.AreEqual(0, req.Headers.Accept.Count);
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<DateTime>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                  .Deserializer(response => DateTime.Parse(response)))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApiCall_GetVoid()
        {
            //Arrange
            var url = "/apicall/get-void/200";

            var content = JsonContent.Create("Test body");
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(0, req.Headers.Accept.Count);
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<VoidType>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.Null(actual);
        }

        [Test]
        public void ApiCall_GetStream()
        {
            //Arrange
            var url = "/apicall/get-stream/200";
            var expected = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data."));

            var content = new StreamContent(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<Stream>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ApiCall_GetHttpResponse()
        {
            //Arrange
            var url = "/apicall/get-http-response/200";
            var expected = new HttpResponse(200, new Dictionary<string, string>(), null, "Test body");

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.OK, content);

            var apiCall = CreateSimpleApiCall<HttpResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act
            var actual = CoreHelper.RunTask(apiCall);

            // Assert
            Assert.NotNull(actual);
            Assert.AreEqual(expected.StatusCode, actual.StatusCode);
            Assert.AreEqual(expected.Headers, actual.Headers);
            Assert.AreEqual(expected.Body, actual.Body);
        }

        [Test]
        public void ApiCall_GetError_Global_400()
        {
            //Arrange
            var url = "/apicall/error/global/400";
            var expected = new Dictionary<string, object>()
            {
                { "data", "This is some additional data" },
                { "body c1", new Dictionary<string, object>
                    {
                        { "field", "This is a field of body in child exception class" }
                    }
                }
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.BadRequest, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<Child1Exception>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("400 Global Child 1", exp.Message);
            Assert.AreEqual("This is some additional data", exp.ExceptionData);
            Assert.AreEqual("This is a field of body in child exception class", exp.BodyC1["field"]);
        }

        [Test]
        public void ApiCall_GetError_Local_400()
        {
            //Arrange
            var url = "/apicall/error/local/400";
            var expected = new Dictionary<string, object>()
            {
                { "data", "This is some additional data" },
                { "body c1", new Dictionary<string, object>
                    {
                        { "field", "This is a field of body in child exception class" }
                    }
                }
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.BadRequest, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("400", CreateErrorCase("400 Local ApiException", (reason, context) => new ApiException(reason, context))))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("400 Local ApiException", exp.Message);
            Assert.AreEqual("This is some additional data", exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetError_Local_Default0()
        {
            //Arrange
            var url = "/apicall/error/local/default0";
            var expected = new Dictionary<string, object>()
            {
                { "body c2", new Dictionary<string, object>
                    {
                        { "field", "This is a field of body in child exception class" }
                    }
                }
            };

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.InternalServerError, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("0", CreateErrorCase("This is a default exception", (reason, context) => new Child2Exception(reason, context))))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<Child2Exception>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("This is a default exception", exp.Message);
            Assert.AreEqual("This is a field of body in child exception class", exp.BodyC2["field"]);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetError_404()
        {
            //Arrange
            var url = "/apicall/error/404";
            var expected = new Dictionary<string, object>();

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.NotFound, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("404 Global", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetError_NullOn404()
        {
            //Arrange
            var url = "/apicall/error/null/on/404";
            var expected = new Dictionary<string, object>();

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.NotFound, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction.NullOn404())
                .ExecuteAsync();

            // Act and Assert
            var actual = CoreHelper.RunTask(apiCall);
            Assert.Null(actual);
        }

        [Test]
        public void ApiCall_GetModel_MissingContent()
        {
            //Arrange
            var url = "/apicall/get/model/missingContent";

            handlerMock.When(GetCompleteUrl(url))
                .Respond(HttpStatusCode.NoContent, new ByteArrayContent(Array.Empty<byte>()));

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act and Assert
            var actual = CoreHelper.RunTask(apiCall);
            Assert.Null(actual);
        }

        [Test]
        public void ApiCall_GetString_MissingContent()
        {
            //Arrange
            var url = "/apicall/get/string/missingContent";

            handlerMock.When(GetCompleteUrl(url))
                .Respond(HttpStatusCode.NoContent, new ByteArrayContent(Array.Empty<byte>()));

            var apiCall = CreateSimpleApiCall<string>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act and Assert
            var actual = CoreHelper.RunTask(apiCall);
            Assert.Null(actual);
        }

        [Test]
        public void ApiCall_GetNumber_MissingContent()
        {
            //Arrange
            var url = "/apicall/get/number/missingContent";

            handlerMock.When(GetCompleteUrl(url))
                .Respond(HttpStatusCode.NoContent, new ByteArrayContent(Array.Empty<byte>()));

            var apiCall = CreateSimpleApiCall<int>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(resHandlerAction => resHandlerAction.Deserializer(res => int.Parse(res)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<FormatException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Input string was not in a correct format.", exp.Message);
        }

        [Test]
        public void ApiCall_GetNullableNumber_MissingContent()
        {
            //Arrange
            var url = "/apicall/get/nullableNumber/missingContent";

            handlerMock.When(GetCompleteUrl(url))
                .Respond(HttpStatusCode.NoContent, new ByteArrayContent(Array.Empty<byte>()));

            var apiCall = CreateSimpleApiCall<int?>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(resHandlerAction => resHandlerAction.Deserializer(res => int.Parse(res)))
                .ExecuteAsync();

            // Act and Assert
            var actual = CoreHelper.RunTask(apiCall);
            Assert.Null(actual);
        }

        [Test]
        public void ApiCall_GetNullableNumber_WithContent()
        {
            //Arrange
            var url = "/apicall/get/nullableNumber/withContent";

            handlerMock.When(GetCompleteUrl(url))
                .Respond(HttpStatusCode.OK, new StringContent("123"));

            var apiCall = CreateSimpleApiCall<int?>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(resHandlerAction => resHandlerAction.Deserializer(res => int.Parse(res)))
                .ExecuteAsync();

            // Act and Assert
            var actual = CoreHelper.RunTask(apiCall);
            Assert.AreEqual(123, actual);
        }
    }
}
