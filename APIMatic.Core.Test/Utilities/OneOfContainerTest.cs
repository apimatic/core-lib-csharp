using APIMatic.Core.Types.Sdk.Exceptions;
using APIMatic.Core.Types;
using NUnit.Framework;
using System;
using APIMatic.Core.Utilities;
using APIMatic.Core.Test.MockTypes.Models.Containers;
using APIMatic.Core.Test.MockTypes.Models;
using System.Collections.Generic;

namespace APIMatic.Core.Test.Utilities
{
    /// <summary>
    /// OneOf Test Controller
    /// </summary>
    [TestFixture]
    public class OneOfContainerTest
    {
        [Test]
        public void TestNativeType()
        {
            // Parameters for the API call
            NativeOneOfContainer formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer>("\"some string\"");

            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestNative());
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestNative());
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer>("0.987");
            formScalar.Match(new TestNative());
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestNative());
        }

        private class TestNative : NativeOneOfContainer.ICases<VoidType>
        {
            public VoidType MString(string mString)
            {
                Assert.AreEqual("some string", mString);
                Console.WriteLine(mString);
                return null;
            }

            public VoidType Precision(double precision)
            {
                Assert.AreEqual(0.987d, precision);
                Console.WriteLine(precision);
                return null;
            }
        }

        [Test]
        public void TestNativeInvalidType()
        {
            OneOfValidationException exception = null;

            try
            {
                var formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer>("12");
                formScalar.Match(new TestNative());
            }
            catch (OneOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from precision, string on: 12", exception.Message);
        }

        [Test]
        public void TestNativeCollectionType()
        {
            // Parameters for the API call
            NativeOneOfCollectionContainer formScalar = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>("[\"some string array\", \"test\"]");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCollection());
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCollection());
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>("[0.987, 0.6987]");
            formScalar.Match(new TestCollection());
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCollection());

            formScalar = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>("{\"key1\" : {\"NumberOfElectrons\":12,\"NumberOfProtons\":13} }");
            formScalar.Match(new TestCollection());
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCollection());
        }

        private class TestCollection : NativeOneOfCollectionContainer.ICases<VoidType>
        {
            public VoidType MString(string[] mString)
            {
                string[] expectedStringArray = { "some string array", "test" };
                CollectionAssert.AreEqual(expectedStringArray, mString);
                Console.WriteLine(string.Join(", ", mString));
                return null;
            }

            public VoidType Precision(double[] precision)
            {
                double[] expectedPrecision = { 0.987, 0.6987 };
                CollectionAssert.AreEqual(expectedPrecision, precision);
                Console.WriteLine(string.Join(", ", precision));
                return null;
            }

            public VoidType CustomTypeDictionary(Dictionary<string, Atom> customDictionary)
            {
                Dictionary<string, Atom> expected = new Dictionary<string, Atom>()
                {
                    { "key1", new Atom(12, 13) }
                };
                CollectionAssert.AreEqual(expected, customDictionary);
                Console.WriteLine(expected.ToString());
                return null;
            }
        }

        [Test]
        public void TestNativeOuterListContainer()
        {
            // Parameters for the API call
            NativeOneOfContainer[] formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer[]>("[\"some string\", 0.987]");
            Assert.IsNotNull(formScalar);
            foreach (var item in formScalar)
            {
                item.Match(new TestNative());
            }
            formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var item in formScalar)
            {
                item.Match(new TestNative());
            }
        }

        [Test]
        public void TestNativeInvalidOuterList()
        {
            OneOfValidationException exception = null;

            try
            {
                var formScalar = CoreHelper.JsonDeserialize<NativeOneOfContainer[]>("[\"some string\", 0.987, 12]");
                foreach (var item in formScalar)
                {
                    item.Match(new TestNative());
                }
            }
            catch (OneOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from precision, string on: 12", exception.Message);
        }

        [Test]
        public void TestCustomType()
        {
            // Parameters for the API call
            CustomOneOfContainer formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCustom());
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustom());
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfShells\":3}");
            formScalar.Match(new TestCustom());
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustom());
        }

        private class TestCustom : CustomOneOfContainer.ICases<VoidType>
        {
            public VoidType Atom(Atom atom)
            {
                Atom expected = new Atom(12, 13);
                Assert.AreEqual(expected, atom);
                Console.WriteLine(atom);
                return null;
            }

            public VoidType Orbit(Orbit orbit)
            {
                Orbit expected = new Orbit(12, 3);
                Assert.AreEqual(expected, orbit);
                Console.WriteLine(orbit);
                return null;
            }
        }

        [Test]
        public void TestCustomInvalidType()
        {
            OneOfValidationException exception = null;

            try
            {
                var formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"NumberOfShells\":12,\"NumberOfProtons\":13}");
                formScalar.Match(new TestCustom());
            }
            catch (OneOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from atom, orbit on: {\n  \"NumberOfShells\": 12,\n  \"NumberOfProtons\": 13\n}", exception.Message.Replace("\r", ""));
        }

        [Test]
        public void TestCustomTwoTypeInvalid()
        {
            OneOfValidationException exception = null;

            try
            {
                var formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13, \"NumberOfElectrons\":12,\"NumberOfShells\":3}");
                formScalar.Match(new TestCustom());
            }
            catch (OneOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            string expectedMessage = "There are more than one matching types i.e. atom and orbit on: {\n  \"NumberOfElectrons\": 12,\n  \"NumberOfProtons\": 13,\n  \"NumberOfShells\": 3\n}";
            Assert.AreEqual(expectedMessage, exception.Message.Replace("\r", ""));
        }

        [Test]
        public void TestCustomTypeCollection()
        {
            // Parameters for the API call
            CustomOneOfCollectionContainer formScalar = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>("[{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}]");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCustomCollection());
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustomCollection());
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>("[{\"NumberOfElectrons\":12,\"NumberOfShells\":3}]");
            formScalar.Match(new TestCustomCollection());
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustomCollection());
        }

        private class TestCustomCollection : CustomOneOfCollectionContainer.ICases<VoidType>
        {
            public VoidType Atom(Atom[] atom)
            {
                Atom expectedAtom = new Atom(12, 13);
                Atom[] expected = { expectedAtom };
                Assert.AreEqual(expected, atom);
                Console.WriteLine(atom);
                return null;
            }

            public VoidType Orbit(Orbit[] orbit)
            {
                Orbit expectedOrbit = new Orbit(12, 3);
                Orbit[] expected = { expectedOrbit };
                Assert.AreEqual(expected, orbit);
                Console.WriteLine(orbit);
                return null;
            }
        }

        [Test]
        public void TestCustomTypeOuter()
        {
            // Parameters for the API call
            CustomOneOfContainer[] formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}, {\"NumberOfElectrons\":12,\"NumberOfShells\":3} ]");
            Assert.IsNotNull(formScalar);
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfShells\":3}, {\"NumberOfElectrons\":12,\"NumberOfProtons\":13}]");
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
        }

        [Test]
        public void TestCustomTypeInner()
        {
            // Parameters for the API call
            CustomOneOfContainer[] formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[ {\"NumberOfElectrons\":12,\"NumberOfProtons\":13} ]");
            Assert.IsNotNull(formScalar);
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[ {\"NumberOfElectrons\":12,\"NumberOfShells\":3} ]");
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
        }
    }
}
