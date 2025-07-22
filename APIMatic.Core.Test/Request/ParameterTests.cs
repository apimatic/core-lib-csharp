using System.Collections.Generic;
using System.Linq;
using APIMatic.Core.Request;
using APIMatic.Core.Request.Parameters;
using NUnit.Framework;

namespace APIMatic.Core.Test.Request
{
    [TestFixture]
    public class ParameterTests : TestBase
    {
        const string ServerUrl = "https://parameter.api.server.com";

        private static readonly Dictionary<string, object> QueryParams = new Dictionary<string, object>
        {
            ["field"] = "query_value",
            ["field1"] = "query_value1",
            ["field2"] = "query_value2",
            ["field3"] = "query_value3",
        };

        private static readonly Dictionary<string, object> AdditionalQueryParams = new Dictionary<string, object>
        {
            ["field"] = "additional_query_value",
            ["field1"] = "additional_query_value1",
            ["field2"] = "additional_query_value2",
            ["field3"] = "additional_query_value3",
        };

        [Test]
        public void AdditionalForms_WhenKeyIsNull_ShouldAssignInnerFieldsCorrectly()
        {
            // Arrange
            var parameters = new Parameter.Builder();
            var fieldParameters = new Dictionary<string, object> { ["field"] = "field", };

            parameters
                .AdditionalForms(additionalForms => additionalForms.Setup(fieldParameters));

            var requestBuilder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);

            // Act
            parameters.Validate().Apply(requestBuilder);

            // Assert
            var formParam = requestBuilder.formParameters.FirstOrDefault(p => p.Key == "field");

            Assert.Multiple(() =>
            {
                Assert.That(formParam, Is.Not.Null, "Expected form parameter with key 'field'");
                Assert.That(formParam.Value, Is.EqualTo("field"), "Expected value mismatch for 'field'");
            });
        }

        [Test]
        public void DuplicateHeaderKeys_Should_UseLastOneInRequestBuilder()
        {
            // Arrange
            var parameters = new Parameter.Builder();

            parameters
                .Header(h => h.Setup("Authorization", "Basic OLD1").Required())
                .Header(h => h.Setup("Authorization", "Basic OLD2").Required())
                .Header(h => h.Setup("Authorization", "Basic OLD3").Required())
                .Header(h => h.Setup("Authorization", "Basic OLD4").Required())
                .Header(h => h.Setup("Authorization", "Basic OLD5").Required())
                .Header(h => h.Setup("Authorization", "Basic OLD6").Required())
                .Header(h => h.Setup("Authorization", "Basic NEW").Required());

            var requestBuilder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);

            // Act
            parameters.Validate().Apply(requestBuilder);

            // Assert
            Assert.That(requestBuilder.headersParameters.TryGetValue("Authorization", out var value), Is.True);
            Assert.That(value, Is.EqualTo("Basic NEW"));
        }

        [Test]
        public void DuplicateQueryKeys_Should_UseLastOneInRequestBuilder()
        {
            // Arrange
            var parameters = new Parameter.Builder();

            parameters
                .Query(q => q.Setup("status", "pending").Required())
                .Query(q => q.Setup("status", "approved").Required());

            var requestBuilder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);

            // Act
            parameters.Validate().Apply(requestBuilder);

            // Assert
            Assert.That(requestBuilder.queryParameters.TryGetValue("status", out var value), Is.True);
            Assert.That(value, Is.EqualTo("approved"));
        }

        [Test]
        public void DuplicateFormKey_WhenSetInAdditionalFormsAndForm_ShouldRetainBothWithSameKey()
        {
            // Arrange
            var parameters = new Parameter.Builder();
            var fieldParameters = new Dictionary<string, object> { ["field"] = "additional_form_value", };

            parameters
                .AdditionalForms(additionalForms => additionalForms.Setup(fieldParameters))
                .Form(f => f.Setup("field", "form_value"));

            var requestBuilder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);

            // Act
            parameters.Validate().Apply(requestBuilder);

            // Assert
            var formParams = requestBuilder.formParameters
                .Where(p => p.Key == "field")
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(formParams.Count, Is.EqualTo(2), "Expected 2 form parameters with the key 'field'");
                Assert.That(formParams.Any(p => Equals(p.Value, "form_value")), Is.True,
                    "Expected 'form_value' to be present");
                Assert.That(formParams.Any(p => Equals(p.Value, "additional_form_value")), Is.True,
                    "Expected 'additional_form_value' to be present");
            });
        }

        [Test]
        public void When_QueryAddedAfterAdditionalQueries_ShouldRetainQueryValues()
        {
            // Arrange
            var parameters = new Parameter.Builder();
            parameters.AdditionalQueries(q => q.Setup(AdditionalQueryParams));
            foreach (var (key, value) in QueryParams)
                parameters.Query(q => q.Setup(key, value));

            var requestBuilder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);

            // Act
            parameters.Validate().Apply(requestBuilder);

            // Assert
            Assert.Multiple(() =>
            {
                foreach (var (key, expectedValue) in QueryParams)
                {
                    Assert.That(requestBuilder.queryParameters.TryGetValue(key, out var actualValue), Is.True,
                        $"Expected query parameter with key '{key}'");
                    Assert.That(actualValue, Is.EqualTo(expectedValue),
                        $"Expected value for key '{key}' to be '{expectedValue}', but was '{actualValue}'");
                }
            });
        }

        [Test]
        public void When_AdditionalQueriesAddedAfterQuery_ShouldRetainAdditionalQueryValues()
        {
            // Arrange
            var parameters = new Parameter.Builder();
            foreach (var (key, value) in QueryParams)
                parameters.Query(q => q.Setup(key, value));
            parameters.AdditionalQueries(q => q.Setup(AdditionalQueryParams));

            var requestBuilder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);

            // Act
            parameters.Validate().Apply(requestBuilder);

            // Assert
            Assert.Multiple(() =>
            {
                foreach (var (key, expectedValue) in AdditionalQueryParams)
                {
                    Assert.That(requestBuilder.queryParameters.TryGetValue(key, out var actualValue), Is.True,
                        $"Expected query parameter with key '{key}'");
                    Assert.That(actualValue, Is.EqualTo(expectedValue),
                        $"Expected value for key '{key}' to be '{expectedValue}', but was '{actualValue}'");
                }
            });
        }
    }
}
