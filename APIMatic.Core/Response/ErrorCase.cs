using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Json.Pointer;
using APIMatic.Core.Types.Sdk;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace APIMatic.Core.Response
{
    /// <summary>
    /// Used to generate an ApiException from the HttpClient response
    /// </summary>
    /// <typeparam name="Request"> Class Type that holds http request info </typeparam>
    /// <typeparam name="Response"> Class Type that holds http response info </typeparam>
    /// <typeparam name="Context"> Class Type that holds http context info </typeparam>
    /// <typeparam name="ApiException"> Class Type that holds BaseException info </typeparam>
    public class ErrorCase<Request, Response, Context, ApiException>
        where Request : CoreRequest
        where Response : CoreResponse
        where Context : CoreContext<Request, Response>
        where ApiException : CoreApiException<Request, Response, Context>
    {
        private readonly bool _isErrorTemplate;
        private readonly string _reason;
        private readonly Func<string, Context, ApiException> _error;
        private readonly string _errorTemplateMatcher = "\\{\\$(.*?)\\}";

        /// <summary>
        /// Creates an instance of <see cref="ErrorCase"/> using the error reason and Func delegate which 
        /// creates the respective exception.
        /// </summary>
        /// <param name="reason">The error reason to create an exception message.</param>
        /// <param name="error">The Func delegate to create an api exception.</param>
        /// <param name="isErrorTemplate">Flags the error reason as a template message.</param>
        public ErrorCase(string reason, Func<string, Context, ApiException> error, bool isErrorTemplate)
        {
            _reason = reason;
            _error = error;
            _isErrorTemplate = isErrorTemplate;
        }

        /// <summary>
        /// Creates the ApiException with the provided error reason.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>ApiException with the provided reason.</returns>
        internal ApiException GetError(Context httpContext)
        {
            if (_isErrorTemplate && Regex.IsMatch(_reason, _errorTemplateMatcher))
            {
                return GetTemplateFormattedError(httpContext);
            }
            return _error(_reason, httpContext);
        }

        /// <summary>
        /// Creates an ApiException with the templated error reason.
        /// </summary>
        /// <param name="httpContext">The HttpContext object.</param>
        /// <returns>ApiException.</returns>
        private ApiException GetTemplateFormattedError(Context httpContext)
        {
            string reason = _reason;

            reason = FormatStatusCodeFromErrorTemplate(reason, httpContext.Response.StatusCode);
            reason = FormatResponseHeaderFromErrorTemplate(reason, httpContext.Response.Headers);
            reason = FormatResponseBodyFromErrorTemplate(reason, httpContext.Response);

            return _error(reason, httpContext);
        }

        /// <summary>
        /// Applies the status code returned by the api call to the error template.
        /// </summary>
        /// <param name="reason">The error message reason.</param>
        /// <param name="statusCode">The status code returned by the api call.</param>
        /// <returns>The formatted error reason.</returns>
        private string FormatStatusCodeFromErrorTemplate(string reason, int statusCode)
        {
            string statusCodePlaceholder = "{$statusCode}";
            return reason.Replace(statusCodePlaceholder, statusCode.ToString());
        }

        /// <summary>
        /// Applies the response header values returned by the api call to the error template.
        /// </summary>
        /// <param name="reason">The error message reason.</param>
        /// <param name="headers">The http headers returend by the api call.</param>
        /// <returns>The formatted error reason.</returns>
        private string FormatResponseHeaderFromErrorTemplate(string reason, Dictionary<string, string> headers)
        {
            string responseHeaderPlaceholderMatcher = "\\{\\$response.header.(.*?)\\}";
            var unqiueMatchValueCollection = Regex.Matches(_reason, responseHeaderPlaceholderMatcher)
                .OfType<Match>()
                .Select(m => m.Value)
                .Distinct();

            foreach (var uniqueMatchValue in unqiueMatchValueCollection)
            {
                reason = reason.Replace(uniqueMatchValue, GetHttpResponseHeaderFromTemplatePlaceholder(uniqueMatchValue, headers));
            }
            return reason;
        }

        /// <summary>
        /// Applies the response body values returned by the api call to the error template.
        /// </summary>
        /// <param name="reason">The error message reason.</param>
        /// <param name="response">The http body returend by the api call.</param>
        /// <returns>The formatted error reason.</returns>
        private string FormatResponseBodyFromErrorTemplate(string reason, Response response)
        {
            string responseHeaderPlaceholderMatcher = "\\{\\$response.body(.*?)\\}";
            var unqiueMatchValueCollection = Regex.Matches(_reason, responseHeaderPlaceholderMatcher)
                .OfType<Match>()
                .Select(m => m.Value)
                .Distinct();

            foreach (var uniqueMatchValue in unqiueMatchValueCollection)
            {
                reason = reason.Replace(uniqueMatchValue, GetHttpResponseBodyFromTemplatePlaceholder(uniqueMatchValue, response.Body));
            }
            return reason;
        }

        /// <summary>
        /// Gets the http response header value from the response header dictionary returned by the api call.
        /// </summary>
        /// <param name="placeholder">The extracted placeholder from the error template.</param>
        /// <param name="headers">The http header dictionary returned by the api call.</param>
        /// <returns>The string value from the header dictionary if exists, otherwise empty string.</returns>
        private string GetHttpResponseHeaderFromTemplatePlaceholder(string placeholder, Dictionary<string, string> headers)
        {
            var headerKey = placeholder.Split('.')[2].TrimEnd('}');
            headers.TryGetValue(headerKey, out var headerValue);
            return headerValue ?? string.Empty;
        }

        /// <summary>
        /// Gets the http response body value from the response body returned by the api call.
        /// </summary>
        /// <param name="placeholder">The extracted placeholder from the error template.</param>
        /// <param name="responseBody">The http string response returned by the api call.</param>
        /// <returns>The string value of the response body or it's fields if response body in not empty, null or whitespace, otherwise empty string.</returns>
        private string GetHttpResponseBodyFromTemplatePlaceholder(string placeholder, string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return string.Empty;
            }

            if (placeholder.Contains('#'))
            {
                return GetTemplatePlaceholderValueFromJPointer(placeholder, responseBody);
            }

            return responseBody;
        }

        /// <summary>
        /// Gets the response body field using the JPointer notation provided in the error template.
        /// </summary>
        /// <param name="placeholder">The extracted placeholder from the error template.</param>
        /// <param name="responseBody">The http string response returned by the api call.</param>
        /// <returns>Returns the response body field if notation and response matches, otherwise empty string.</returns>
        private string GetTemplatePlaceholderValueFromJPointer(string placeholder, string responseBody)
        {
            var pointerKey = placeholder.Split('#')[1].TrimEnd('}');
            if (!string.IsNullOrWhiteSpace(pointerKey))
            {
                try
                {
                    JToken responseBodyToken = JToken.Parse(responseBody);
                    var jPointer = new JsonPointer(pointerKey);
                    return jPointer.Evaluate(responseBodyToken).ToString(Formatting.None);
                }
                catch (Exception)
                {
                    // return empty string if unable to process the responseBody JPointer
                }
            }
            return string.Empty;
        }
    }
}
