// <copyright file="ComancheParam.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.ModelsV2;

using System;
using System.Globalization;

/// <summary>
/// A modelled parameter.
/// </summary>
public class ComancheParam
{
    /// <summary>
    /// Initialises a new instance of the <see cref="ComancheParam"/> class.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="alias">The parameter alias, if applicable.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="defaultValueRaw">The raw default value.</param>
    public ComancheParam(string name, string? alias, Type type, object? defaultValueRaw)
    {
        this.Name = name;
        this.Alias = alias;
        this.Type = type;
        this.DefaultValue = defaultValueRaw;
    }

    /// <summary>
    /// Gets the parameter name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the parameter alias.
    /// </summary>
    public string? Alias { get; }

    /// <summary>
    /// Gets the parameter type.
    /// </summary>
    public Type Type { get; }

    /// <summary>
    /// Gets the default value.
    /// </summary>
    public object? DefaultValue { get; }

    /// <summary>
    /// Parses input to the parameter type.
    /// </summary>
    /// <param name="input">The text input.</param>
    /// <returns>The parsed value.</returns>
    public virtual object? ParseInput(string? input) => input == null ? null
        : Convert.ChangeType(input, this.Type, CultureInfo.InvariantCulture);
}
