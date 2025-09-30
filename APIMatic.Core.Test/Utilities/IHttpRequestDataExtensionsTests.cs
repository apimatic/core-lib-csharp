using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Test.MockTypes.Http.Request;
using APIMatic.Core.Utilities;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities
{
    [TestFixture]
    public class IHttpRequestDataExtensionsTests
    {
        
        [TestCase(null, new byte[0])]
        [TestCase("", new byte[0])]
        [TestCase("hello", new byte[] { 104, 101, 108, 108, 111 })]
        public async Task ReadBodyStreamToByteArrayAsync_VariousBodies_ReturnsExpected(string body, byte[] expected)
        {
            var stream = body == null ? null : new MemoryStream(Encoding.UTF8.GetBytes(body));
            var request = new HttpRequestData(new Dictionary<string, string[]>(), stream);

            var result = await request.ReadBodyStreamToByteArrayAsync();

            Assert.AreEqual(expected, result);
        }
    }
}
