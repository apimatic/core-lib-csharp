using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Utilities;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities
{
    [TestFixture]
    internal class XmlUtilityTest
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
            List<TestModel> testModels = new List<TestModel>()
            {
                new TestModel()
                {
                    Integers = new int[] { 1, 2, 3 }
                }
            };

            string expected = "<ArrayOfTestModel xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <TestModel>\r\n    <TestDateTime>0001-01-01T00:00:00</TestDateTime>\r\n    <Integers>\r\n      <int>1</int>\r\n      <int>2</int>\r\n      <int>3</int>\r\n    </Integers>\r\n  </TestModel>\r\n</ArrayOfTestModel>";
            string actual = XmlUtility.ModelsArrayToXml(testModels);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ModelsArrayToXml_WithArrayItemName()
        {
            List<TestModel> testModels = new List<TestModel>()
            {
                new TestModel()
                {
                    Integers = new int[] { 1, 2, 3 }
                }
            };

            string expected = "<TestModel xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <TestDateTime>0001-01-01T00:00:00</TestDateTime>\r\n  <Integers>\r\n    <int>1</int>\r\n    <int>2</int>\r\n    <int>3</int>\r\n  </Integers>\r\n</TestModel>";
            string actual = XmlUtility.ModelsArrayToXml(testModels, arrayItemName: "TestModel");
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
            List<TestModel> testModels = new List<TestModel>()
            {
                new TestModel()
                {
                    Integers = new int[] { 1, 2, 3 }
                }
            };

            string expected = "<abd>\r\n  <xyz xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n    <TestDateTime>0001-01-01T00:00:00</TestDateTime>\r\n    <Integers>\r\n      <int>1</int>\r\n      <int>2</int>\r\n      <int>3</int>\r\n    </Integers>\r\n  </xyz>\r\n</abd>";
            string actual = XmlUtility.ModelsArrayToXml(testModels, arrayName: "abd", arrayItemName: "xyz");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NativeTypesArrayFromXml_WithArrayName()
        {
            List<string> expected = new List<string>()
            {
                "alpha", "beta", "gamma"
            };

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
            List<string> testModels = new List<string>()
            {
                "alpha", "beta", "gamma"
            };

            string expected = "<root>\r\n  <String>alpha</String>\r\n  <String>beta</String>\r\n  <String>gamma</String>\r\n</root>";
            string actual = XmlUtility.NativeTypesArrayToXml(testModels, arrayName: "root");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NativeTypesArrayToXml_WithoutArrayName()
        {
            List<string> testModels = new List<string>()
            {
                "alpha", "beta", "gamma"
            };

            string expected = "<String>\r\n  <String>alpha</String>\r\n  <String>beta</String>\r\n  <String>gamma</String>\r\n</String>";
            string actual = XmlUtility.NativeTypesArrayToXml(testModels);
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
            List<string> expected = new List<string>()
            {
                "alpha", "beta", "gamma"
            };

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
            string expected = "<root>\r\n  <entry key=\"key1\">value1</entry>\r\n  <entry key=\"key2\">value2</entry>\r\n</root>";
            Assert.AreEqual(expected, actual);
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
