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

        [Test]
        public void TestNativeNullableType()
        {
            // Parameters for the API call
            NativeNullableOneOfContainer formScalar = CoreHelper.JsonDeserialize<NativeNullableOneOfContainer>("null");

            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestNativeNullable());
            formScalar = CoreHelper.JsonDeserialize<NativeNullableOneOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestNativeNullable());
        }

        private class TestNative : NativeOneOfContainer.ICases<VoidType>
        {
            public VoidType MString(string value)
            {
                Assert.AreEqual("some string", value);
                Console.WriteLine(value);
                return null;
            }

            public VoidType Precision(double value)
            {
                Assert.AreEqual(0.987d, value);
                Console.WriteLine(value);
                return null;
            }
        }

        private class TestNativeNullable : NativeNullableOneOfContainer.ICases<VoidType>
        {
            public VoidType MString(string? value)
            {
                Assert.AreEqual(null, value);
                Console.WriteLine(value);
                return null;
            }

            public VoidType Precision(double value)
            {
                Assert.AreEqual(0.987d, value);
                Console.WriteLine(value);
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
            Assert.AreEqual("We could not match any acceptable type from double, string on: 12", exception.Message);
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
            public VoidType MString(string[] value)
            {
                string[] expectedStringArray = { "some string array", "test" };
                CollectionAssert.AreEqual(expectedStringArray, value);
                Console.WriteLine(string.Join(", ", value));
                return null;
            }

            public VoidType Precision(double[] value)
            {
                double[] expectedPrecision = { 0.987, 0.6987 };
                CollectionAssert.AreEqual(expectedPrecision, value);
                Console.WriteLine(string.Join(", ", value));
                return null;
            }

            public VoidType CustomTypeDictionary(Dictionary<string, Atom> value)
            {
                Dictionary<string, Atom> expected = new Dictionary<string, Atom>()
                {
                    { "key1", new Atom(12, 13) }
                };
                CollectionAssert.AreEqual(expected, value);
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
            Assert.AreEqual("We could not match any acceptable type from double, string on: 12", exception.Message);
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
            public VoidType Atom(Atom value)
            {
                Atom expected = new Atom(12, 13);
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }

            public VoidType Orbit(Orbit value)
            {
                Orbit expected = new Orbit(12, 3);
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }
        }

        private class TestCustomWithDisc : CustomOneOfWithDiscContainer.ICases<VoidType>
        {
            public VoidType Atom(Atom value)
            {
                Atom expected = new Atom(12, 13);
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }

            public VoidType Helium(Helium value)
            {
                Helium expected = new Helium(3, 3);
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }
        }

        [Test]
        public void TestCustomWithDiscriminatorType()
        {
            // Parameters for the API call
            CustomOneOfWithDiscContainer formScalar = CoreHelper.JsonDeserialize<CustomOneOfWithDiscContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCustomWithDisc());
            formScalar = CoreHelper.JsonDeserialize<CustomOneOfWithDiscContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustomWithDisc());
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
            public VoidType Atom(Atom[] value)
            {
                Atom expectedAtom = new Atom(12, 13);
                Atom[] expected = { expectedAtom };
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }

            public VoidType Orbit(Orbit[] value)
            {
                Orbit expectedOrbit = new Orbit(12, 3);
                Orbit[] expected = { expectedOrbit };
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
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
