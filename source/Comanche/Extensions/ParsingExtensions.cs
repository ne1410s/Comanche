﻿// <copyright file="ParsingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Comanche.Exceptions;
using Comanche.Models;

/// <summary>
/// Extenions relating to parameter parsing.
/// </summary>
public static class ParsingExtensions
{
    /// <summary>
    /// Prepares a boxed array of parameters, ready to be fed into the method.
    /// </summary>
    /// <param name="methodParams">Method parameter list definition.</param>
    /// <param name="paramMap">A parameter map.</param>
    /// <returns>An array of parameters.</returns>
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
            var byAlias = param.Alias != null && paramMap.TryGetValue("-" + param.Alias, out inputs);

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
            else if (byName && byAlias)
            {
                errors[param] = "duplicate";
            }
            else if (param.Hidden)
            {
                errors[param] = "unrecognised";
            }
            else if (typeof(IEnumerable).IsAssignableFrom(param.ParameterType))
            {
                if (param.ParameterType.IsArray && !typeof(IEnumerable).IsAssignableFrom(
                    param.ParameterType.GetElementType()))
                {
                    var itemType = param.ParameterType.GetElementType();
                    var iterVal = inputs.Select(i => i.Parse(itemType));
                    retVal.Add(iterVal.ToArray());
                }
                else if (param.ParameterType.GenericTypeArguments.Length == 1)
                {
                    var itemType = param.ParameterType.GenericTypeArguments[0];
                    var iterVal = inputs.Select(i => i.Parse(itemType));
                    retVal.Add(iterVal.ToList());
                }
                else
                {
                    errors[param] = "unsupported";
                }
            }
            else if (param.ParameterType.IsArray && param.ParameterType.GetArrayRank() == 1)
            {
                // TODO!
            }
            else if (inputs.Count > 1)
            {
                errors[param] = "not array";
            }
            else
            {
                retVal.Add(inputs[0].Parse(param.ParameterType));
            }

            unmatched.Remove(byName ? "--" + param.Name : string.Empty);
            unmatched.Remove(byAlias ? "-" + param.Alias : string.Empty);
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

    private static object? Parse(this string input, Type target)
    {
        return target switch
        {
            _ => Convert.ChangeType(input, target)
        };
    }
}
