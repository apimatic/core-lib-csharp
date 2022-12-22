using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIMatic.Core.Utilities;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities
{
    [TestFixture]
    internal class TestHelperTest
    {
        internal const string OBJECT_STRING = "[{\"name\":\"Shahid Khaliq\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"boss\":{\"personType\":\"Boss\",\"name\":\"Zeeshan Ejaz\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\",\"promotedAt\":1484719381},\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\"},{\"name\":\"Shahid Khaliq\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"boss\":{\"personType\":\"Boss\",\"name\":\"Zeeshan Ejaz\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\",\"promotedAt\":1484719381},\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\"}]";

        internal const string RIGHT_OBJECT = "{\"personType\":\"Empl\",\"name\":\"Shahid Khaliq\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"boss\":{\"personType\":\"Boss\",\"name\":\"Zeeshan Ejaz\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\",\"promotedAt\":1484719381},\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\"}";

        [Test]
        public void IsArrayOfJsonObjectsProperSubsetOf_SameLeftRightObject()
        {
            string leftObject = OBJECT_STRING;

            string rightObject = OBJECT_STRING;

            Assert.IsTrue(TestHelper.IsArrayOfJsonObjectsProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsArrayOfJsonObjectsProperSubsetOf_SameLeftRightObjectIsOrdered()
        {
            string leftObject = OBJECT_STRING;

            string rightObject = OBJECT_STRING;

            Assert.IsTrue(TestHelper.IsArrayOfJsonObjectsProperSubsetOf(leftObject, rightObject, checkValues: true, allowExtra: true, isOrdered: true));
        }

        [Test]
        public void IsArrayOfJsonObjectsProperSubsetOf_DifferentLeftRightObjectIsOrdered()
        {
            string leftObject = OBJECT_STRING;

            string rightObject = "[{\"person\" : \"Employee\"}]";

            Assert.IsFalse(TestHelper.IsArrayOfJsonObjectsProperSubsetOf(leftObject, rightObject, checkValues: true, allowExtra: false, isOrdered: true));
        }

        [Test]
        public void IsArrayOfJsonObjectsProperSubsetOf_DIfferentLeftRightObject()
        {
            string leftObject = OBJECT_STRING;

            string rightObject = "[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}";

            Assert.IsFalse(TestHelper.IsArrayOfJsonObjectsProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void AreHeadersProperSubsetOf_Equals()
        {
            Dictionary<string, string> leftDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/responseType" },
                { "Accept", "application/noTerm" },
                { "Accept-Encoding", "UTF-8" }
            };

            Dictionary<string, string> rightDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/responseType" },
                { "Connection", "keep-alive" },
                { "Keep-Alive", "timeout=5" },
                { "Accept", "application/noTerm" },
                { "Accept-Encoding", "UTF-8" },
                { "X-Powered-By", "Express"},
                { "Content-Length", "0"},
            };

            Assert.IsTrue(TestHelper.AreHeadersProperSubsetOf(leftDict: leftDictionary, rightDict: rightDictionary));
        }


        [Test]
        public void AreHeadersProperSubsetOf_NullValueInLeftOne()
        {
            Dictionary<string, string> leftDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/responseType" },
                { "Accept", "application/noTerm" },
                { "test", null },
                { "Accept-Encoding", "UTF-8" },
            };

            Dictionary<string, string> rightDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/responseType" },
                { "Connection", "keep-alive" },
                { "Keep-Alive", "timeout=5" },
                { "Accept", "application/noTerm" },
                { "Accept-Encoding", "UTF-8" },
                { "X-Powered-By", "Express"},
                { "Content-Length", "0"},
                { "test", "something"},
            };

            Assert.IsTrue(TestHelper.AreHeadersProperSubsetOf(leftDict: leftDictionary, rightDict: rightDictionary));
        }


        [Test]
        public void AreHeadersProperSubsetOf_RightKeyDoesNotContain()
        {
            Dictionary<string, string> leftDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/responseType" },
                { "Accept", "application/noTerm" },
                { "Accept-Encoding", "UTF-8" },
                { "test", "UTF-8" },
            };

            Dictionary<string, string> rightDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/responseType" },
                { "Connection", "keep-alive" },
                { "Keep-Alive", "timeout=5" },
                { "Accept", "application/noTerm" },
                { "Accept-Encoding", "UTF-8" },
                { "X-Powered-By", "Express"},
                { "Content-Length", "0"},
            };

            Assert.IsFalse(TestHelper.AreHeadersProperSubsetOf(leftDict: leftDictionary, rightDict: rightDictionary));
        }

        [Test]
        public void AreHeadersProperSubsetOf_RightValueNotEquals()
        {
            Dictionary<string, string> leftDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/responseType" },
                { "Accept", "application/noTerm" },
                { "Accept-Encoding", "UTF-8" },
                { "test", "UTF-8" },
            };

            Dictionary<string, string> rightDictionary = new Dictionary<string, string>()
            {
                { "Content_Type", "application/json" },
                { "Connection", "keep-alive" },
                { "Keep-Alive", "timeout=5" },
                { "Accept", "application/noTerm" },
                { "Accept-Encoding", "UTF-8" },
                { "X-Powered-By", "Express"},
                { "Content-Length", "0"},
            };

            Assert.IsFalse(TestHelper.AreHeadersProperSubsetOf(leftDict: leftDictionary, rightDict: rightDictionary));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_ExtraValueInLeft()
        {
            string leftObject = "{\"passed\":true}";
            string rightObject = "{\"passed\":true,\"message\":\"OK\",\"input\":{\"path\":\"/body/additionalModelProperties\",\"query\":{},\"headers\":{\"host\":\"localhost:3000\",\"accept\":\"application/json\",\"content-type\":\"application/json\",\"content-length\":\"100\"},\"method\":\"POST\",\"body\":{\"name\":\"farhan\",\"field\":\"QA\",\"address\":\"Ghori Town\",\"Job\":{\"company\":\"APIMATIC\",\"location\":\"NUST\"}},\"uploadCount\":0}}";

            Assert.IsTrue(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }


        [Test]
        public void IsJsonObjectProperSubsetOf_Equals()
        {
            string leftObject = "{\"name\": {\"Role\" : \"Developer\"},\"age\": 5147483649,\"address\": [\"H # 531, S #20\"],   \"uid\": \"123412\"}";
            string rightObject = "{\"name\": {\"Role\" : \"Developer2\"},\"age\": 5147483649,\"address\": [\"H # 531, S #20\"],   \"uid\": \"123412\"}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_LeftContainJObjectOnly()
        {
            string leftObject = "{\"name\": {\"Role\" : \"Developer\"}}";
            string rightObject = "{\"name\": \"hamza\"}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_LeftJArray()
        {
            string leftObject = "{\"address\": [\"H # 531, S #20\"]}";
            string rightObject = "{\"address\": \"H # 531 S #20\"}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }


        [Test]
        public void IsJsonObjectProperSubsetOf_BothArray()
        {
            string leftObject = "{\"address\": [{\"street\" : \"H # 531, S #20\"}]}";
            string rightObject = "{\"address\": [{\"street\" : \"H # 531, S #20\"}]}";

            Assert.IsTrue(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_BothArrayButDifferentValue()
        {
            string leftObject = "{\"address\": [{\"street\" : \"H # 531\"}]}";
            string rightObject = "{\"address\": [{\"street\" : \"H # 531, S #20\"}]}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_LeftJArrayContainsJObject()
        {
            string leftObject = "{\"address\": [{\"street\" : \"H # 531, S #20\"}]}";
            string rightObject = "{\"address\": [\"H # 531 S #20\"]}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_JObjectAndPrimitiveValues()
        {
            string leftObject = "{\"address\": [{\"street\" : \"H # 531, S #20\"}, \"25\"]}";
            string rightObject = "{\"address\": [{\"street\" : \"H # 531, S #20\"}, \"98\"]}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_JArray()
        {
            string leftObject = "{\"address\": [\"Test\", \"25\"]}";
            string rightObject = "{\"address\": [\"Test\", \"98\"]}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }


        [Test]
        public void IsJsonObjectProperSubsetOf_JObjectDifferent()
        {
            string leftObject = "{\"address\": [{\"street\" : \"H # 531, S #20\"}, \"25\"]}";
            string rightObject = "{\"address\": [{\"street\" : \"H # 531,\"}, \"98\"]}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_LeftValueNull()
        {
            string leftObject = "{\"address\": null}";
            string rightObject = "{\"address\": [{\"street\" : \"H # 531,\"}, \"98\"]}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

         [Test]
        public void IsJsonObjectProperSubsetOf_LeftRightNull()
        {
            string leftObject = "{\"address\": null}";
            string rightObject = "{\"address\": null}";

            Assert.IsTrue(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_LeftNullRightJObject()
        {
            string leftObject = "{\"address\": null}";
            string rightObject = "{\"address\": {\"street\" : 123}}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_RightObjectMissingProperty()
        {
            string leftObject = "{\"passed\":true}";
            string rightObject = "{\"message\":\"OK\",\"input\":{\"path\":\"/body/additionalModelProperties\",\"query\":{},\"headers\":{\"host\":\"localhost:3000\",\"accept\":\"application/json\",\"content-type\":\"application/json\",\"content-length\":\"100\"},\"method\":\"POST\",\"body\":{\"name\":\"farhan\",\"field\":\"QA\",\"address\":\"Ghori Town\",\"Job\":{\"company\":\"APIMATIC\",\"location\":\"NUST\"}},\"uploadCount\":0}}";

            Assert.IsFalse(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsJsonObjectProperSubsetOf_ListInRight()
        {
            string leftObject = "{\"name\":\"Shahid Khaliq\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"boss\":{\"personType\":\"Boss\",\"name\":\"Zeeshan Ejaz\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\",\"promotedAt\":1484719381},\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\"}";

            string rightObject = "{\"personType\":\"Empl\",\"name\":\"Shahid Khaliq\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"boss\":{\"personType\":\"Boss\",\"name\":\"Zeeshan Ejaz\",\"age\":5147483645,\"address\":\"H # 531, S # 20\",\"uid\":\"123321\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\",\"salary\":20000,\"department\":\"Software Development\",\"joiningDay\":\"Saturday\",\"workingDays\":[\"Monday\",\"Tuesday\",\"Friday\"],\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\",\"promotedAt\":1484719381},\"dependents\":[{\"name\":\"Future Wife\",\"age\":5147483649,\"address\":\"H # 531, S # 20\",\"uid\":\"123412\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"},{\"name\":\"Future Kid\",\"age\":5147483648,\"address\":\"H # 531, S # 20\",\"uid\":\"312341\",\"birthday\":\"1994-02-13\",\"birthtime\":\"1994-02-13T14:01:54.9571247Z\"}],\"hiredAt\":\"Sun, 06 Nov 1994 08:49:37 GMT\"}";

            Assert.IsTrue(TestHelper.IsJsonObjectProperSubsetOf(leftObject: leftObject, rightObject: rightObject, checkValues: true, allowExtra: true, isOrdered: false));
        }

        [Test]
        public void IsSameAsFile_GistURL()
        {
            string gistUrl = "https://gist.githubusercontent.com/asadali214/0a64efec5353d351818475f928c50767/raw/8ad3533799ecb4e01a753aaf04d248e6702d4947/testFile.txt";
            byte[] buffer = Encoding.ASCII.GetBytes("This test file is created to test CoreFileWrapper functionality");
            Stream memoryStream = new MemoryStream(buffer);
            bool expected = true;
            bool actual = TestHelper.IsSameAsFile(gistUrl, memoryStream);
            memoryStream.Dispose();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsSameInputStream_Equals()
        {
            byte[] buffer = Encoding.ASCII.GetBytes("This test file is created to test CoreFileWrapper functionality");
            Stream memoryStream = new MemoryStream(buffer);
            Stream memoryStream1 = new MemoryStream(buffer);
            bool expected = true;
            bool actual = TestHelper.IsSameInputStream(memoryStream, memoryStream1);
            memoryStream.Dispose();
            memoryStream1.Dispose();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsSameInputStream_ExactEquals()
        {
            byte[] buffer = Encoding.ASCII.GetBytes("This test file is created to test CoreFileWrapper functionality");
            Stream memoryStream = new MemoryStream(buffer);
            bool expected = true;
            bool actual = TestHelper.IsSameInputStream(memoryStream, memoryStream);
            memoryStream.Dispose();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsSameInputStream_UnEquals()
        {
            byte[] buffer = Encoding.ASCII.GetBytes("This test file is created to test CoreFileWrapper functionality");
            byte[] buffer1 = Encoding.ASCII.GetBytes("This test file is created to test FileWrapper functionality");
            Stream memoryStream = new MemoryStream(buffer);
            Stream secondmemoryStream = new MemoryStream(buffer1);
            bool expected = false;
            bool actual = TestHelper.IsSameInputStream(memoryStream, secondmemoryStream);
            memoryStream.Dispose();
            secondmemoryStream.Dispose();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ConvertStreamToString()
        {
            string expectedString = "This string is to test the functionality of ConvertStreamToString method";
            byte[] buffer = ASCIIEncoding.GetEncoding("UTF-8").GetBytes(expectedString);
            Stream memoryStream = new MemoryStream(buffer);
            string actualString = TestHelper.ConvertStreamToString(memoryStream);
            Assert.AreEqual(expectedString, actualString);
        }

        [Test]
        public void IsSuperSetOf()
        {
            Assert.IsTrue(TestHelper.IsSuperSetOf(GetEnumerable("TestCase", "TryItYourSelf"), GetEnumerable("TestCase")));
        }

        [Test]
        public void IsOrderedSubsetOf_SameSizeOfEnumerable()
        {
            Assert.IsTrue(TestHelper.IsOrderedSupersetOf(GetEnumerable("TestCase", "TryItYourSelf"), GetEnumerable("TestCase", "TryItYourSelf"), true));
        }

        [Test]
        public void IsOrderedSubsetOf_DifferentSizeOfEnumerable()
        {
            Assert.False(TestHelper.IsOrderedSupersetOf(GetEnumerable("TestCase", "TryItYourSelf"), GetEnumerable("TestCase"), true));
        }

        [Test]
        public void IsOrderedSubsetOf_DifferentSizeOfEnumerable_WithoutCheckSize()
        {
            Assert.False(TestHelper.IsOrderedSupersetOf(GetEnumerable("TryItYourSelf", "TestCase"), GetEnumerable("TestCase")));
        }


        [Test]
        public void IsOrderedSubsetOf_DifferentSizeOfEnumerable_WithCheckSize()
        {
            Assert.False(TestHelper.IsOrderedSupersetOf(GetEnumerable("TestCase"), GetEnumerable("TestCase", "TryItYourSelf"), true));
        }

        [Test]
        public void IsListProperSubsetOf_EqualList()
        {
            string leftList = "[\"alpha\", \"beta\", \"gamma\"]";
            string rightList = "[\"alpha\", \"beta\", \"gamma\"]";

            bool actual = TestHelper.IsListProperSubsetOf(CoreHelper.JsonDeserialize<dynamic>(leftList), CoreHelper.JsonDeserialize<dynamic>(rightList), false, true);
            bool expected = true;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsListProperSubsetOf_UnEqualLeftList()
        {
            string leftList = "[\"alpha\", \"beta\"]";
            string rightList = "[\"alpha\", \"beta\", \"gamma\"]";

            bool actual = TestHelper.IsListProperSubsetOf(leftList, rightList, false, true);
            bool expected = false;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void IsListProperSubsetOf_UnEqualRightList()
        {
            string leftList = "[\"alpha\", \"beta\", \"gamma\"]";
            string rightList = "[\"alpha\", \"beta\"]";

            bool actual = TestHelper.IsListProperSubsetOf(leftList, rightList, true, true);
            bool expected = false;
            Assert.AreEqual(expected, actual);
        }

        private IEnumerable<object> GetEnumerable(params object[] parameters)
        {
            foreach (object parameter in parameters)
            {
                yield return parameter;
            }
        }


    }
}
