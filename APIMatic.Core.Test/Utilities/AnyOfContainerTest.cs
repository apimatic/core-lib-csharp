﻿// <copyright file="SenderControllerTest.cs" company="APIMatic">
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
            NativeAnyOfContainer container = CoreHelper.JsonDeserialize<NativeAnyOfContainer>("\"some string\"");
            Assert.IsNotNull(container);
            container.Match<VoidType>(
                    precision: value =>
                    {
                        Assert.AreEqual(0.987d, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    mString: value =>
                    {
                        Assert.AreEqual("some string", value);
                        Console.WriteLine(value);
                        return null;
                    });
            container = CoreHelper.JsonDeserialize<NativeAnyOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                    precision: value =>
                    {
                        Assert.AreEqual(0.987d, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    mString: value =>
                    {
                        Assert.AreEqual("some string", value);
                        Console.WriteLine(value);
                        return null;
                    });
            container = CoreHelper.JsonDeserialize<NativeAnyOfContainer>("0.987");
            container.Match<VoidType>(
                     precision: value =>
                     {
                         Assert.AreEqual(0.987d, value);
                         Console.WriteLine(value);
                         return null;
                     },
                     mString: value =>
                     {
                         Assert.AreEqual("some string", value);
                         Console.WriteLine(value);
                         return null;
                     });
            container = CoreHelper.JsonDeserialize<NativeAnyOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                     precision: value =>
                     {
                         Assert.AreEqual(0.987d, value);
                         Console.WriteLine(value);
                         return null;
                     },
                     mString: value =>
                     {
                         Assert.AreEqual("some string", value);
                         Console.WriteLine(value);
                         return null;
                     });
        }

        [Test]
        public void TestNativeInvalidType()
        {
            AnyOfValidationException exception = null;

            try
            {
                var container = CoreHelper.JsonDeserialize<NativeAnyOfContainer>("12");
                container.Match<VoidType>(
                     precision: value =>
                     {
                         Assert.AreEqual(0.987d, value);
                         Console.WriteLine(value);
                         return null;
                     },
                     mString: value =>
                     {
                         Assert.AreEqual("some string", value);
                         Console.WriteLine(value);
                         return null;
                     });
            }
            catch (AnyOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from double, string on: 12", exception.Message);
        }


        [Test]
        public void TestNativeCollectionType()
        {
            NativeAnyOfCollectionContainer container = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>("[\"some string array\"]");
            Assert.IsNotNull(container);
            container.Match<VoidType>(
                precisionArray: value =>
                {
                    double[] expectedPrecision = { 0.987 };
                    CollectionAssert.AreEqual(expectedPrecision, value);
                    Console.WriteLine(string.Join(", ", value));
                    return null;
                },
                mStringArray: value =>
                {
                    string[] expectedStringArray = { "some string array" };
                    CollectionAssert.AreEqual(expectedStringArray, value);
                    Console.WriteLine(string.Join(", ", value));
                    return null;
                });
            container = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                 precisionArray: value =>
                 {
                     double[] expectedPrecision = { 0.987 };
                     CollectionAssert.AreEqual(expectedPrecision, value);
                     Console.WriteLine(string.Join(", ", value));
                     return null;
                 },
                 mStringArray: value =>
                 {
                     string[] expectedStringArray = { "some string array" };
                     CollectionAssert.AreEqual(expectedStringArray, value);
                     Console.WriteLine(string.Join(", ", value));
                     return null;
                 });
            container = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>("[0.987]");
            container.Match<VoidType>(
                 precisionArray: value =>
                 {
                     double[] expectedPrecision = { 0.987 };
                     CollectionAssert.AreEqual(expectedPrecision, value);
                     Console.WriteLine(string.Join(", ", value));
                     return null;
                 },
                 mStringArray: value =>
                 {
                     string[] expectedStringArray = { "some string array" };
                     CollectionAssert.AreEqual(expectedStringArray, value);
                     Console.WriteLine(string.Join(", ", value));
                     return null;
                 });
            container = CoreHelper.JsonDeserialize<NativeAnyOfCollectionContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                precisionArray: value =>
                {
                    double[] expectedPrecision = { 0.987 };
                    CollectionAssert.AreEqual(expectedPrecision, value);
                    Console.WriteLine(string.Join(", ", value));
                    return null;
                },
                mStringArray: value =>
                {
                    string[] expectedStringArray = { "some string array" };
                    CollectionAssert.AreEqual(expectedStringArray, value);
                    Console.WriteLine(string.Join(", ", value));
                    return null;
                });
        }

        [Test]
        public void TestNativeOuterListContainer()
        {
            NativeAnyOfContainer[] container = CoreHelper.JsonDeserialize<NativeAnyOfContainer[]>("[\"some string\", 0.987]");
            Assert.IsNotNull(container);
            foreach (var item in container)
            {
                item.Match<VoidType>(
                    precision: value =>
                    {
                        Assert.AreEqual(0.987d, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    mString: value =>
                    {
                        Assert.AreEqual("some string", value);
                        Console.WriteLine(value);
                        return null;
                    });
            }
            container = CoreHelper.JsonDeserialize<NativeAnyOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var item in container)
            {
                item.Match<VoidType>(
                    precision: value =>
                    {
                        Assert.AreEqual(0.987d, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    mString: value =>
                    {
                        Assert.AreEqual("some string", value);
                        Console.WriteLine(value);
                        return null;
                    });
            }
        }

        [Test]
        public void TestNativeInvalidOuterList()
        {
            AnyOfValidationException exception = null;

            try
            {
                var container = CoreHelper.JsonDeserialize<NativeAnyOfContainer[]>("[\"some string\", 0.987, 12]");
                foreach (var item in container)
                {
                    item.Match<VoidType>(
                    precision: value =>
                    {
                        Assert.AreEqual(0.987d, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    mString: value =>
                    {
                        Assert.AreEqual("some string", value);
                        Console.WriteLine(value);
                        return null;
                    });
                }
            }
            catch (AnyOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from double, string on: 12", exception.Message);
        }

        [Test]
        public void TestCustomType()
        {
            CustomAnyOfContainer container = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfProtons\":13,\"Name\":\"Hydrogen\"}");
            Assert.IsNotNull(container);
            container.Match<VoidType>(
                atom: value =>
                {
                    Atom expected = new Atom(12, 13, "Hydrogen");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                },
                orbit: value =>
                {
                    Orbit expected = new Orbit(12, "3");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                });
            container = CoreHelper.JsonDeserialize<CustomAnyOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                 atom: value =>
                 {
                     Atom expected = new Atom(12, 13, "Hydrogen");
                     Assert.AreEqual(expected, value);
                     Console.WriteLine(value);
                     return null;
                 },
                 orbit: value =>
                 {
                     Orbit expected = new Orbit(12, "3");
                     Assert.AreEqual(expected, value);
                     Console.WriteLine(value);
                     return null;
                 });
            container = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"}");
            container.Match<VoidType>(
                atom: value =>
                {
                    Atom expected = new Atom(12, 13, "Hydrogen");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                },
                orbit: value =>
                {
                    Orbit expected = new Orbit(12, "3");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                });
            container = CoreHelper.JsonDeserialize<CustomAnyOfContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                atom: value =>
                {
                    Atom expected = new Atom(12, 13, "Hydrogen");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                },
                orbit: value =>
                {
                    Orbit expected = new Orbit(12, "3");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                });
        }

        [Test]
        public void TestCustomInvalidType()
        {
            AnyOfValidationException exception = null;

            try
            {
                var container = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{\"NumberOfShells\":\"12\",\"NumberOfProtons\":13}");
                container.Match<VoidType>(
                atom: value =>
                {
                    Atom expected = new Atom(12, 13, "Hydrogen");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                },
                orbit: value =>
                {
                    Orbit expected = new Orbit(12, "3");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                });
            }
            catch (AnyOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from atom, orbit on: {\n  \"NumberOfShells\": \"12\",\n  \"NumberOfProtons\": 13\n}", exception.Message.Replace("\r", ""));
        }

        [Test]
        public void TestCustomTwoValidType()
        {
            var container = CoreHelper.JsonDeserialize<CustomAnyOfContainer>("{ \"NumberOfElectrons\":12,\"NumberOfProtons\":13, \"NumberOfElectrons\":12,\"NumberOfShells\":\"3\",\"Name\":\"Hydrogen\" }");
            Assert.IsNotNull(container);
            container.Match<VoidType>(
                atom: value =>
                {
                    Atom expected = new Atom(12, 13, "Hydrogen");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                },
                orbit: value =>
                {
                    Orbit expected = new Orbit(12, "3");
                    Assert.AreEqual(expected, value);
                    Console.WriteLine(value);
                    return null;
                });
        }

        [Test]
        public void TestCustomTypeOuter()
        {
            CustomAnyOfContainer[] container = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfProtons\":13,\"Name\":\"Hydrogen\"}, {\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\",\"Name\":\"Hydrogen\"} ]");
            Assert.IsNotNull(container);
            foreach (var form in container)
            {
                form.Match<VoidType>(
                    atom: value =>
                    {
                        Atom expected = new Atom(12, 13, "Hydrogen");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    orbit: value =>
                    {
                        Orbit expected = new Orbit(12, "3");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    });
            }
            container = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var form in container)
            {
                form.Match<VoidType>(
                    atom: value =>
                    {
                        Atom expected = new Atom(12, 13, "Hydrogen");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    orbit: value =>
                    {
                        Orbit expected = new Orbit(12, "3");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    });
            }
            container = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>("[{\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\"}, {\"NumberOfElectrons\":12,\"NumberOfProtons\":13,\"Name\":\"Hydrogen\"}]");
            foreach (var form in container)
            {
                form.Match<VoidType>(
                    atom: value =>
                    {
                        Atom expected = new Atom(12, 13, "Hydrogen");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    orbit: value =>
                    {
                        Orbit expected = new Orbit(12, "3");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    });
            }
            container = CoreHelper.JsonDeserialize<CustomAnyOfContainer[]>(CoreHelper.JsonSerialize(container));
            foreach (var form in container)
            {
                form.Match<VoidType>(
                    atom: value =>
                    {
                        Atom expected = new Atom(12, 13, "Hydrogen");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    },
                    orbit: value =>
                    {
                        Orbit expected = new Orbit(12, "3");
                        Assert.AreEqual(expected, value);
                        Console.WriteLine(value);
                        return null;
                    });
            }
        }

        [Test]
        public void TestCustomTypeArrayOfMap()
        {
            ArrayOfMapContainer container = CoreHelper.JsonDeserialize<ArrayOfMapContainer>("[ { \"key1\" : {\"NumberOfElectrons\":12,\"NumberOfProtons\":13 } } ]");
            Assert.IsNotNull(container);
            container.Match<VoidType>(
                atom: value =>
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
                },
                orbit: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
            container = CoreHelper.JsonDeserialize<ArrayOfMapContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                atom: value =>
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
                },
                orbit: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
            container = CoreHelper.JsonDeserialize<ArrayOfMapContainer>("[{ \"key1\" : {\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\" } } ]");
            container.Match<VoidType>(
                atom: value =>
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
                },
                orbit: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
            container = CoreHelper.JsonDeserialize<ArrayOfMapContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                atom: value =>
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
                },
                orbit: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
        }

        [Test]
        public void TestCustomTypeMapOfArray()
        {
            MapOfArrayContainer container = CoreHelper.JsonDeserialize<MapOfArrayContainer>("{ \"key1\" : [{\"NumberOfElectrons\":12,\"NumberOfProtons\":13 } ]} ");
            Assert.IsNotNull(container);
            container.Match<VoidType>(
                atomDictionay: value =>
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
                },
                orbitsDictionay: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
            container = CoreHelper.JsonDeserialize<MapOfArrayContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                atomDictionay: value =>
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
                },
                orbitsDictionay: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
            container = CoreHelper.JsonDeserialize<MapOfArrayContainer>("{ \"key1\" : [{\"NumberOfElectrons\":12,\"NumberOfShells\":\"3\" } ] } ");
            container.Match<VoidType>(
                atomDictionay: value =>
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
                },
                orbitsDictionay: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
            container = CoreHelper.JsonDeserialize<MapOfArrayContainer>(CoreHelper.JsonSerialize(container));
            container.Match<VoidType>(
                atomDictionay: value =>
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
                },
                orbitsDictionay: value =>
                {
                    Orbit expectedOrbit = new Orbit(12, "3");
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
                });
        }

        [Test]
        public void TestNativeDateTimeType()
        {
            NativeDateTimeAnyOfContainer container = CoreHelper.JsonDeserialize<NativeDateTimeAnyOfContainer>("\"1994-02-13T14:01:54.9571247Z\"");

            Assert.IsNotNull(container);
            var dateTime = container.Match<DateTime?>(
                rfc1123DateTime: value =>
                {
                    Console.WriteLine(value);
                    return value;
                },
                rfc3339DateTime: value =>
                {
                    Console.WriteLine(value);
                    return value;
                });
            Assert.IsNotNull(dateTime);
            container = CoreHelper.JsonDeserialize<NativeDateTimeAnyOfContainer>(CoreHelper.JsonSerialize(container));
            dateTime = container.Match<DateTime?>(
                rfc1123DateTime: value =>
                {
                    Console.WriteLine(value);
                    return value;
                },
                rfc3339DateTime: value =>
                {
                    Console.WriteLine(value);
                    return value;
                });
            Assert.IsNotNull(dateTime);
            container = CoreHelper.JsonDeserialize<NativeDateTimeAnyOfContainer>("\"2023-07-20T14:30:00Z\"");
            dateTime = container.Match<DateTime?>(
                rfc1123DateTime: value =>
                {
                    Console.WriteLine(value);
                    return value;
                },
                rfc3339DateTime: value =>
                {
                    Console.WriteLine(value);
                    return value;
                });
            Assert.IsNotNull(dateTime);
            container = CoreHelper.JsonDeserialize<NativeDateTimeAnyOfContainer>(CoreHelper.JsonSerialize(container));
            dateTime = container.Match<DateTime?>(
                 rfc1123DateTime: value =>
                 {
                     Console.WriteLine(value);
                     return value;
                 },
                 rfc3339DateTime: value =>
                 {
                     Console.WriteLine(value);
                     return value;
                 });
            Assert.IsNotNull(dateTime);
        }

        [Test]
        public void TestEnumType()
        {
            EnumAnyOfContainer container = CoreHelper.JsonDeserialize<EnumAnyOfContainer>("\"Monday\"");
            Assert.AreEqual("WorkingDays: Monday", container.Match<string>(
                workingDays: value =>
                {
                    return $"WorkingDays: {value}";
                },
                days: value =>
                {
                    return $"Days: {value}";
                },
                monthNumber: value =>
                {
                    return $"MonthNumber: {value}";
                }));

            container = CoreHelper.JsonDeserialize<EnumAnyOfContainer>(CoreHelper.JsonSerialize(container));
            Assert.AreEqual("WorkingDays: Monday", container.Match<string>(
                workingDays: value =>
                {
                    return $"WorkingDays: {value}";
                },
                days: value =>
                {
                    return $"Days: {value}";
                },
                monthNumber: value =>
                {
                    return $"MonthNumber: {value}";
                }));

            container = CoreHelper.JsonDeserialize<EnumAnyOfContainer>("\"Sunday\"");
            Assert.AreEqual("Days: Sunday", container.Match<string>(
                workingDays: value =>
                {
                    return $"WorkingDays: {value}";
                },
                days: value =>
                {
                    return $"Days: {value}";
                },
                monthNumber: value =>
                {
                    return $"MonthNumber: {value}";
                }));

            container = CoreHelper.JsonDeserialize<EnumAnyOfContainer>("2");
            Assert.AreEqual("MonthNumber: February", container.Match<string>(
                workingDays: value =>
                {
                    return $"WorkingDays: {value}";
                },
                days: value =>
                {
                    return $"Days: {value}";
                },
                monthNumber: value =>
                {
                    return $"MonthNumber: {value}";
                }));

            Assert.AreEqual("2", CoreHelper.JsonSerialize(container));
        }

        [Test]
        public void TestEnumTypeFailure()
        {
            AnyOfValidationException exception = null;
            try
            {
                CoreHelper.JsonDeserialize<EnumAnyOfContainer>("\"Invalid value\"");
            }
            catch (AnyOfValidationException ex)
            {
                exception = ex;
            }

            Assert.NotNull(exception);
            Assert.AreEqual("We could not match any acceptable type from workingdays, days, monthnumber on: Invalid value", exception.Message);
        }
    }
}
