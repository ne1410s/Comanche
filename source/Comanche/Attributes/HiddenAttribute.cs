// <copyright file="HiddenAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Attributes
{
    using System;

    /// <summary>
    /// Prevents a module, method or parameter from being exposed to Comanche.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
    public class HiddenAttribute : Attribute
    { }
}
