﻿// <copyright file="SenderControllerTest.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
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
            public VoidType MString(string[] mString)
            {
                string[] expectedStringArray = { "some string array" };
                CollectionAssert.AreEqual(expectedStringArray, mString);
                Console.WriteLine(string.Join(", ", mString));
                return null;
            }

            public VoidType Precision(double[] precision)
            {
                double[] expectedPrecision = { 0.987 };
                CollectionAssert.AreEqual(expectedPrecision, precision);
                Console.WriteLine(string.Join(", ", precision));
                return null;
            }
        }

        [Test]
        public void TestNativeOuterListContainer()
        {
            // Parameters for the API call
            NativeAnyOfContainer[] formScalar = CoreHelper.JsonDeserialize<NativeAnyOfContainer[]>("[\"some string\", 0.987]");
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
            Assert.AreEqual("We could not match any acceptable type from atom, orbit on: {\r\n  \"NumberOfShells\": 12,\r\n  \"NumberOfProtons\": 13\r\n}", exception.Message);
        }

        [Test]
        public void TestCustomTwoValidType()
        {
            var formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{ \"NumberOfElectrons\":12,\"NumberOfProtons\":13, \"NumberOfElectrons\":12,\"NumberOfShells\":3 }");
            formScalar.Match(new TestCustom());
        }

        [Test]
        public void TestCustomTypeOuter()
        {
            // Parameters for the API call
            CustomAnyOfContainer[] formScalar = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfProtons\":13}, {\"NumberOfElectrons\":12,\"NumberOfShells\":3} ]");
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
    }
}
