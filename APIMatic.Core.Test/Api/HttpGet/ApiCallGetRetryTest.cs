﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Test.MockTypes.Exceptions;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace APIMatic.Core.Test.Api.HttpGet
{
    [TestFixture]
    public class ApiCallGetRetryTest : ApiCallTest
    {
        [Test]
        public void ApiCall_GetWithRetryOption_ApiException()
        {
            //Arrange
            var content = "Test retry configuration";
            var url = "/apicall/get-with-retry/exception";

            var stringContent = JsonContent.Create(content);
            stringContent.Headers.Add("Content-Length", content.Length.ToString());

            var retryCounter = -1;
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    retryCounter++;
                    return true;
                })
                .Respond(HttpStatusCode.InternalServerError, stringContent);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .WithRetryOption(RetryOption.EnableForHttpMethod)
                    .Setup(HttpMethod.Post, url))
                .ExecuteAsync();

            // Act
            Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual(numberOfRetries, retryCounter);
        }

        [Test]
        public void ApiCall_GetWithRetryOption_Diable_ApiException()
        {
            //Arrange
            var content = "Test retry configuration";
            var url = "/apicall/get-with-retry-disable/exception";

            var stringContent = JsonContent.Create(content);
            stringContent.Headers.Add("Content-Length", content.Length.ToString());

            var retryCounter = -1;
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    retryCounter++;
                    return true;
                })
                .Respond(HttpStatusCode.InternalServerError, stringContent);

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .WithRetryOption(RetryOption.Disable)
                    .Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act
            Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual(0, retryCounter);
        }

        [Test]
        public void ApiCall_GetWithRetryOptionWithRetryAfterResponse_ApiException()
        {
            //Arrange
            var content = "Test retry configuration with retry after from response.";
            var url = "/apicall/get-with-retry-response-retry-after/exception";

            var stringContent = new StringContent(content);
            stringContent.Headers.Add("Content-Length", content.Length.ToString());

            var retryCounter = -1;
            handlerMock.When(GetCompleteUrl(url))
                .With(req =>
                {
                    retryCounter++;
                    return true;
                })
                .Respond(_ =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = stringContent
                    };
                    response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(1));
                    return response;
                });

            var apiCall = CreateApiCall<ServerResponse>()
                .RequestBuilder(requestBuilderAction => requestBuilderAction
                    .WithRetryOption(RetryOption.EnableForHttpMethod)
                    .Setup(HttpMethod.Get, url))
                .ExecuteAsync();

            // Act
            Assert.Throws<ApiException>(() => CoreHelper.RunTask(apiCall));
            Assert.AreEqual(numberOfRetries, retryCounter);
        }
    }
}
