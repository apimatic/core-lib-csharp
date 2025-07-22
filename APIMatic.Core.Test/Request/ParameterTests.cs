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
        
        [Test]
        public void DuplicateHeaderKeys_Should_UseLastOneInRequestBuilder()
        {
            // Arrange
            var parameters = new Parameter.Builder();

            parameters
                .Header(h => h.Setup("Authorization", "Basic OLD").Required())
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
        public void AdditionalForms_WhenKeyIsNull_ShouldAssignInnerFieldsCorrectly()
        {
            // Arrange
            var parameters = new Parameter.Builder();
            var fieldParameters = new Dictionary<string, object>
            {
                ["inner_field"] =  "inner_field",
            };
            
            parameters
                .AdditionalForms(additionalForms => additionalForms.Setup(fieldParameters));

            var requestBuilder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);

            // Act
            parameters.Validate().Apply(requestBuilder);

            // Assert
            var formParam = requestBuilder.formParameters.FirstOrDefault(p => p.Key == "inner_field");

            Assert.Multiple(() =>
            {
                Assert.That(formParam, Is.Not.Null, "Expected form parameter with key 'inner_field'");
                Assert.That(formParam.Value, Is.EqualTo("inner_field"), "Expected value mismatch for 'inner_field'");
            });
        }
    }
}
