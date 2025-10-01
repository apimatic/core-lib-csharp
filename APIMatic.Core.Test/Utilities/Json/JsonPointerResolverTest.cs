using APIMatic.Core.Utilities.Json;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Json
{
    [TestFixture]
    public class JsonPointerResolverTest
    {
        [Test]
        public void ResolveScopedJsonValue_NullPointer_ReturnsNull()
        {
            var result = JsonPointerResolver.ResolveScopedJsonValue(null, "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void ResolveScopedJsonValue_InvalidFormat_ReturnsNull()
        {
            var result = JsonPointerResolver.ResolveScopedJsonValue("$response.body", "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void ResolveScopedJsonValue_NullJsonPointer_ReturnsNull()
        {
            var result = JsonPointerResolver.ResolveScopedJsonValue("$response.body#", "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void ResolveScopedJsonValue_ValidBodyPointer_ReturnsValue()
        {
            var json = @"{""name"":""alice"", ""age"":30}";
            var pointer = "$response.body#/name";

            var result = JsonPointerResolver.ResolveScopedJsonValue(pointer, json, null);

            Assert.AreEqual("alice", result);
        }

        [Test]
        public void ResolveScopedJsonValue_ValidHeadersPointer_ReturnsValue()
        {
            var headers = @"{""content-type"":""application/json""}";
            var pointer = "$response.headers#/content-type";

            var result = JsonPointerResolver.ResolveScopedJsonValue(pointer, null, headers);

            Assert.AreEqual("application/json", result);
        }

        [Test]
        public void ResolveScopedJsonValue_PointerNotFound_ReturnsNull()
        {
            var json = @"{""name"":""alice""}";
            var pointer = "$response.body#/nonexistent";

            var result = JsonPointerResolver.ResolveScopedJsonValue(pointer, json, null);

            Assert.IsNull(result);
        }

        [Test]
        public void ResolveScopedJsonValue_UnsupportedPrefix_ReturnsNull()
        {
            var pointer = "$request.body#/name";
            var json = @"{""name"":""alice""}";

            var result = JsonPointerResolver.ResolveScopedJsonValue(pointer, json, null);

            Assert.IsNull(result);
        }

        [Test]
        public void ResolveScopedJsonValue_BooleanToken_ReturnsTokenToString()
        {
            var jsonBody = @"{ ""isActive"": true }";
            var pointerString = "$response.body#/isActive";

            var result = JsonPointerResolver.ResolveScopedJsonValue(pointerString, jsonBody, null);

            Assert.AreEqual("True", result);
        }
        
        [TestCase("#/name", "{\"name\":\"John\",\"age\":30}", "John")]
        [TestCase("#/invalid", "{\"name\":\"John\"}", null)]
        [TestCase(null, "{\"name\":\"John\"}", null)]
        [TestCase("", "{\"name\":\"John\"}", null)]
        [TestCase("/name", "{\"name\":\"John\"}", null)]
        [TestCase("#/age", "{\"age\":30}", "30")]
        [TestCase("#/name", null, null)]
        public void ResolveJsonValue_VariousCases_ReturnsExpected(string jsonPointer, string json, string expected)
        {
            var result = JsonPointerResolver.ResolveJsonValue(jsonPointer, json);
            Assert.AreEqual(expected, result);
        }
    }
}
