// <copyright file="ParsingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using Comanche.Exceptions;
using Comanche.Models;

/// <summary>
/// Extenions relating to parameter parsing.
/// </summary>
internal static class ParsingExtensions
{
    /// <summary>
    /// Prepares a boxed array of parameters, ready to be fed into the method.
    /// </summary>
    /// <param name="methodParams">Method parameter list definition.</param>
    /// <param name="paramMap">A parameter map.</param>
    /// <returns>An array of parameters.</returns>
    /// <exception cref="ParamBuilderException">Param builder error.</exception>
    public static object?[] ParseMap(
        this IReadOnlyList<ComancheParam> methodParams,
        IReadOnlyDictionary<string, List<string>> paramMap)
    {
        var retVal = new List<object?>();
        var errors = new Dictionary<ComancheParam, string>();
        var unmatched = paramMap.Keys.ToList();

        foreach (var param in methodParams)
        {
            var byName = paramMap.TryGetValue("--" + param.Name, out List<string> inputs);
            var byAlias = param.Alias != null && !byName && paramMap.TryGetValue("-" + param.Alias, out inputs);

            if (!byName && !byAlias)
            {
                if (!param.HasDefault)
                {
                    errors[param] = "missing";
                }
                else
                {
                    retVal.Add(param.DefaultValue);
                }
            }
            else if (param.Hidden)
            {
                errors[param] = "unrecognised";
            }
            else if (inputs.Count == 0)
            {
                if (param.ParameterType == typeof(bool))
                {
                    retVal.Add(true);
                }
                else if (!param.ParameterType.IsValueType || Nullable.GetUnderlyingType(param.ParameterType) != null)
                {
                    retVal.Add(null);
                }
                else
                {
                    errors[param] = "missing";
                }
            }
            else if (param.ParameterType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(param.ParameterType)
                && (param.ParameterType.IsArray
                || (param.ParameterType.GenericTypeArguments.Length == 1
                    && param.ParameterType.GenericTypeArguments[0].GenericTypeArguments.Length == 0)))
            {
                if (param.ParameterType.IsArray)
                {
                    var type = param.ParameterType.GetElementType();
                    var res = inputs.Select(i => new { ok = i.TryParse(type, out var val, out var err), val, err });
                    var firstError = res.FirstOrDefault(r => !r.ok)?.err;
                    if (firstError == null)
                    {
                        var typedArray = Array.CreateInstance(type, inputs.Count);
                        for (var i = 0; i < inputs.Count; i++)
                        {
                            typedArray.SetValue(res.ElementAt(i).val, i);
                        }

                        retVal.Add(typedArray);
                    }
                    else
                    {
                        errors[param] = firstError;
                    }
                }
                else
                {
                    var type = param.ParameterType.GenericTypeArguments[0];
                    var res = inputs.Select(i => new { ok = i.TryParse(type, out var val, out var err), val, err });
                    var firstError = res.FirstOrDefault(r => !r.ok)?.err;
                    if (firstError == null)
                    {
                        var listType = typeof(List<>).MakeGenericType(type);
                        var typedList = (IList)Activator.CreateInstance(listType);
                        for (var i = 0; i < inputs.Count; i++)
                        {
                            typedList.Add(res.ElementAt(i).val);
                        }

                        retVal.Add(typedList);
                    }
                    else
                    {
                        errors[param] = firstError;
                    }
                }
            }
            else if (inputs.Count > 1)
            {
                errors[param] = "not array";
            }
            else if (inputs[0].TryParse(param.ParameterType, out var val, out var err))
            {
                retVal.Add(val);
            }
            else
            {
                errors[param] = err!;
            }

            var matchToRemove = byName ? "--" + param.Name : byAlias ? "-" + param.Alias : null;
            if (matchToRemove != null)
            {
                unmatched.Remove(matchToRemove);
            }
        }

        if (errors.Count != 0 || unmatched.Count != 0)
        {
            var errorMap = errors.ToDictionary(
                e => "--" + e.Key.Name + (e.Key.Alias == null ? string.Empty : " (-" + e.Key.Alias + ")"),
                e => e.Value);
            unmatched.ForEach(u => errorMap[u] = "unrecognised");
            throw new ParamBuilderException(errorMap);
        }

        return retVal.ToArray();
    }

    private static bool TryParse(this string input, Type targetType, out object? value, out string? error)
    {
        error = null;
        var nullableType = Nullable.GetUnderlyingType(targetType);
        if (targetType.IsPrimitive || targetType == typeof(string)
            || nullableType?.IsPrimitive == true || nullableType == typeof(string))
        {
            try
            {
                value = Convert.ChangeType(input, nullableType ?? targetType);
            }
            catch
            {
                value = null;
                error = "cannot convert";
            }
        }
        else
        {
            try
            {
                value = JsonSerializer.Deserialize(input!, targetType);
            }
            catch
            {
                value = null;
                error = "cannot deserialize";
            }
        }

        return error == null;
    }

    ////private static object GetDefault(Type t) => typeof(ParsingExtensions)
    ////    .GetMethod(nameof(GetDefaultGeneric), BindingFlags.NonPublic | BindingFlags.Static)
    ////    .MakeGenericMethod(t).Invoke(null, null);

    ////private static T? GetDefaultGeneric<T>() => default;
}
