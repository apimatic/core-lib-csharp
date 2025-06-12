using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using APIMatic.Core.Request;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Test.MockTypes.Models.Pagination;
using APIMatic.Core.Utilities.Json;
using Microsoft.Json.Pointer;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities.Json
{
    [TestFixture]
    public class JsonPointerAccessorTest : TestBase
    {
        private const string ServerUrl = "https://api.example.com/users";

        readonly Action<RequestBuilder> setupRequest = rb =>
            rb.Setup(HttpMethod.Get, ServerUrl);

        [Test]
        public async Task UpdateByReference_HeaderParam_SingleValue_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Header(h => h.Setup("id", 0)));
            await builder.Build();

            // Act
            builder.UpdateByReference("$request.headers#/id", old => 999);

            // Assert
            Assert.IsTrue(builder.headersParameters.ContainsKey("id"));
            Assert.AreEqual("999", builder.headersParameters["id"].ToString());
        }
        
        [Test]
        public async Task UpdateByReference_HeaderParam_IncorrectJsonPointer_UpdatesPrevious()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Header(h => h.Setup("id", 0)));
            await builder.Build();

            var previousHeaders = builder.headersParameters;
            
            // Act
            builder.UpdateByReference("$request.headers", old => 999);
            
            // Assert
            Assert.AreEqual(previousHeaders, builder.headersParameters);
        }

        [Test]
        public async Task UpdateByReference_HeaderParam_SingleValue_UpdatesWithEmptyPointer()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Header(h => h.Setup("id", 0)));
            await builder.Build();

            var previousHeaders = builder.headersParameters;

            // Act
            builder.UpdateByReference("$request.headers#/nonexistent", old => 999);

            // Assert
            Assert.AreEqual(previousHeaders, builder.headersParameters);
        }

        [Test]
        public async Task UpdateByReference_HeaderParam_ObjectValue_UpdatesObjectCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            var initialUser = new User { Id = 1, Name = "User1" };
            builder.Parameters(p => p.Header(h => h.Setup("user", initialUser)));
            await builder.Build();

            // Act
            builder.UpdateByReference("$request.headers#/user/Id", old => 456);

            // Assert
            Assert.IsTrue(builder.headersParameters.ContainsKey("user"));
            var updatedUser = builder.headersParameters["user"] as User;
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(456, updatedUser.Id);
            Assert.AreEqual("User1", updatedUser.Name);
        }

        [Test]
        public async Task UpdateByReference_QueryParam_ObjectValue_UpdatesObjectCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            var initialUser = new User { Id = 1, Name = "User1" };
            builder.Parameters(p => p.Query(q => q.Setup("user", initialUser)));
            await builder.Build();

            // Act
            builder.UpdateByReference("$request.query#/user/Id", old => 456);

            // Assert
            Assert.IsTrue(builder.queryParameters.ContainsKey("user"));
            var updatedUser = builder.queryParameters["user"] as User;
            Assert.IsNotNull(updatedUser);
            Assert.AreEqual(456, updatedUser.Id);
            Assert.AreEqual("User1", updatedUser.Name);
        }

        [Test]
        public async Task UpdateByReference_TemplateParam_SingleValue_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Template(t => t.Setup("userId", 1)));
            await builder.Build();

            // Act
            builder.UpdateByReference("$request.path#/userId", old => 456);

            // Assert
            Assert.IsTrue(builder.templateParameters.ContainsKey("userId"));
            Assert.AreEqual(456, builder.templateParameters["userId"]);
        }

        [Test]
        public async Task UpdateByReference_TemplateParam_Collection_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Template(t => t.Setup("ids", new List<int> { 1, 2, 3 })));
            await builder.Build();

            // Act
            builder.UpdateByReference("$request.path#/ids/1", old => 999);

            // Assert
            Assert.IsTrue(builder.templateParameters.ContainsKey("ids"));
            var updated = builder.templateParameters["ids"] as IList<int>;
            Assert.IsNotNull(updated);
            CollectionAssert.AreEqual(new List<int> { 1, 999, 3 }, updated);
        }

        [Test]
        public void UpdateValueByPointer_ValidPointer_UpdatesValue()
        {
            // Arrange
            var person = new Person { Name = "Alice", Age = 30 };
            var pointer = new JsonPointer("/age");

            // Act
            var updated = JsonPointerAccessor.UpdateBodyValueByPointer(
                person,
                pointer,
                old => int.Parse(old.ToString() ?? "") + 5
            );

            // Assert
            Assert.AreEqual(35, updated.Age);
            Assert.AreEqual("Alice", updated.Name);
        }

        [Test]
        public void UpdateValueByPointer_NullValue_ReturnsOriginal()
        {
            // Act
            var result = JsonPointerAccessor.UpdateBodyValueByPointer<Person>(
                null,
                new JsonPointer("/name"),
                old => "Bob"
            );

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateValueByPointer_NullPointer_ReturnsOriginal()
        {
            var person = new Person { Name = "Charlie", Age = 40 };
            var result = JsonPointerAccessor.UpdateBodyValueByPointer(
                person,
                null,
                old => "David"
            );

            Assert.AreEqual("Charlie", result.Name);
        }

        [Test]
        public void UpdateValueByPointer_NullUpdater_ReturnsOriginal()
        {
            var person = new Person { Name = "Charlie", Age = 40 };
            var result = JsonPointerAccessor.UpdateBodyValueByPointer(
                person,
                new JsonPointer("/name"),
                null
            );

            Assert.AreEqual("Charlie", result.Name);
        }

        [Test]
        public void UpdateValueByPointer_UpdaterReturnsNull_ReturnsOriginal()
        {
            var person = new Person { Name = "Eva", Age = 50 };
            var pointer = new JsonPointer("/name");

            var result = JsonPointerAccessor.UpdateBodyValueByPointer(
                person,
                pointer,
                old => null
            );

            Assert.AreEqual("Eva", result.Name);
        }

        [Test]
        public void UpdateValueByPointer_InvalidPointer_ReturnsOriginal()
        {
            var person = new Person { Name = "Frank", Age = 60 };
            var invalidPointer = new JsonPointer("/nonexistent");

            var result = JsonPointerAccessor.UpdateBodyValueByPointer(
                person,
                invalidPointer,
                old => "ShouldNotApply"
            );

            Assert.AreEqual("Frank", result.Name);
        }

        [Test]
        public void ResolveJsonValueByReference_NullPointer_ReturnsNull()
        {
            var result = JsonPointerAccessor.ResolveJsonValueByReference(null, "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void ResolveJsonValueByReference_InvalidFormat_ReturnsNull()
        {
            var result = JsonPointerAccessor.ResolveJsonValueByReference("$response.body", "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void ResolveJsonValueByReference_NullJsonPointer_ReturnsNull()
        {
            var result = JsonPointerAccessor.ResolveJsonValueByReference("$response.body#", "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void ResolveJsonValueByReference_ValidBodyPointer_ReturnsValue()
        {
            var json = @"{""name"":""alice"", ""age"":30}";
            var pointer = "$response.body#/name";

            var result = JsonPointerAccessor.ResolveJsonValueByReference(pointer, json, null);

            Assert.AreEqual("alice", result);
        }

        [Test]
        public void ResolveJsonValueByReference_ValidHeadersPointer_ReturnsValue()
        {
            var headers = @"{""content-type"":""application/json""}";
            var pointer = "$response.headers#/content-type";

            var result = JsonPointerAccessor.ResolveJsonValueByReference(pointer, null, headers);

            Assert.AreEqual("application/json", result);
        }

        [Test]
        public void ResolveJsonValueByReference_PointerNotFound_ReturnsNull()
        {
            var json = @"{""name"":""alice""}";
            var pointer = "$response.body#/nonexistent";

            var result = JsonPointerAccessor.ResolveJsonValueByReference(pointer, json, null);

            Assert.IsNull(result);
        }

        [Test]
        public void ResolveJsonValueByReference_UnsupportedPrefix_ReturnsNull()
        {
            var pointer = "$request.body#/name";
            var json = @"{""name"":""alice""}";

            var result = JsonPointerAccessor.ResolveJsonValueByReference(pointer, json, null);

            Assert.IsNull(result);
        }

        [Test]
        public void ResolveJsonValueByReference_BooleanToken_ReturnsTokenToString()
        {
            var jsonBody = @"{ ""isActive"": true }";
            var pointerString = "$response.body#/isActive";

            var result = JsonPointerAccessor.ResolveJsonValueByReference(pointerString, jsonBody, null);

            Assert.AreEqual("True", result);
        }
    }
}
