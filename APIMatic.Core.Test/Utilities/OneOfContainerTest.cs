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
            NativeOneOfContainer container = CoreHelper.JsonDeserialize<NativeOneOfContainer>("\"some string\"");

            Assert.IsNotNull(container);
            container.Match(new TestNative());
            container = CoreHelper.JsonDeserialize<NativeOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestNative());
            container = CoreHelper.JsonDeserialize<NativeOneOfContainer>("0.987");
            container.Match(new TestNative());
            container = CoreHelper.JsonDeserialize<NativeOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestNative());
        }

        [Test]
        public void TestNativeNullableType()
        {
            NativeNullableOneOfContainer container = CoreHelper.JsonDeserialize<NativeNullableOneOfContainer>("null");
            Assert.IsNotNull(container);
            container.Match(new TestNativeNullable());
            container = CoreHelper.JsonDeserialize<NativeNullableOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestNativeNullable());
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
                var container = CoreHelper.JsonDeserialize<NativeOneOfContainer>("12");
                container.Match(new TestNative());
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
            NativeOneOfCollectionContainer container = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>("[\"some string array\", \"test\"]");
            Assert.IsNotNull(container);
            container.Match(new TestCollection());
            container = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCollection());
            container = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>("[0.987, 0.6987]");
            container.Match(new TestCollection());
            container = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCollection());

            container = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>("{\"key1\" : {\"NumberOfElectrons\":12,\"NumberOfProtons\":13} }");
            container.Match(new TestCollection());
            container = CoreHelper.JsonDeserialize<NativeOneOfCollectionContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCollection());
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
            NativeOneOfContainer[] container = CoreHelper.JsonDeserialize<NativeOneOfContainer[]>("[\"some string\", 0.987]");
            Assert.IsNotNull(container);
            foreach (var item in container)
            {
                item.Match(new TestNative());
            }
            container = CoreHelper.JsonDeserialize<NativeOneOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var item in container)
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
                var container = CoreHelper.JsonDeserialize<NativeOneOfContainer[]>("[\"some string\", 0.987, 12]");
                foreach (var item in container)
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
            CustomOneOfContainer container = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}");
            Assert.IsNotNull(container);
            container.Match(new TestCustom());
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustom());
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"}");
            container.Match(new TestCustom());
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustom());
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
                Orbit expected = new Orbit(12, "3");
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
            CustomOneOfWithDiscContainer container = CoreHelper.JsonDeserialize<CustomOneOfWithDiscContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}");
            Assert.IsNotNull(container);
            container.Match(new TestCustomWithDisc());
            container = CoreHelper.JsonDeserialize<CustomOneOfWithDiscContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustomWithDisc());
        }

        [Test]
        public void TestCustomInvalidType()
        {
            OneOfValidationException exception = null;

            try
            {
                var container = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"Name\":2,\"NumberOfShells\":12,\"NumberOfProtons\":13}");
                container.Match(new TestCustom());
            }
            catch (OneOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from atom, orbit on: {\n  \"Name\": 2,\n  \"NumberOfShells\": 12,\n  \"NumberOfProtons\": 13\n}", exception.Message.Replace("\r", ""));
        }

        [Test]
        public void TestCustomTwoTypeInvalid()
        {
            OneOfValidationException exception = null;

            try
            {
                var container = CoreHelper.JsonDeserialize<CustomOneOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13, \"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"}");
                container.Match(new TestCustom());
            }
            catch (OneOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            string expectedMessage = "There are more than one matching types i.e. atom and orbit on: {\n  \"NumberOfElectrons\": 12,\n  \"NumberOfProtons\": 13,\n  \"NumberOfShells\": \"3\"\n}";
            Assert.AreEqual(expectedMessage, exception.Message.Replace("\r", ""));
        }

        [Test]
        public void TestCustomTypeCollection()
        {
            CustomOneOfCollectionContainer container = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>("[{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}]");
            Assert.IsNotNull(container);
            container.Match(new TestCustomCollection());
            container = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustomCollection());
            container = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>("[{\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"}]");
            container.Match(new TestCustomCollection());
            container = CoreHelper.JsonDeserialize<CustomOneOfCollectionContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustomCollection());
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
                Orbit expectedOrbit = new Orbit(12, "3");
                Orbit[] expected = { expectedOrbit };
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }
        }

        [Test]
        public void TestCustomTypeOuter()
        {
            CustomOneOfContainer[] container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}, {\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"} ]");
            Assert.IsNotNull(container);
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"}, {\"NumberOfElectrons\":12,\"NumberOfProtons\":13}]");
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
        }

        [Test]
        public void TestCustomTypeInner()
        {
            CustomOneOfContainer[] container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[ {\"NumberOfElectrons\":12,\"NumberOfProtons\":13} ]");
            Assert.IsNotNull(container);
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>("[ {\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"} ]");
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
            container = CoreHelper.JsonDeserialize<CustomOneOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var form in container)
            {
                form.Match(new TestCustom());
            }
        }

        [Test]
        public void TestMixTypeNested()
        {
            CustomNestedOneOfContainer container = CoreHelper.JsonDeserialize<CustomNestedOneOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}");
            Assert.IsNotNull(container);
            container.Match(new TestCustomNested());
            container = CoreHelper.JsonDeserialize<CustomNestedOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustomNested());
            container = CoreHelper.JsonDeserialize<CustomNestedOneOfContainer>("\"some string\"");
            container.Match(new TestCustomNested());
            container = CoreHelper.JsonDeserialize<CustomNestedOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustomNested());
            container = CoreHelper.JsonDeserialize<CustomNestedOneOfContainer>("0.987");
            container.Match(new TestCustomNested());
            container = CoreHelper.JsonDeserialize<CustomNestedOneOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match(new TestCustomNested());
        }

        private class TestCustomNested : CustomNestedOneOfContainer.ICases<VoidType>
        {
            public VoidType Atom(Atom value)
            {
                Atom expected = new Atom(12, 13);
                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }

            public VoidType Orbit(NativeOneOfContainer value)
            {
                value.Match(new TestNative());
                return null;
            }
        }

        [Test]
        public void TestUnionTypeConverter()
        {
            NotImplementedException exception = null;

            try
            {
                var value = CoreHelper.JsonSerialize(TestContainer.FromMString("some string"));
                Console.WriteLine(value);
            }
            catch (NotImplementedException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("The method or operation is not implemented.", exception.Message);
        }
    }
}
