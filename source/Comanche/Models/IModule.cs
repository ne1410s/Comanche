// <copyright file="IModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// All Comanche modules must inherit this type to be exposed.
/// </summary>
[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "By design")]
public interface IModule
{ }
