using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Test.Utilities.Date;
using APIMatic.Core.Utilities;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities
{
    [TestFixture]
    internal sealed class XmlUtilityTest
    {
        [Test]
        public void ToXml_NullObject()
        {
            object dataObject = null;
            string expected = string.Empty;
            string actual = XmlUtility.ToXml(dataObject);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModelsArrayToXml_WithoutArrayName()
        {
            List<TestModel> testModels =
            [
                new TestModel() { Integers = [1, 2, 3] }
            ];

            string expected = "<ArrayOfTestModel xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n  <TestModel>\n    <TestDateTime>0001-01-01T00:00:00</TestDateTime>\n    <Integers>\n      <int>1</int>\n      <int>2</int>\n      <int>3</int>\n    </Integers>\n  </TestModel>\n</ArrayOfTestModel>";
            string actual = StringReplacer.ReplaceBackSlashR(XmlUtility.ModelsArrayToXml(testModels));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModelsArrayToXml_WithArrayItemName()
        {
            List<TestModel> testModels =
            [
                new TestModel() { Integers = [1, 2, 3] }
            ];

            string expected = "<TestModel xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n  <TestDateTime>0001-01-01T00:00:00</TestDateTime>\n  <Integers>\n    <int>1</int>\n    <int>2</int>\n    <int>3</int>\n  </Integers>\n</TestModel>";
            string actual = StringReplacer.ReplaceBackSlashR(XmlUtility.ModelsArrayToXml(testModels, arrayItemName: "TestModel"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModelsArrayToXml_NullList()
        {
            List<TestModel> testModels = null;
            string expected = string.Empty;
            string actual = XmlUtility.ModelsArrayToXml(testModels);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModelsArrayToXml_WithArrayName()
        {
            List<TestModel> testModels =
            [
                new TestModel() { Integers = [1, 2, 3] }
            ];

            string expected = "<abd>\n  <xyz xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n    <TestDateTime>0001-01-01T00:00:00</TestDateTime>\n    <Integers>\n      <int>1</int>\n      <int>2</int>\n      <int>3</int>\n    </Integers>\n  </xyz>\n</abd>";
            string actual = StringReplacer.ReplaceBackSlashR(XmlUtility.ModelsArrayToXml(testModels, arrayName: "abd", arrayItemName: "xyz"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NativeTypesArrayFromXml_WithArrayName()
        {
            List<string> expected = ["alpha", "beta", "gamma"];

            string nativeObjectString = "<root>\r\n  <String>alpha</String>\r\n  <String>beta</String>\r\n  <String>gamma</String>\r\n</root>";
            List<string> actual = XmlUtility.NativeTypesArrayFromXml<string>(nativeObjectString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NativeTypesArrayFromXml_NullString()
        {
            string nativeObjectString = string.Empty;
            List<string> actual = XmlUtility.NativeTypesArrayFromXml<string>(nativeObjectString);
            Assert.IsNull(actual);
        }

        [Test]
        public void NativeTypesArrayToXml_WithArrayName()
        {
            List<string> testModels = ["alpha", "beta", "gamma"];

            string expected = "<root>\n  <String>alpha</String>\n  <String>beta</String>\n  <String>gamma</String>\n</root>";
            string actual = XmlUtility.NativeTypesArrayToXml(testModels, arrayName: "root");
            Assert.AreEqual(expected, StringReplacer.ReplaceBackSlashR(actual));
        }

        [Test]
        public void NativeTypesArrayToXml_WithoutArrayName()
        {
            List<string> testModels = ["alpha", "beta", "gamma"];

            string expected = "<String>\n  <String>alpha</String>\n  <String>beta</String>\n  <String>gamma</String>\n</String>";
            string actual = StringReplacer.ReplaceBackSlashR(XmlUtility.NativeTypesArrayToXml(testModels));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NativeTypesArrayToXml_NullObject()
        {
            List<string> testModels = null;
            string expected = string.Empty;
            string actual = XmlUtility.NativeTypesArrayToXml(testModels, arrayName: "root");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FromXml_EmptyString()
        {
            string expected = string.Empty;
            string actual = XmlUtility.FromXml<string>(expected);
            Assert.IsNull(actual);
        }

        [Test]
        public void FromXml_ValidInput()
        {
            List<string> expected = ["alpha", "beta", "gamma"];

            string local = XmlUtility.ToXml(expected, rootName: "root");
            var actual = XmlUtility.FromXml<List<string>>(local, rootName: "root");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DictionaryToXml_ValidInput()
        {
            Dictionary<string, object> testDictionary = new Dictionary<string, object>()
            {
                {"key1", "value1"}, {"key2", "value2"}
            };

            string actual = XmlUtility.DictionaryToXml(testDictionary, rootName: "root");
            string expected = "<root>\n  <entry key=\"key1\">value1</entry>\n  <entry key=\"key2\">value2</entry>\n</root>";
            Assert.AreEqual(expected, StringReplacer.ReplaceBackSlashR(actual));
        }

        [Test]
        public void DictionaryToXml_NullInput()
        {
            Dictionary<string, object> testDictionary = null;

            string actual = XmlUtility.DictionaryToXml(testDictionary, rootName: "root");
            Assert.IsNull(actual);
        }

        [Test]
        public void XmlToDictionary_ValidInput()
        {
            Dictionary<string, object> expected = new Dictionary<string, object>()
            {
                {"key1", "value1"}, {"key2", "value2"}
            };

            string xmlString = "<root>\r\n  <entry key=\"key1\">value1</entry>\r\n  <entry key=\"key2\">value2</entry>\r\n</root>";
            Dictionary<string, object> actual = XmlUtility.XmlToDictionary<object>(xmlString);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void XmlToDictionary_NullString()
        {
            string xmlString = string.Empty;
            Dictionary<string, object> actual = XmlUtility.XmlToDictionary<object>(xmlString);
            Assert.IsNull(actual);
        }
    }
}
