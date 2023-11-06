// <copyright file="WriterExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Comanche.Models;

/// <summary>
/// Extensions relating to output writing.
/// </summary>
internal static class WriterExtensions
{
    private static readonly Regex GenericDashRegex = new("`.*");

    /// <summary>
    /// Gets a printable type name.
    /// </summary>
    /// <param name="type">The original type.</param>
    /// <returns>Printable name.</returns>
    internal static string ToPrintableName(this Type type)
    {
        return type switch
        {
            //_ when type == typeof(bool) => "bool",
            _ when type == typeof(int) => "int",
            _ when type.IsPrimitive => type.Name.ToLowerInvariant(),
            _ when type == typeof(string) => "string",
            _ when type == typeof(void) => "<void>",
            _ when type.IsArray
                => type.GetElementType().ToPrintableName() + "[]",
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                => type.GenericTypeArguments[0].ToPrintableName() + "?",
            _ when type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>)
                => type.GenericTypeArguments[0].ToPrintableName(),
            _ when type.IsGenericType => string.Format(
                "{0}<{1}>",
                GenericDashRegex.Replace(type.Name, string.Empty),
                string.Join(", ", type.GenericTypeArguments.Select(t => t.ToPrintableName()))),
            _ => type.Name,
        };
    }

    /// <summary>
    /// Gets a printable default value.
    /// </summary>
    /// <param name="param">The parameter.</param>
    /// <returns>Printable default value.</returns>
    internal static string? GetPrintableDefault(this ComancheParam param)
    {
        var type = param.ParameterType;
        var checkType = Nullable.GetUnderlyingType(type) != null ? type.GenericTypeArguments[0] : type;

        return param switch
        {
            _ when !param.HasDefault => null,
            _ when param.DefaultValue == null => "null",
            _ when checkType == typeof(string) => $"\"{param.DefaultValue}\"",
            _ when checkType.IsPrimitive => param.DefaultValue.ToString(),
            _ => null
        };
    }
}
