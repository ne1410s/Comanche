// <copyright file="ComancheParam.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models;

using System;

/// <summary>
/// A modelled parameter.
/// </summary>
public class ComancheParam
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ComancheParam"/> class.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="summary">The parameter summary.</param>
    /// <param name="alias">The parameter alias, if applicable.</param>
    /// <param name="parameterType">The parameter type.</param>
    /// <param name="hidden">Whether the parameter is hidden.</param>
    /// <param name="hasDefaultValue">Whether a default value has been specified.</param>
    /// <param name="defaultValue">The default value.</param>
    public ComancheParam(
        string name,
        string? summary,
        string? alias,
        Type parameterType,
        bool hidden,
        bool hasDefaultValue,
        object? defaultValue)
    {
        this.Name = name;
        this.Summary = summary;
        this.Alias = alias;
        this.ParameterType = parameterType;
        this.Hidden = hidden;
        this.HasDefault = hasDefaultValue;
        this.DefaultValue = defaultValue;
    }

    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the parameter summary.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// Gets the parameter alias.
    /// </summary>
    public string? Alias { get; }

    /// <summary>
    /// Gets the parameter type.
    /// </summary>
    public Type ParameterType { get; }

    /// <summary>
    /// Gets a value indicating whether the value is hidden.
    /// </summary>
    public bool Hidden { get; }

    /// <summary>
    /// Gets a value indicating whether a default value is provided.
    /// </summary>
    public bool HasDefault { get; }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    public object? DefaultValue { get; }
}
