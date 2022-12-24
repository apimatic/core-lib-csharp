using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using APIMatic.Core.Request.Parameters;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk;
using NUnit.Framework;

namespace APIMatic.Core.Test
{
    [TestFixture]
    public class ParametersTest : TestBase
    {
        [Test]
        public void Parameters_Apply_WithoutValidation()
        {
            var requestBuilder = LazyGlobalConfiguration.Value.GlobalRequestBuilder().Setup(HttpMethod.Get, "/path/{template}");

            new Parameter.Builder()
                .Query(p => p.Setup("query", "some query value"))
                .Form(p => p.Setup("form", "some form value"))
                .Header(p => p.Setup("header", "some header"))
                .Template(p => p.Setup("template", "someTemplate"))
                .Apply(requestBuilder);
            Assert.False(requestBuilder.queryParameters.ContainsKey("query"));
            Assert.False(requestBuilder.formParameters.Any(kv => kv.Key == "form"));
            Assert.False(requestBuilder.headers.ContainsKey("header"));
            Assert.True(requestBuilder.QueryUrl.ToString().Contains("{template}"));


            new Parameter.Builder()
                .Body(p => p.Setup("some body"))
                .AdditionalForms(p => p.Setup("form", "some form value"))
                .Apply(requestBuilder);
            Assert.True(requestBuilder.body == null);
            Assert.False(requestBuilder.formParameters.Any(kv => kv.Key == "form"));
        }

        [Test]
        public void TemplateParameter_Null()
        {
            var requestBuilder = LazyGlobalConfiguration.Value.GlobalRequestBuilder().Setup(HttpMethod.Get, "/path/{template}");

            new Parameter.Builder().Template(p => p.Setup("template", null)).Validate().Apply(requestBuilder);

            Assert.AreEqual("http://my/path:3000/v1/path/", requestBuilder.QueryUrl.ToString());
        }

        [Test]
        public void TemplateParameter_Collection()
        {
            var requestBuilder = LazyGlobalConfiguration.Value.GlobalRequestBuilder().Setup(HttpMethod.Get, "/path/{template}");

            new Parameter.Builder().Template(p => p.Setup("template", new List<string>
            {
                "asad",
                "alpha",
                "gamma"
            })).Validate().Apply(requestBuilder);

            Assert.AreEqual("http://my/path:3000/v1/path/asad/alpha/gamma", requestBuilder.QueryUrl.ToString());
        }

        [Test]
        public void TemplateParameter_DateTime()
        {
            var requestBuilder = LazyGlobalConfiguration.Value.GlobalRequestBuilder().Setup(HttpMethod.Get, "/path/{template}");

            new Parameter.Builder().Template(p => p.Setup("template", new DateTime(2022, 12, 14))).Validate().Apply(requestBuilder);

            Assert.AreEqual("http://my/path:3000/v1/path/2022-12-14T00:00:00", requestBuilder.QueryUrl.ToString());
        }

        [Test]
        public void TemplateParameter_DateTimeOffset()
        {
            var requestBuilder = LazyGlobalConfiguration.Value.GlobalRequestBuilder().Setup(HttpMethod.Get, "/path/{template}");

            new Parameter.Builder().Template(p => p.Setup("template", new DateTimeOffset(2022, 12, 14, 2, 1, 4, TimeSpan.Zero)))
                .Validate()
                .Apply(requestBuilder);

            Assert.AreEqual("http://my/path:3000/v1/path/2022-12-14T02:01:04+00:00", requestBuilder.QueryUrl.ToString());
        }

        [Test]
        public void FormParameter_Multipart()
        {
            var requestBuilder = LazyGlobalConfiguration.Value.GlobalRequestBuilder().Setup(HttpMethod.Get, "/path");
            var memStream = new MemoryStream(Encoding.UTF8.GetBytes("Test memory stream data."));
            var file = new CoreFileStreamInfo(memStream, "test - stream.file");
            var model = new Dictionary<string, object>
            {
                { "name", "This is a field" }
            };
            new Parameter.Builder()
                .Form(p => p.EncodingHeader("Content-Type", "image/png").Setup("file", file))
                .Form(p => p.EncodingHeader("Content-Type", "application/json").Setup("model", model))
                .Validate()
                .Apply(requestBuilder);
            var multipartFile = requestBuilder.formParameters.Find(kv => kv.Key == "file").Value;
            var multipartByteArray = requestBuilder.formParameters.Find(kv => kv.Key == "model").Value;
            Assert.IsInstanceOf(typeof(MultipartFileContent), multipartFile);

            Assert.IsInstanceOf(typeof(MultipartByteArrayContent), multipartByteArray);

            var byteArrayMultipart = multipartByteArray as MultipartByteArrayContent;
            byteArrayMultipart.Rewind();
            var content = byteArrayMultipart.ToHttpContent("model");
            Assert.AreEqual("{\"name\":\"This is a field\"}", Encoding.UTF8.GetString(content.ReadAsByteArrayAsync().Result));
        }
    }
}
