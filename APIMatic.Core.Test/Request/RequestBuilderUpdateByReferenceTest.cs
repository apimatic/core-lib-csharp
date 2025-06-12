using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using APIMatic.Core.Request;
using APIMatic.Core.Test.MockTypes.Models.Pagination;
using APIMatic.Core.Types.Sdk;
using NUnit.Framework;

namespace APIMatic.Core.Test.Request
{
    [TestFixture]
    public class RequestBuilderUpdateByReferenceTest : TestBase
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
            builder.UpdateByReference("$request.headers#", old => 999);

            // Assert
            Assert.AreEqual(previousHeaders, builder.headersParameters);
        }

        [Test]
        public async Task UpdateByReference_HeaderParam_SingleValue_UpdatesWithNonExistentPointer()
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
        public async Task UpdateByReference_BodyParameters_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Body(b => b.Setup("age", 25)));
            await builder.Build();

            builder.UpdateByReference("$request.body#/age", old => 30);

            Assert.IsTrue(builder.bodyParameters.ContainsKey("age"));
            Assert.AreEqual(30, builder.bodyParameters["age"]);
        }

        [Test]
        public async Task UpdateByReference_FormParameters_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Form(f =>
            {
                f.Setup("username", "john");
                f.Setup("email", "john@example.com");
            }));

            await builder.Build();

            builder.UpdateByReference("$request.body#/email", old => "jane@example.com");

            var updated = builder.formParameters.ToDictionary(p => p.Key, p => p.Value);
            Assert.AreEqual("jane@example.com", updated["email"]);
        }

        [Test]
        public async Task UpdateByReference_DynamicBody_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Body( b => b.Setup(new { name = "Alice", age = 30 })));
            await builder.Build();

            builder.UpdateByReference("$request.body#/age", old => 35);

            Assert.AreEqual(35, builder.body.age);
        }

        [Test]
        public async Task UpdateByReference_StringBody_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            builder.Parameters(p => p.Body( b => b.Setup("InitialValue")));
            await builder.Build();

            builder.UpdateByReference("$request.body#/", old => "UpdatedValue");

            Assert.AreEqual("UpdatedValue", builder.body);
        }

        [Test]
        public async Task UpdateByReference_ObjectBody_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);
            
            builder.Parameters(p => p.Body( b => b.Setup(new User { Id = 1, Name = "Alice" })));
            await builder.Build();

            builder.UpdateByReference("$request.body#/Id", old => 35);

            var updatedBody = builder.body as User;
            Assert.IsNotNull(updatedBody);
            Assert.AreEqual(35, updatedBody.Id);
            Assert.AreEqual("Alice", updatedBody.Name); // Ensure other values remain intact
        }
        
        [Test]
        public async Task UpdateByReference_Body_CoreFileStreamInfo_DoesNotUpdate()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value, ServerUrl);
            setupRequest(builder);

            var originalBody = new CoreFileStreamInfo(new MemoryStream(), "text/plain", "file.txt");
            builder.Parameters(p => p.Body( b => b.Setup(originalBody)));

            await builder.Build();

            builder.UpdateByReference("$request.body#/", _ => "should-not-apply");

            Assert.AreSame(originalBody, builder.body); // Still the same object
        }
    }
}
