// <copyright file="HiddenAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Attributes
{
    using System;

    /// <summary>
    /// Prevents a method or parameter from being exposed in routing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
    public class HiddenAttribute : Attribute
    { }
}
