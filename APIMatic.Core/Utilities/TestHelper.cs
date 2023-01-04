// <copyright file="TestHelper.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;

namespace APIMatic.Core.Utilities
{
    /// <summary>
    /// TestHelper Class.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// Check if left array of objects is a subset of right array.
        /// </summary>
        /// <param name="leftObject">Left array as a JSON string.</param>
        /// <param name="rightObject">Right array as a JSON string.</param>
        /// <param name="checkValues">Check primitive values for equality?.</param>
        /// <param name="allowExtra">Are extra elements allowed in right array?.</param>
        /// <param name="isOrdered">Should elements in right be compared in order to left?.</param>
        /// <returns>True if it is a subset.</returns>
        public static bool IsArrayOfJsonObjectsProperSubsetOf(
            string leftObject,
            string rightObject,
            bool checkValues,
            bool allowExtra,
            bool isOrdered)
        {
            // Deserialize left and right objects from their respective strings
            JArray left = CoreHelper.JsonDeserialize<dynamic>(leftObject);
            JArray right = CoreHelper.JsonDeserialize<dynamic>(rightObject);

            return IsArrayOfJsonObjectsProperSubsetOf(left, right, checkValues, allowExtra, isOrdered);
        }

        /// <summary>
        /// Check if left array of objects is a subset of right array.
        /// </summary>
        /// <param name="leftList">Left array.</param>
        /// <param name="rightList">Right array.</param>
        /// <param name="checkValues">Check primitive values for equality?.</param>
        /// <param name="allowExtra">Are extra elements allowed in right array?.</param>
        /// <param name="isOrdered">Should elements in right be compared in order to left?.</param>
        /// <returns>True if it is a subset.</returns>
        public static bool IsArrayOfJsonObjectsProperSubsetOf(
                JArray leftList,
                JArray rightList,
                bool checkValues,
                bool allowExtra,
                bool isOrdered)
        {
            // Return false if size different and checking was strict
            if (IsDifferentSizeListAllowed(leftList, rightList, allowExtra))
            {
                return false;
            }

            // Create list iterators
            var leftIter = leftList.GetEnumerator();
            var rightIter = rightList.GetEnumerator();

            // Iterate left list and check if each value is present in the right list
            while (leftIter.MoveNext())
            {
                var leftTree = leftIter.Current;
                bool found = false;

                // restart right iterator if ordered comparision is not required
                if (!isOrdered)
                {
                    rightIter = rightList.GetEnumerator();
                }

                while (rightIter.MoveNext())
                {
                    if (IsProperSubsetOf((JObject)leftTree, (JObject)rightIter.Current, checkValues, allowExtra, isOrdered))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsDifferentSizeListAllowed(JArray leftList, JArray rightList, bool allowExtra)
        {
            return (!allowExtra) && (rightList.Count != leftList.Count);
        }

        /// <summary>
        /// Check whether the a list (as JSON string) is a subset of another list (as JSON string).
        /// </summary>
        /// <param name="leftListJson">Expected List.</param>
        /// <param name="rightListJson">List to check.</param>
        /// <param name="allowExtra">Are extras allowed in the list to check?.</param>
        /// <param name="isOrdered">Should checking be in order?.</param>
        /// <returns>True if it is a subset.</returns>
        public static bool IsListProperSubsetOf(
                string leftListJson,
                string rightListJson,
                bool allowExtra,
                bool isOrdered)
        {
            // Deserialize left and right lists from their respective strings
            JArray left = CoreHelper.JsonDeserialize<dynamic>(leftListJson);
            JArray right = CoreHelper.JsonDeserialize<dynamic>(rightListJson);

            return IsListProperSubsetOf(left, right, allowExtra, isOrdered);
        }

        /// <summary>
        /// Check whether the a list is a subset of another list.
        /// </summary>
        /// <param name="leftList">Expected List.</param>
        /// <param name="rightList">List to check.</param>
        /// <param name="allowExtra">Are extras allowed in the list to check?.</param>
        /// <param name="isOrdered">Should checking be in order?.</param>
        /// <returns>True if it is a subset.</returns>
        public static bool IsListProperSubsetOf(
                JArray leftList,
                JArray rightList,
                bool allowExtra,
                bool isOrdered)
        {
            if (IsDifferentSizeListAllowed(leftList, rightList, allowExtra))
            {
                return false;
            }

            if (isOrdered)
            {
                if (rightList.Count < leftList.Count)
                {
                    return false;
                }

                int rIndex = 0, lIndex = 0;
                while (rIndex < rightList.Count)
                {
                    if (rightList[rIndex].ToString() == leftList[lIndex].ToString())
                    {
                        lIndex++;
                    }

                    rIndex++;
                }

                return lIndex == leftList.Count;
            }

            return IsSuperSetOf(left: rightList, right: leftList);
        }

        /// <summary>
        /// Recursively check whether the left headers map is a proper subset of the right headers map.
        /// </summary>
        /// <param name="leftDict">Left headers map.</param>
        /// <param name="rightDict">Right headers map.</param>
        /// <returns>True if it is a subset.</returns>
        public static bool AreHeadersProperSubsetOf(
                Dictionary<string, string> leftDict,
                Dictionary<string, string> rightDict)
        {
            Dictionary<string, string> leftDictInv = new Dictionary<string, string>(leftDict, StringComparer.CurrentCultureIgnoreCase);
            Dictionary<string, string> rightDictInv = new Dictionary<string, string>(rightDict, StringComparer.CurrentCultureIgnoreCase);

            foreach (var leftKey in leftDictInv.Keys)
            {
                if (!rightDictInv.ContainsKey(leftKey))
                {
                    return false;
                }

                if (leftDictInv[leftKey] == null)
                {
                    continue;
                }

                if (!leftDictInv[leftKey].Equals(rightDictInv[leftKey]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Compare the input stream to file byte-by-byte.
        /// </summary>
        /// <param name="file">First input.</param>
        /// <param name="input">Second input.</param>
        /// <returns>True if stream contains the same content as the file.</returns>
        public static bool IsSameAsFile(string file, Stream input)
        {
            return IsSameInputStream(GetFile(file, (stream, _) => stream), input);
        }

        /// <summary>
        /// Compare two input streams.
        /// </summary>
        /// <param name="input1">First stream.</param>
        /// <param name="input2">Second stream.</param>
        /// <returns>True if streams contain the same content.</returns>
        public static bool IsSameInputStream(Stream input1, Stream input2)
        {
            if (input1 == input2)
            {
                return true;
            }

            int ch = input1.ReadByte();
            while (ch != -1)
            {
                int ch2 = input2.ReadByte();
                if (ch != ch2)
                {
                    return false;
                }

                ch = input1.ReadByte();
            }

            // should reach end of stream
            bool input2Finished = input2.ReadByte() == -1;
            input1.Dispose();
            input2.Dispose();

            return input2Finished;
        }

        /// <summary>
        /// Downloads a given url and return a path to its local version.
        /// Files are cached.Second call for the same URL will return cached version.
        /// </summary>
        /// <param name="url">URL to download.</param>
        /// <param name="fileCreator">Function to convert filestream into any Selected Type.</param>
        /// <returns>Absolute path to the local downloaded version of file.</returns>
        public static T GetFile<T>(string url, Func<Stream, string, T> fileCreator)
        {
            string originalFileName = Path.GetFileName(url);
            string tmpFileName = "sdk_tests" + ToSHA1(url) + ".tmp";
            string filePath = Path.Combine(Path.GetTempPath(), tmpFileName);
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                File.WriteAllBytes(filePath, CoreHelper.RunTask(new HttpClient().GetByteArrayAsync(url)));
            }
            return fileCreator(File.OpenRead(filePath), originalFileName);
        }

        /// <summary>
        /// Get SHA1 hash of a string.
        /// </summary>
        /// <param name="convertme">The string to convert.</param>
        /// <returns>SHA1 hash.</returns>
        public static string ToSHA1(string convertme)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(convertme);
            using (var sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(bytes);
                return ByteArrayToHexString(hashBytes);
            }
        }

        /// <summary>
        /// Convert byte array to the hexadecimal representation in string.
        /// </summary>
        /// <param name="bytes">Byte array to convert.</param>
        /// <returns>Hex representation in string.</returns>
        public static string ByteArrayToHexString(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Checks if the left items set is the superset of right items set.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="left">Left items set.</param>
        /// <param name="right">Right items set.</param>
        /// <returns>True if the left has all items of right.</returns>
        public static bool IsSuperSetOf<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            HashSet<T> lHashSet = new HashSet<T>(left);
            return lHashSet.IsSupersetOf(right);
        }

        /// <summary>
        /// Checks if the left items ordered set is the ordered superset of right items ordered set.
        /// </summary>
        /// <typeparam name="T">Type of items.</typeparam>
        /// <param name="left">Left items set.</param>
        /// <param name="right">Right items set.</param>
        /// <param name="checkSize">Should the size of left and right be equal as well.</param>
        /// <returns>True if the left has all items of right in the same order.</returns>
        public static bool IsOrderedSupersetOf<T>(this IEnumerable<T> left, IEnumerable<T> right, bool checkSize = false)
        {
            var lItr = left.GetEnumerator();
            var rItr = right.GetEnumerator();

            while (lItr.MoveNext())
            {
                T lCurrent = lItr.Current;

                // right list ended prematurely
                if (!rItr.MoveNext())
                {
                    return false;
                }

                T rCurrent = rItr.Current;

                if (!lCurrent.Equals(rCurrent))
                {
                    return false;
                }
            }

            // if checking for size, right items should have been exhaustively read
            if (checkSize && rItr.MoveNext())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Recursively check whether the left JSON object is a proper subset of the right JSON object.
        /// </summary>
        /// <param name="leftObject">Left JSON object as string.</param>
        /// <param name="rightObject">rightObject Right JSON object as string.</param>
        /// <param name="checkValues">Check primitive values for equality?.</param>
        /// <param name="allowExtra">Are extra elements allowed in right array?.</param>
        /// <param name="isOrdered">Should elements in right be compared in order to left?.</param>
        /// <returns>True, if the given object is a proper subset of other other.</returns>
        public static bool IsJsonObjectProperSubsetOf(
                string leftObject,
                string rightObject,
                bool checkValues,
                bool allowExtra,
                bool isOrdered)
        {
            return IsProperSubsetOf(
                CoreHelper.JsonDeserialize<dynamic>(leftObject),
                CoreHelper.JsonDeserialize<dynamic>(rightObject),
                checkValues,
                allowExtra,
                isOrdered);
        }

        /// <summary>
        /// Convert an InputStream to a string (utility function).
        /// </summary>
        /// <param name="inStream">The input stream to read.</param>
        /// <returns>string read from the stream.</returns>
        public static string ConvertStreamToString(Stream inStream)
        {
            using (StreamReader reader = new StreamReader(inStream))
            {
                var str = reader.ReadToEnd();
                return str;
            }
        }

        /// <summary>
        /// Recursively check whether the leftTree is a proper subset of the right tree.
        /// </summary>
        /// <param name="leftTree">Left tree.</param>
        /// <param name="rightTree">Right tree.</param>
        /// <param name="checkValues">Check primitive values for equality?.</param>
        /// <param name="allowExtra">Are extra elements allowed in right array?.</param>
        /// <param name="isOrdered">Should elements in right be compared in order to left?.</param>
        /// <returns>Boolean.</returns>
        private static bool IsProperSubsetOf(
            JObject leftTree,
            JObject rightTree,
            bool checkValues,
            bool allowExtra,
            bool isOrdered)
        {
            foreach (var property in leftTree.Properties())
            {
                // Check if key exists
                if (rightTree.Property(property.Name) == null)
                {
                    return false;
                }

                object leftVal = property.Value;
                object rightVal = rightTree.Property(property.Name).Value;

                if (leftVal is JObject leftSideValue)
                {
                    return IsProperSubsetOfJObject(checkValues, allowExtra, isOrdered, rightVal, leftSideValue);
                }
                else if (checkValues)
                {
                    return CheckValuesAreSameOnBothSides(allowExtra, isOrdered, leftVal, rightVal);
                }
            }
            return true;
        }

        private static bool IsProperSubsetOfJObject(bool checkValues, bool allowExtra, bool isOrdered, object rightVal, JObject leftSideValue)
        {
            bool isProperSubset = true;
            // If left value is tree, right value should be be tree too
            if (!(rightVal is JObject rightSideValue))
            {
                return false;
            }
            if (!IsProperSubsetOf(leftSideValue, rightSideValue, checkValues, allowExtra, isOrdered))
            {
                isProperSubset = false;
            }
            return isProperSubset;
        }

        private static bool CheckValuesAreSameOnBothSides(bool allowExtra, bool isOrdered, object leftVal, object rightVal)
        {
            bool isSame = true;
            // If left value is a primitive, check if it equals right value
            if (leftVal is JArray leftJArray)
            {
                if (!DoesRightValContainsSameItems(leftJArray, rightVal, allowExtra, isOrdered))
                {
                    isSame = false;
                }
            }
            else if (!leftVal.Equals(rightVal))
            {
                isSame = false;
            }
            return isSame;
        }

        private static bool DoesRightValContainsSameItems(JArray leftJArray, object rightVal, bool allowExtra, bool isOrdered)
        {
            if (!(rightVal is JArray rightJArray))
            {
                return false;
            }

            bool bothArrayContainsJObject = IsArrayOfJObject(leftJArray) && IsArrayOfJObject(rightJArray);
            bool containsJObject = ListContainsJObject(leftJArray) && ListContainsJObject(rightJArray);
            if (!bothArrayContainsJObject && containsJObject)
            {
                var leftJToken = leftJArray.Where(x => x is JObject);
                JArray leftArray = new JArray(leftJToken);
                var rightToken = rightJArray.Where(x => x is JObject);
                JArray rightArray = new JArray(rightToken);
                // is array of objects
                if (!IsArrayOfJsonObjectsProperSubsetOf(leftArray, rightArray,
                    true, allowExtra, isOrdered))
                {
                    return false;
                }
                var remainingLeftListToken = leftJArray.Where(x => !(x is JObject));
                JArray remainingLeftList = new JArray(remainingLeftListToken);
                var remainingRightListToken = rightJArray.Where(x => !(x is JObject));
                JArray remainingRightList = new JArray(remainingRightListToken);
                return DoesRightValContainsSameItems(remainingLeftList, remainingRightList, allowExtra, isOrdered);
            }
            else if (leftJArray.First is JObject && bothArrayContainsJObject)
            {
                if (!IsArrayOfJsonObjectsProperSubsetOf(leftJArray, rightJArray, true,
                    allowExtra, isOrdered))
                {
                    return false;
                }
            }
            else if (!IsListProperSubsetOf(leftJArray, rightJArray, allowExtra, isOrdered))
            {
                return false;
            }

            return true;
        }

        private static bool ListContainsJObject(JArray jArray)
        {
            bool containsJObject = false;
            foreach (var item in jArray)
            {
                if (item is JObject)
                {
                    containsJObject = true;
                    break;
                }
            }
            return containsJObject;
        }

        private static bool IsArrayOfJObject(JArray jArray)
        {
            bool listOfJObject = true;
            foreach (var item in jArray)
            {
                if (!(item is JObject))
                {
                    listOfJObject = false;
                    break;
                }
            }
            return listOfJObject;
        }
    }
}
