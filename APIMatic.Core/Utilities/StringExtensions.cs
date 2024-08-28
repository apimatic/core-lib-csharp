// <copyright file="StringExtensions.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;

namespace APIMatic.Core.Utilities
{
    internal static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string source, string target)
        {
            return source.Equals(target, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
