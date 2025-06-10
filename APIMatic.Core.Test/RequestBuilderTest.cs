using System;
using System.Net.Http;
using System.Threading.Tasks;
using APIMatic.Core.Request;
using NUnit.Framework;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class RequestBuilderTest : TestBase
    {
        readonly Action<RequestBuilder> setupRequest = rb =>
            rb.Setup(HttpMethod.Get, "https://api.example.com/users");
        
        [Test]
        public async Task UpdateByReference_HeaderParam_UpdatesCorrectly()
        {
            var builder = new RequestBuilder(LazyGlobalConfiguration.Value);
            setupRequest(builder);
            builder.Parameters(p => p.Header(h => h.Setup("id", 0)));
            await builder.Build();
            
            builder.UpdateByReference("$request.headers#/id", old => "999");
            
            Assert.IsTrue(builder.headers.ContainsKey("id"));
            Assert.IsTrue(builder.headers.ContainsValue("999"));
        }
    }
}
