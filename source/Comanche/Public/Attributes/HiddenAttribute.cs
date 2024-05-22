// <copyright file="HiddenAttribute.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;

/// <summary>
/// Prevents a module, method or parameter from being exposed to Comanche.
/// In the case of parameters, those hidden must be registered in the service collection.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter)]
public sealed class HiddenAttribute : Attribute
{ }
