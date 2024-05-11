// <copyright file="ComancheParam.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;

/// <summary>
/// A modelled parameter.
/// </summary>
/// <param name="name">The parameter name.</param>
/// <param name="summary">The parameter summary.</param>
/// <param name="alias">The parameter alias, if applicable.</param>
/// <param name="parameterType">The parameter type.</param>
/// <param name="hidden">Whether the parameter is hidden.</param>
/// <param name="hasDefaultValue">Whether a default value has been specified.</param>
/// <param name="defaultValue">The default value.</param>
internal sealed class ComancheParam(
    string name,
    string? summary,
    string? alias,
    Type parameterType,
    bool hidden,
    bool hasDefaultValue,
    object? defaultValue)
{
    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the parameter summary.
    /// </summary>
    public string? Summary { get; } = summary;

    /// <summary>
    /// Gets the parameter alias.
    /// </summary>
    public string? Alias { get; } = alias;

    /// <summary>
    /// Gets the parameter type.
    /// </summary>
    public Type ParameterType { get; } = parameterType;

    /// <summary>
    /// Gets a value indicating whether the parameter is hidden. In the case of parameters, this is
    /// necessary and sufficient for dependency injection.
    /// </summary>
    public bool Hidden { get; } = hidden;

    /// <summary>
    /// Gets a value indicating whether a default value is provided.
    /// </summary>
    public bool HasDefault { get; } = hasDefaultValue;

    /// <summary>
    /// Gets the default value.
    /// </summary>
    public object? DefaultValue { get; } = defaultValue;
}
