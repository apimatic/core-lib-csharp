using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Net;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Models;
using NUnit.Framework;
using RichardSzalay.MockHttp;
using APIMatic.Core.Http.Configuration;
using System.Net.Http;
using APIMatic.Core.Utilities;
using System.Net.Http.Headers;

namespace APIMatic.Core.Test.Api.HttpGet
{
    [TestFixture]
    internal class ApiCallGetDynamicError : ApiCallTest
    {
        [Test]
        public void ApiCall_GetDynamicError_Local_501StausCode()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/501/statuscode/";
            var expected = new Dictionary<string, object>();

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.NotImplemented, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("501", CreateErrorCase("NotImplemented error, Code: {$statusCode}.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("NotImplemented error, Code: 501.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_403MissingHeader()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/403/missing/header";
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.Forbidden);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("403", CreateErrorCase("Forbidden: Retry after {$response.header.retry-after} seconds.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Forbidden: Retry after  seconds.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_403Header()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/403/header";
            var expected = new Dictionary<string, object>();

            var content = JsonContent.Create(expected);
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(_ =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                    {
                        Content = content
                    };
                    response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));
                    return response;
                });

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("403", CreateErrorCase("Forbidden: {$response.header.content-type}. Retry after {$response.header.retry-after} seconds.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Forbidden: application/json; charset=utf-8. Retry after 1 seconds.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_501ResponseBody()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/501/responsebody";
            var expected = new Dictionary<string, object>()
            {
                { "body c2", new Dictionary<string, object>
                    {
                        { "field", "This is a field of body in child exception class" },
                        { "integer", 15},
                        { "bool", true}
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
                .Respond(HttpStatusCode.NotImplemented, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("501", CreateErrorCase("Not Implemented error: {$response.body}.", (reason, context) => new Child2Exception(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<Child2Exception>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Not Implemented error: {\"body c2\":{\"field\":\"This is a field of body in child exception class\",\"integer\":15,\"bool\":true}}.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_501ResponseBodyJpointer()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/501/responsebody/jpointer";
            var expected = new Dictionary<string, object>()
            {
                { "body c2", new Dictionary<string, object>
                    {
                        { "field", "This is a field of body in child exception class" },
                        { "integer", 15},
                        { "bool", true}
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
                .Respond(HttpStatusCode.NotImplemented, content);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("501", CreateErrorCase("Not Implemented error: {$response.body#}, {$response.body#/body c2/integer}, {$response.body#/body c2/bool}, {$response.body#/body c2/na}, {$response.body#/body c2}.", (reason, context) => new Child2Exception(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<Child2Exception>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Not Implemented error: , 15, true, , {\"field\":\"This is a field of body in child exception class\",\"integer\":15,\"bool\":true}.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_500ResponseBodyEmptyResponse()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/500/responsebody/emptyresponse";

            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.NotImplemented);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("501", CreateErrorCase("Internal server error: {$response.body}.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Internal server error: .", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_402Override()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/402/override";

            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.PaymentRequired);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("402", CreateErrorCase("Conflict: {$statusCode}.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Conflict: 402.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_5XX()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/5xx/range";

            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.NotImplemented);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("5XX", CreateErrorCase("Server error: {$statusCode}.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Server error: 501.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDynamicError_Local_4XX()
        {
            //Arrange
            var url = "/apicall/error/dynamic/local/409/range";

            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.Conflict);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("5XX", CreateErrorCase("Internal server error: {$response.body}.", (reason, context) => new ApiException(reason, context), true))
                    .ErrorCase("4XX", CreateErrorCase("Conflict: {$statusCode}.", (reason, context) => new ApiException(reason, context), true))
                    .ErrorCase("0", CreateErrorCase("Default error: {$statusCode}.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Conflict: 409.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetDefaultError_Local_5XX()
        {
            //Arrange
            var url = "/apicall/error/default/local/5xx/range";

            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.Conflict);

            var apiCall = CreateSimpleApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ResponseHandler(responseHandlerAction => responseHandlerAction
                    .ErrorCase("5XX", CreateErrorCase("Internal server error: {$response.body}.", (reason, context) => new ApiException(reason, context), true))
                    .ErrorCase("0", CreateErrorCase("Default local error: {$statusCode}.", (reason, context) => new ApiException(reason, context), true)))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("Default local error: 409.", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetNotOKError_301()
        {
            //Arrange
            var url = "/apicall/error/notok/local/301";

            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.Moved);

            var apiCall = CreateSimpleApiCallWithoutErrors<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("HTTP Response Not OK", exp.Message);
            Assert.Null(exp.ExceptionData);
        }

        [Test]
        public void ApiCall_GetNotOKError_404()
        {
            //Arrange
            var url = "/apicall/error/notok/local/404";

            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    Assert.AreEqual(ContentType.JSON.GetValue(), req.Headers.Accept.ToString());
                    return true;
                })
                .Respond(HttpStatusCode.NotFound);

            var apiCall = CreateSimpleApiCallWithoutErrors<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction.Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act and Assert
            var exp = Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual("HTTP Response Not OK", exp.Message);
            Assert.Null(exp.ExceptionData);
        }
    }
}
