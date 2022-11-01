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
    private readonly Func<string?, object?> converter;

    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheParam"/> class.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="summary">The parameter summary.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="alias">The parameter alias, if applicable.</param>
    /// <param name="typeName">The parameter type name.</param>
    /// <param name="hidden">Whether the parameter is hidden.</param>
    /// <param name="hasDefaultValue">Whether a default value has been specified.</param>
    /// <param name="defaultValue">The default value.</param>
    public ComancheParam(
        string name,
        string? summary,
        Func<string?, object?> converter,
        string? alias,
        string typeName,
        bool hidden,
        bool hasDefaultValue,
        object? defaultValue)
    {
        this.Name = name;
        this.Summary = summary;
        this.converter = converter;
        this.Alias = alias;
        this.TypeName = typeName;
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
    /// Gets the parameter type name.
    /// </summary>
    public string TypeName { get; }

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

    /// <summary>
    /// Converts input to a value.
    /// </summary>
    /// <param name="input">The command line input.</param>
    /// <returns>A converted value.</returns>
    public object? Convert(string? input) => this.converter(input);
}
