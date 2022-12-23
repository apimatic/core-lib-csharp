// <copyright file="JsogAttribute.cs" company="APIMatic">
// Copyright (c) APIMatic. All rights reserved.
// </copyright>
using System;

namespace APIMatic.Core.Utilities
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class JsogAttribute : Attribute { }
}
