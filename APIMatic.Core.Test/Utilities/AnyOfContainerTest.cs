// <copyright file="SenderControllerTest.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using APIMatic.Core.Test.MockTypes.Models;
using APIMatic.Core.Test.MockTypes.Models.Containers;
using APIMatic.Core.Types;
using APIMatic.Core.Types.Sdk.Exceptions;
using APIMatic.Core.Utilities;
using NUnit.Framework;

namespace APIMatic.Core.Test.Utilities
{
    /// <summary>
    /// AnyOf Test Controller
    /// </summary>
    [TestFixture]
    public class AnyOfContainerTest
    {
        [Test]
        public void TestNativeType()
        {
            // Parameters for the API call
            NativeAnyOfContainer formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer>("\"some string\"");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestNative());
            formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestNative());
            formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer>("0.987");
            formScalar.Match(new TestNative());
            formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestNative());
        }

        private class TestNative : NativeAnyOfContainer.ICases<VoidType>
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

        [Test]
        public void TestNativeInvalidType()
        {
            AnyOfValidationException exception = null;

            try
            {
                var formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer>("12");
                formScalar.Match(new TestNative());
            }
            catch (AnyOfValidationException ex)
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
            NativeAnyOfCollectionContainer formScalar = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>("[\"some string array\"]");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestNativeCollection());
            formScalar = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestNativeCollection());
            formScalar = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>("[0.987]");
            formScalar.Match(new TestNativeCollection());
            formScalar = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestNativeCollection());
        }

        private class TestNativeCollection : NativeAnyOfCollectionContainer.ICases<VoidType>
        {
            public VoidType MString(string[] value)
            {
                string[] expectedStringArray = { "some string array" };
                CollectionAssert.AreEqual(expectedStringArray, value);
                Console.WriteLine(string.Join(", ", value));
                return null;
            }

            public VoidType Precision(double[] value)
            {
                double[] expectedPrecision = { 0.987 };
                CollectionAssert.AreEqual(expectedPrecision, value);
                Console.WriteLine(string.Join(", ", value));
                return null;
            }
        }

        [Test]
        public void TestNativeOuterListContainer()
        {
            // Parameters for the API call
            NativeAnyOfContainer[] formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer[]>("[\"some string\", 0.987]");
            Assert.IsNotNull(formScalar);
            foreach (var item in formScalar)
            {
                item.Match(new TestNative());
            }
            formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var item in formScalar)
            {
                item.Match(new TestNative());
            }
        }

        [Test]
        public void TestNativeInvalidOuterList()
        {
            AnyOfValidationException exception = null;

            try
            {
                var formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer[]>("[\"some string\", 0.987, 12]");
                foreach (var item in formScalar)
                {
                    item.Match(new TestNative());
                }
            }
            catch (AnyOfValidationException ex)
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
            CustomAnyOfContainer formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCustom());
            formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustom());
            formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfShells\":3}");
            formScalar.Match(new TestCustom());
            formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustom());
        }

        private class TestCustom : CustomAnyOfContainer.ICases<VoidType>
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


        [Test]
        public void TestCustomInvalidType()
        {
            AnyOfValidationException exception = null;

            try
            {
                var formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{\"NumberOfShells\":12,\"NumberOfProtons\":13}");
                formScalar.Match(new TestCustom());
            }
            catch (AnyOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from atom, orbit on: {\n  \"NumberOfShells\": 12,\n  \"NumberOfProtons\": 13\n}", exception.Message.Replace("\r", ""));
        }

        [Test]
        public void TestCustomTwoValidType()
        {
            var formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{ \"NumberOfElectrons\":12,\"NumberOfProtons\":13, \"NumberOfElectrons\":12,\"NumberOfShells\":3 }");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCustom());
        }

        [Test]
        public void TestCustomTypeOuter()
        {
            // Parameters for the API call
            CustomAnyOfContainer[] formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}, {\"NumberOfElectrons\":12,\"NumberOfShells\":3} ]");
            Assert.IsNotNull(formScalar);
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfShells\":3}, {\"NumberOfElectrons\":12,\"NumberOfProtons\":13}]");
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
            formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>(CoreHelper.JsonSerialize(formScalar));
            foreach (var form in formScalar)
            {
                form.Match(new TestCustom());
            }
        }

        [Test]
        public void TestCustomTypeArrayOfMap()
        {
            // Parameters for the API call
            ArrayOfMapContainer formScalar = CoreHelper.JsonDeserialize<ArrayOfMapContainer>("[ { \"key1\" : {\"NumberOfElectrons\":12,\"NumberOfProtons\":13 } } ]");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCustomArrayOfMap());
            formScalar = CoreHelper.JsonDeserialize<ArrayOfMapContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustomArrayOfMap());
            formScalar = CoreHelper.JsonDeserialize<ArrayOfMapContainer>("[{ \"key1\" : {\"NumberOfElectrons\":12,\"NumberOfShells\":3 } } ]");
            formScalar.Match(new TestCustomArrayOfMap());
            formScalar = CoreHelper.JsonDeserialize<ArrayOfMapContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustomArrayOfMap());
        }

        private class TestCustomArrayOfMap : ArrayOfMapContainer.ICases<VoidType>
        {
            public VoidType Atom(List<Dictionary<string, Atom>> value)
            {
                Atom expectedAtom = new Atom(12, 13);
                List<Dictionary<string, Atom>> expected = new List<Dictionary<string, Atom>>
                {
                    new Dictionary<string, Atom>()
                    {
                        { "key1", expectedAtom }
                    }
                };

                Assert.AreEqual(expected, value);
                Console.WriteLine(value.ToString());
                return null;
            }

            public VoidType Orbit(List<Dictionary<string, Orbit>> value)
            {
                Orbit expectedOrbit = new Orbit(12, 3);
                List<Dictionary<string, Orbit>> expected = new List<Dictionary<string, Orbit>>
                {
                    new Dictionary<string, Orbit>()
                    {
                        { "key1", expectedOrbit }
                    }
                };

                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }
        }

        [Test]
        public void TestCustomTypeMapOfArray()
        {
            // Parameters for the API call
            MapOfArrayContainer formScalar = CoreHelper.JsonDeserialize<MapOfArrayContainer>("{ \"key1\" : [{\"NumberOfElectrons\":12,\"NumberOfProtons\":13 } ]} ");
            Assert.IsNotNull(formScalar);
            formScalar.Match(new TestCustomMapOfArray());
            formScalar = CoreHelper.JsonDeserialize<MapOfArrayContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustomMapOfArray());
            formScalar = CoreHelper.JsonDeserialize<MapOfArrayContainer>("{ \"key1\" : [{\"NumberOfElectrons\":12,\"NumberOfShells\":3 } ] } ");
            formScalar.Match(new TestCustomMapOfArray());
            formScalar = CoreHelper.JsonDeserialize<MapOfArrayContainer>(CoreHelper.JsonSerialize(formScalar));
            formScalar.Match(new TestCustomMapOfArray());
        }

        private class TestCustomMapOfArray : MapOfArrayContainer.ICases<VoidType>
        {
            public VoidType Atom(Dictionary<string, List<Atom>> value)
            {
                Atom expectedAtom = new Atom(12, 13);
                Dictionary<string, List<Atom>> expected = new Dictionary<string, List<Atom>>
                {
                    { "key1", new List<Atom>
                        {
                            expectedAtom
                        }
                    }
                };

                Assert.AreEqual(expected, value);
                Console.WriteLine(value.ToString());
                return null;
            }

            public VoidType Orbit(Dictionary<string, List<Orbit>> value)
            {
                Orbit expectedOrbit = new Orbit(12, 3);
                Dictionary<string, List<Orbit>> expected = new Dictionary<string, List<Orbit>>
                {
                    {
                        "key1", new List<Orbit>
                        {
                            expectedOrbit
                        }
                    }
                };

                Assert.AreEqual(expected, value);
                Console.WriteLine(value);
                return null;
            }
        }
    }
}
