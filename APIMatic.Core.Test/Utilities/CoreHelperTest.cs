using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Test.MockTypes.Utilities;
using APIMatic.Core.Types;
using APIMatic.Core.Utilities;
using APIMatic.Core.Utilities.Converters;
using APIMatic.Core.Utilities.Date;
using Microsoft.Json.Pointer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities
{
    [TestFixture]
    public class CoreHelperTest : TestBase
    {
        internal const string SERVER_URL = "http://my/path:3000/v1";
        #region CleanUrl

        [Test]
        public void CleanUrl_ValidUrl()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(SERVER_URL);
            string expected = stringBuilder.ToString();
            string actual = CoreHelper.CleanUrl(stringBuilder);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CleanUrl_RemoveExtraStuffFromUrl()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("https://localhost:3000//api/body");
            string expected = "https://localhost:3000/api/body";
            string actual = CoreHelper.CleanUrl(stringBuilder);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CleanUrl_RemoveExtraSlashFromUrl()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("https://localhost:3000//api/body?x=4");
            string expected = "https://localhost:3000/api/body?x=4";
            string actual = CoreHelper.CleanUrl(stringBuilder);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CleanUrl_InvalidUrl()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("://localhost:3000//api/body");
            Assert.Throws<ArgumentException>(() => CoreHelper.CleanUrl(stringBuilder));
        }
        #endregion

        #region Serialize
        [Test]
        public void JsonSerialize_ServerResponse()
        {
            ServerResponse serverResponse = new ServerResponse()
            {
                Message = "Pass",
                Passed = true,
                Input = "Test is pass"
            };
            string expected = "{\"passed\":true,\"Message\":\"Pass\",\"input\":\"Test is pass\",\"AdditionalProperties\":null}";
            string actual = CoreHelper.JsonSerialize(serverResponse);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JsonSerialize_CustomAttributeClass()
        {
            TestModelC testModelC = new TestModelC()
            {
                Name = "Test",
            };
            string expected = "{\"$id\":\"1\",\"Name\":\"Test\"}";
            string actual = CoreHelper.JsonSerialize(testModelC);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JsonSerialize_Null_Objects()
        {
            object obj = null;
            string expected = null;
            string actual = CoreHelper.JsonSerialize(obj);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JsonSerialize_DateTime_RFCFormat()
        {
            List<DateTime> list = new List<DateTime>
            {
                DateTime.Parse("2018-4-12")
            };
            string expected = "[\"Thu, 12 Apr 2018 00:00:00 GMT\"]";
            string actual = CoreHelper.JsonSerialize(list, new CoreCustomDateTimeConverter("r"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JsonSerialize_JsonString()
        {
            string str = "a";
            string expected = "\"a\"";
            var actual = CoreHelper.JsonSerialize(str, new JsonStringConverter());
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void JsonSerialize_JsonString_NotAString()
        {
            int number = 24;
            string expected = "24";
            var actual = CoreHelper.JsonSerialize(number, new JsonStringConverter());
            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public void JsonSerialize_EnumStringAllowUnknownEnumValues()
        {
            Assert.Throws<JsonSerializationException>(() => CoreHelper.JsonSerialize(WorkingDaysAllowAdditionalValues._Unknown));
        }

        [Test]
        public void JsonSerialize_EnumNumberAllowUnknownEnumValues()
        {
            Assert.Throws<JsonSerializationException>(() => CoreHelper.JsonSerialize(MonthNumberAllowAdditionalValues._Unknown));
        }

        [Test]
        public void JsonSerialize_EnumNumber()
        {
            var testEnum = MonthNumber.January;
            Assert.That(CoreHelper.JsonSerialize(testEnum), Is.EqualTo("1"));

            var testEnumRelaxed = MonthNumberAllowAdditionalValues.January;
            Assert.That(CoreHelper.JsonSerialize(testEnumRelaxed), Is.EqualTo("1"));
        }

        #endregion

        #region Deserialize

        [Test]
        public void JsonDeserialize_ServerResponse()
        {
            string serverResponseJson = "{\"passed\":true,\"Message\":\"Pass\",\"input\":\"Test is pass\",\"AdditionalProperties\":null}";
            ServerResponse expected = new ServerResponse()
            {
                Message = "Pass",
                Passed = true,
                Input = "Test is pass"
            };

            ServerResponse actual = CoreHelper.JsonDeserialize<ServerResponse>(serverResponseJson);
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void JsonDeserialize_DateTime()
        {
            DateTime dateTime = new DateTime(2022, 12, 12);
            List<DateTime?> expected = new List<DateTime?>
            {
                dateTime
            };
            string dateTimeString = CoreHelper.JsonSerialize(expected, new CoreCustomDateTimeConverter("r"));

            List<DateTime> actual = CoreHelper.JsonDeserialize<List<DateTime>>(dateTimeString, new CoreCustomDateTimeConverter("r"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JsonDeserialize_EmptyString()
        {
            string serverResponseJson = null;

            string expected = default;
            ServerResponse actual = CoreHelper.JsonDeserialize<ServerResponse>(serverResponseJson);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void JsonDeserialize_EnumString()
        {
            var actualNullable = CoreHelper.JsonDeserialize<WorkingDays?>("\"Monday\"");
            Assert.AreEqual(WorkingDays.Monday, actualNullable);

            var actual = CoreHelper.JsonDeserialize<WorkingDays>("\"Monday\"");
            Assert.AreEqual(WorkingDays.Monday, actual);
        }

        [Test]
        public void JsonDeserialize_EnumStringNullable()
        {
            var actual = CoreHelper.JsonDeserialize<WorkingDays?>("null");
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void JsonDeserialize_EnumStringAllowUnknownEnumValues()
        {
            var actual = CoreHelper.JsonDeserialize<WorkingDaysAllowAdditionalValues>("\"InvalidString\"");
            Assert.AreEqual(WorkingDaysAllowAdditionalValues._Unknown, actual);
        }

        [Test]
        public void JsonDeserialize_EnumStringAllowUnknownEnumValuesWithNormalValue()
        {
            var actualAdditional = CoreHelper.JsonDeserialize<WorkingDaysAllowAdditionalValues>("\"Mon\"");
            Assert.AreEqual(WorkingDaysAllowAdditionalValues.Monday, actualAdditional);
        }

        [Test]
        public void JsonDeserialize_EnumNumber()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumber>("3");
            Assert.AreEqual(MonthNumber.March, actual);
        }

        [Test]
        public void JsonDeserialize_EnumNumberNullableWithNullValue()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumber?>("null");
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void JsonDeserialize_EnumNumberNullableWithNormalValue()
        {
            var actualNullable = CoreHelper.JsonDeserialize<MonthNumber?>("3");
            Assert.AreEqual(MonthNumber.March, actualNullable);
        }

        [Test]
        public void JsonDeserialize_EnumNumberAllowUnknownEnumValues()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumberAllowAdditionalValues>("\"-1\"");
            Assert.AreEqual(MonthNumberAllowAdditionalValues._Unknown, actual);
        }

        [Test]
        public void JsonDeserialize_EnumNumberAllowUnknownEnumValuesNullableWithNullValue()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumberAllowAdditionalValues?>("null");
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void JsonDeserialize_EnumNumberAllowUnknownEnumValuesNullableWithNormalValue()
        {
            var actualAdditional = CoreHelper.JsonDeserialize<MonthNumberAllowAdditionalValues?>("3");
            Assert.AreEqual(MonthNumberAllowAdditionalValues.March, actualAdditional);
        }

        #endregion

        #region AppendQueryParameters

        [Test]
        public void AppendQueryParameter_ValidParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var parametersKeys = new List<string>()
            {
                "service", "api"
            };

            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, "messagingService", "testApi"));

            string expected = $"{SERVER_URL}?service=messagingService&api=testApi";
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_EmptyParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);


            string expected = $"{SERVER_URL}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters());
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_DateTimeParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime=2022-12-14T00:00:00";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, new DateTime(2022, 12, 14)));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_DateTimeOffsetParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            var dateTimeOffset = new DateTimeOffset(new DateTime(2022, 12, 14));
            string dateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
            string convertedDateTime = dateTimeOffset.ToString(dateTimeFormat);
            string expected = $"{SERVER_URL}?dateTime={convertedDateTime}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, dateTimeOffset));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_NullParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            object obj = null;
            string expected = $"{SERVER_URL}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, obj));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_NullListParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            List<string> parametersKeys = new List<string>()
            {
                "list"
            };

            List<string> obj = null;
            string expected = $"{SERVER_URL}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, obj));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_EmptyListParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            List<string> parametersKeys = new List<string>()
            {
                "list"
            };

            List<string> obj = new List<string>();
            string expected = $"{SERVER_URL}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, obj));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_NullDictionaryParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            List<string> parametersKeys = new List<string>()
            {
                "dictionary"
            };

            Dictionary<string, string> dictionary = null;
            string expected = $"{SERVER_URL}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, dictionary));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_EmptyDictionaryParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            List<string> parametersKeys = new List<string>()
            {
                "dictionary"
            };

            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string expected = $"{SERVER_URL}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, dictionary));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_UnIndexedCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<string>()
            {
                "test", "collection"
            };

            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime[]=test&dateTime[]=collection";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void AppendQueryParameter_IndexedCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<string>()
            {
                "test", "collection"
            };

            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime[0]=test&dateTime[1]=collection";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection), ArraySerialization.Indexed);
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_PlainCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<string>()
            {
                "test", "collection"
            };

            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime=test&dateTime=collection";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection), ArraySerialization.Plain);
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_TabSeparatedCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<string>()
            {
                "test", "collection"
            };

            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime=test\tcollection";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection), ArraySerialization.TSV);
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CommaSeparatedCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<string>()
            {
                "test", "collection"
            };

            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime=test,collection";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection), ArraySerialization.CSV);
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CustomParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var serverResponse = new ServerResponse(true);

            var parametersKeys = new List<string>()
            {
                "serverResponse"
            };

            string expected = $"{SERVER_URL}?serverResponse%5Bpassed%5D=true";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, serverResponse));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void AppendQueryParameter_IndexedCustomDictionaryParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var dictionaryData = new Dictionary<string, string>()
            {
                {"key1", "value1" },
                {"key2", "value2" }
            };

            var parametersKeys = new List<string>()
            {
                "dictionary"
            };

            string expected = $"{SERVER_URL}?dictionary[0]=%5Bkey1%2C%20value1%5D&dictionary[1]=%5Bkey2%2C%20value2%5D";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, dictionaryData), ArraySerialization.Indexed);
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CustomDateTimeParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            DateTime dateTime = new DateTime(2022, 12, 15);
            TestModel testModel = new TestModel()
            {
                TestDateTime = dateTime
            };
            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime%5BTestDateTime%5D=2022-12-15T00%3A00%3A00";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, testModel));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CustomDateTimeConvertorParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            DateTime dateTime = new DateTime(2022, 12, 15);
            TestModelB testModel = new TestModelB()
            {
                TestDateTime = dateTime
            };
            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime%5BTestDateTime%5D=2022-12-15";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, testModel));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void AppendQueryParameter_CustomStreamParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            byte[] buffer = Encoding.ASCII.GetBytes("test");
            Stream memoryStream = new MemoryStream(buffer);
            TestModel testModel = new TestModel()
            {
                DateStream = memoryStream
            };
            var parametersKeys = new List<string>()
            {
                "stream"
            };

            string expected = $"{SERVER_URL}?stream%5BDateStream%5D=System.IO.MemoryStream&stream%5BTestDateTime%5D=0001-01-01T00%3A00%3A00";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, testModel));
            memoryStream.Dispose();
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CustomJObjectParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);

            dynamic jObject = new JObject();
            jObject.Album = "alpha";
            jObject.Artist = "beta";

            var parametersKeys = new List<string>()
            {
                "jObject"
            };

            string expected = $"{SERVER_URL}?jObject%5BAlbum%5D=alpha&jObject%5BArtist%5D=beta";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, jObject));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CustomEnumParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);

            var parametersKeys = new List<string>()
            {
                "enum"
            };

            string expected = $"{SERVER_URL}?enum=5";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, ArraySerialization.PSV));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_PipeSeparatedCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<string>()
            {
                "test", "collection"
            };

            var parametersKeys = new List<string>()
            {
                "dateTime"
            };

            string expected = $"{SERVER_URL}?dateTime=test|collection";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection), ArraySerialization.PSV);
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CustomCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<ServerResponse>()
            {
                new ServerResponse(true)
            };

            var parametersKeys = new List<string>()
            {
                "serverResponse"
            };

            string expected = $"{SERVER_URL}?serverResponse%5B0%5D%5Bpassed%5D=true";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_CustomNullCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            IList stringCollection = null;
            var parametersKeys = new List<string>()
            {
                "serverResponse"
            };

            string expected = $"{SERVER_URL}";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_WithAlreadyAppendedParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL + "?x=9");
            IList stringCollection = null;
            var parametersKeys = new List<string>()
            {
                "serverResponse"
            };

            string expected = $"{SERVER_URL}?x=9";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AppendQueryParameter_IntegerArrayParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);

            int[] integers = new int[5];
            integers[0] = 9;
            integers[1] = 4;
            integers[2] = 5;
            integers[3] = 6;
            integers[4] = 7;
            TestModel testModel = new TestModel
            {
                Integers = integers
            };
            var parametersKeys = new List<string>()
            {
                "req"
            };

            string expected = $"{SERVER_URL}?req%5BIntegers%5D[]=9&req%5BIntegers%5D[]=4&req%5BIntegers%5D[]=5&req%5BIntegers%5D[]=6&req%5BIntegers%5D[]=7&req%5BTestDateTime%5D=0001-01-01T00%3A00%3A00";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, testModel));
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }


        [Test]
        public void AppendQueryParameter_CustomCollectionOfCollectionParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var stringCollection = new List<List<ServerResponse>>()
            {
               new List<ServerResponse>(){ new ServerResponse(true) }
            };

            var parametersKeys = new List<string>()
            {
                "serverResponse"
            };

            string expected = $"{SERVER_URL}?serverResponse=System.Collections.Generic.List%601%5BAPIMatic.Core.Test.MockTypes.Models.ServerResponse%5D";
            CoreHelper.AppendUrlWithQueryParameters(queryBuilder, GetParameters(parametersKeys, stringCollection), ArraySerialization.Plain);
            string actual = queryBuilder.ToString();
            Assert.AreEqual(expected, actual);
        }

        private IEnumerable<KeyValuePair<string, object>> GetParameters(List<string> keys = null, params object[] parameters)
        {
            int index = 0;
            foreach (object parameter in parameters)
            {
                yield return new KeyValuePair<string, object>(keys[index], parameter);
                index++;
            }
        }

        #endregion
        
        #region ExtractQueryParameters
        [Test]
        public void ExtractQueryParametersForUrl_WithValidUrl_ReturnsCorrectDictionary()
        {
            // Arrange
            var url = "https://api.example.com/items?limit=10&page=2&search=hello%20world";

            // Act
            var result = CoreHelper.ExtractQueryParametersForUrl(url);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("10", result["limit"]);
            Assert.AreEqual("2", result["page"]);
            Assert.AreEqual("hello world", result["search"]);
        }

        [Test]
        public void ExtractQueryParametersForUrl_WithNoQuery_ReturnsEmptyDictionary()
        {
            // Arrange
            var url = "https://api.example.com/items";

            // Act
            var result = CoreHelper.ExtractQueryParametersForUrl(url);

            // Assert
            Assert.IsEmpty(result);
        }

        [Test]
        public void ExtractQueryParametersForUrl_WithEmptyValue_ReturnsEmptyString()
        {
            // Arrange
            var url = "https://api.example.com/items?foo=";

            // Act
            var result = CoreHelper.ExtractQueryParametersForUrl(url);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(string.Empty, result["foo"]);
        }

        [Test]
        public void ExtractQueryParametersForUrl_WithMissingValuePart_IgnoresIt()
        {
            // Arrange
            var url = "https://api.example.com/items?onlykey";

            // Act
            var result = CoreHelper.ExtractQueryParametersForUrl(url);

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(string.Empty, result["onlykey"]);
        }
        #endregion

        #region JasonUpdatealueByPointer
        [Test]
        public void UpdateValueByPointer_ValidPointer_UpdatesValue()
        {
            // Arrange
            var person = new Person { Name = "Alice", Age = 30 };
            var pointer = new JsonPointer("/age");

            // Act
            var updated = CoreHelper.UpdateValueByPointer(
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
            var result = CoreHelper.UpdateValueByPointer<Person>(
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
            var result = CoreHelper.UpdateValueByPointer(
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
            var result = CoreHelper.UpdateValueByPointer(
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

            var result = CoreHelper.UpdateValueByPointer(
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

            var result = CoreHelper.UpdateValueByPointer(
                person,
                invalidPointer,
                old => "ShouldNotApply"
            );

            Assert.AreEqual("Frank", result.Name);
        }
        
        [Test]
        public void GetValueByReference_NullPointer_ReturnsNull()
        {
            var result = CoreHelper.GetValueByReference(null, "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void GetValueByReference_InvalidFormat_ReturnsNull()
        {
            var result = CoreHelper.GetValueByReference("$response.body", "{}", "{}");
            Assert.IsNull(result);
        }

        [Test]
        public void GetValueByReference_ValidBodyPointer_ReturnsValue()
        {
            var json = @"{""name"":""alice"", ""age"":30}";
            var pointer = "$response.body#/name";

            var result = CoreHelper.GetValueByReference(pointer, json, null);

            Assert.AreEqual("alice", result);
        }

        [Test]
        public void GetValueByReference_ValidHeadersPointer_ReturnsValue()
        {
            var headers = @"{""content-type"":""application/json""}";
            var pointer = "$response.headers#/content-type";

            var result = CoreHelper.GetValueByReference(pointer, null, headers);

            Assert.AreEqual("application/json", result);
        }

        [Test]
        public void GetValueByReference_PointerNotFound_ReturnsNull()
        {
            var json = @"{""name"":""alice""}";
            var pointer = "$response.body#/nonexistent";

            var result = CoreHelper.GetValueByReference(pointer, json, null);

            Assert.IsNull(result);
        }

        [Test]
        public void GetValueByReference_UnsupportedPrefix_ReturnsNull()
        {
            var pointer = "$request.body#/name";
            var json = @"{""name"":""alice""}";

            var result = CoreHelper.GetValueByReference(pointer, json, null);

            Assert.IsNull(result);
        }
        
        [Test]
        public void GetValueByReference_BooleanToken_ReturnsTokenToString()
        {
            var jsonBody = @"{ ""isActive"": true }";
            var pointerString = "$response.body#/isActive";

            var result = CoreHelper.GetValueByReference(pointerString, jsonBody, null);

            Assert.AreEqual("True", result);
        }
        #endregion
        
        #region PrepareFormFieldsFromObject

        [Test]
        public void PrepareFormFieldsFromObject_IndexesCustomDictionaryParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var dictionaryData = new Dictionary<string, string>()
            {
                {"key1", "value1" },
                {"key2", "value2" }
            };


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("dictionary[key1]", "value1"),
                new KeyValuePair<string, object>("dictionary[key2]", "value2")
            };
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("dictionary", dictionaryData, ArraySerialization.Indexed);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_PlainCustomListParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var arraySerializations = new List<string>()
            {
                "alpha", "beta", "gamma"
            };


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("arraySerialization", "alpha"),
                new KeyValuePair<string, object>("arraySerialization", "beta"),
                new KeyValuePair<string, object>("arraySerialization", "gamma")
            };
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("arraySerialization", arraySerializations, ArraySerialization.Plain);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_UnIndexedCustomListParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var arraySerializations = new List<string>()
            {
                "alpha", "beta", "gamma"
            };


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("arraySerialization[]", "alpha"),
                new KeyValuePair<string, object>("arraySerialization[]", "beta"),
                new KeyValuePair<string, object>("arraySerialization[]", "gamma")
            };
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("arraySerialization", arraySerializations, ArraySerialization.UnIndexed);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_UnIndexedCustomListNullParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var arraySerializations = new List<string>()
            {
                "alpha", "beta", "gamma", null
            };


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("arraySerialization[]", "alpha"),
                new KeyValuePair<string, object>("arraySerialization[]", "beta"),
                new KeyValuePair<string, object>("arraySerialization[]", "gamma")
            };
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("arraySerialization", arraySerializations, ArraySerialization.UnIndexed);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_PlainCustomNestedListParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var arraySerializations = new List<List<ArraySerialization>>()
            {
                 new List<ArraySerialization>()
                 {
                     ArraySerialization.Indexed, ArraySerialization.PSV
                 }
            };


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("arraySerialization[0][0]", "0"),
                new KeyValuePair<string, object>("arraySerialization[0][1]", "5")
            };
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("arraySerialization", arraySerializations, ArraySerialization.Plain);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_IndexedCoreJsonObjectParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            JsonObject jsonObject = JsonObject.FromJsonString("{\"message\" : \"TestCase\"}");


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("jsonObject[message]", "TestCase")
            };
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("jsonObject", jsonObject, ArraySerialization.Indexed);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_IndexedCoreJsonObjectNullValuesParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            JsonObject jsonObject = JsonObject.FromJsonString("{\"message\" : {\"prop\" : null}}");


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>();
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("jsonObject", jsonObject, ArraySerialization.Indexed);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_IndexedCoreJsonValueParameter()
        {
            StringBuilder queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            JsonValue jsonValue = JsonValue.FromString("TestCase");


            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new KeyValuePair<string, object>("jsonValue", "TestCase")
            };
            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject("jsonValue", jsonValue, ArraySerialization.Indexed);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_WithAdditionalPropertiesAsField()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var simpleModelWithAdditionalPropertiesField =
                new SimpleModelWithAdditionalPropertiesField("Required Field")
                {
                    ["additionalPropertyKey"] = "additionalPropertyValue"
                };

            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new("simpleModelWithAdditionalPropertiesField[requiredProperty]", "Required Field"),
                new("simpleModelWithAdditionalPropertiesField[additionalPropertyKey]", "additionalPropertyValue")
            };

            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject(
                "simpleModelWithAdditionalPropertiesField", simpleModelWithAdditionalPropertiesField,
                ArraySerialization.Indexed);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PrepareFormFieldsFromObject_WithAdditionalPropertiesBaseModel()
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(SERVER_URL);
            var simpleModelWithAdditionalPropertiesBaseModel =
                new SimpleModelWithAdditionalPropertiesBaseModel("Required Field")
                {
                    AdditionalProperties =
                        new Dictionary<string, object> { { "additionalPropertyKey", "additionalPropertyValue" } }
                };

            List<KeyValuePair<string, object>> expected = new List<KeyValuePair<string, object>>()
            {
                new("simpleModelWithAdditionalPropertiesBaseModel[requiredProperty]", "Required Field"),
                new("simpleModelWithAdditionalPropertiesBaseModel[additionalPropertyKey]", "additionalPropertyValue")
            };

            List<KeyValuePair<string, object>> actual = CoreHelper.PrepareFormFieldsFromObject(
                "simpleModelWithAdditionalPropertiesBaseModel", simpleModelWithAdditionalPropertiesBaseModel,
                ArraySerialization.Indexed);

            // Assert that all items in 'expected' are found within 'actual'
            bool containsAllExpectedEntries = expected.All(item => actual.Contains(item));
            Assert.IsTrue(containsAllExpectedEntries, "Not all expected entries are present in actual.");

        }

        #endregion

        #region DeepCloneObject
        [Test]
        public void DeepCloneObject_ServerResponse()
        {
            ServerResponse expected = new ServerResponse(true);
            ServerResponse actual = CoreHelper.DeepCloneObject(expected);
            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region Converters

        [Test]
        public void UnknownEnumConverter_CannotConvertEnumWithoutUnknownValue()
        {
            var converter = new UnknownEnumConverter<StringEnumConverter>("_Unknown");
            Assert.That(converter.CanConvert(typeof(WorkingDays)), Is.False);
        }

        [Test]
        public void UnknownEnumConverter_CanConvertEnumWithUnknownValue()
        {
            var converter = new UnknownEnumConverter<StringEnumConverter>("_Unknown");
            Assert.That(converter.CanConvert(typeof(WorkingDaysAllowAdditionalValues)), Is.True);
        }

        #endregion

        #region Others

        [Test]
        public void IsNullableType_AllTypes()
        {
            Assert.That(CoreHelper.IsNullableType(typeof(WorkingDays)), Is.False);
            Assert.That(CoreHelper.IsNullableType(typeof(WorkingDays?)), Is.True);

            Assert.That(CoreHelper.IsNullableType(typeof(DateTime)), Is.False);
            Assert.That(CoreHelper.IsNullableType(typeof(DateTime?)), Is.True);

            Assert.That(CoreHelper.IsNullableType(typeof(void)), Is.False);
            Assert.That(CoreHelper.IsNullableType(typeof(VoidType)), Is.True);

            Assert.That(CoreHelper.IsNullableType(typeof(int)), Is.False);
            Assert.That(CoreHelper.IsNullableType(typeof(int?)), Is.True);
            Assert.That(CoreHelper.IsNullableType(typeof(Nullable<int>)), Is.True);

            Assert.That(CoreHelper.IsNullableType(typeof(double)), Is.False);
            Assert.That(CoreHelper.IsNullableType(typeof(double?)), Is.True);
            Assert.That(CoreHelper.IsNullableType(typeof(Nullable<double>)), Is.True);

            Assert.That(CoreHelper.IsNullableType(typeof(bool)), Is.False);
            Assert.That(CoreHelper.IsNullableType(typeof(bool?)), Is.True);
            Assert.That(CoreHelper.IsNullableType(typeof(Nullable<bool>)), Is.True);

            Assert.That(CoreHelper.IsNullableType(typeof(string)), Is.True);
            Assert.That(CoreHelper.IsNullableType(typeof(ServerResponse)), Is.True);
            Assert.That(CoreHelper.IsNullableType(typeof(List<object>)), Is.True);
            Assert.That(CoreHelper.IsNullableType(typeof(Dictionary<string, object>)), Is.True);
        }
        #endregion
    }
}
