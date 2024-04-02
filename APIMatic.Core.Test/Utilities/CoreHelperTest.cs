using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using APIMatic.Core.Http.Configuration;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Test.MockTypes.Utilities;
using APIMatic.Core.Utilities;
using APIMatic.Core.Utilities.Converters;
using APIMatic.Core.Utilities.Date;
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
        public void JsonDeserialize_EnumNumber()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumber>("3");
            Assert.AreEqual(MonthNumber.March, actual);

            var actualNullable = CoreHelper.JsonDeserialize<MonthNumber?>("3");
            Assert.AreEqual(MonthNumber.March, actualNullable);

            var actualAdditional = CoreHelper.JsonDeserialize<MonthNumberAllowAdditionalValues>("3");
            Assert.AreEqual(MonthNumberAllowAdditionalValues.March, actualAdditional);
        }

        [Test]
        public void JsonDeserialize_EnumNumberNullable()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumber?>("null");
            Assert.AreEqual(null, actual);
        }

        [Test]
        public void JsonDeserialize_EnumNumberAllowUnknownEnumValues()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumberAllowAdditionalValues>("\"-1\"");
            Assert.AreEqual(MonthNumberAllowAdditionalValues._Unknown, actual);
        }

        [Test]
        public void JsonDeserialize_EnumNumberAllowUnknownEnumValuesNullable()
        {
            var actual = CoreHelper.JsonDeserialize<MonthNumberAllowAdditionalValues?>("null");
            Assert.AreEqual(null, actual);
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
    }
}
