using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using APIMatic.Core.Request;
using APIMatic.Core.Request.Parameters;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using NUnit.Framework;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class TypesTest : TestBase
    {
        [Test]
        public void MultipartContent_AdditionalHeaders()
        {
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data."));
            var file = new CoreFileStreamInfo(memStream, "test - stream.file");
            Dictionary<string, IReadOnlyCollection<string>> multipartHeaders = new(StringComparer.OrdinalIgnoreCase)
            {
                { "myHeader", new[] { "personalHeaderValue" }}
            };
            var fileContent = new MultipartFileContent(file, multipartHeaders);
            var content = fileContent.ToHttpContent("file");
            Assert.AreEqual("Test memory stream data.", Encoding.UTF8.GetString(content.ReadAsByteArrayAsync().Result));
            Assert.AreEqual("form-data; name=file; filename=\"test - stream.file\"", content.Headers.ContentDisposition.ToString());
            Assert.AreEqual("application/octet-stream", content.Headers.ContentType.ToString());
            Assert.AreEqual("personalHeaderValue", content.Headers.GetValues("myHeader").First());
        }
    }
}
