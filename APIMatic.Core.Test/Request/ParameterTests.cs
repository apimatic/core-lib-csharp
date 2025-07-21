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
    }
}
