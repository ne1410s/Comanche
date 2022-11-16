// <copyright file="ParsingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
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
            else if (param.ParameterType.IsAssignableFrom(typeof(IEnumerable<>)))
            {
                var genArgs = param.ParameterType.GenericTypeArguments;
                var genType = param.ParameterType.GetGenericTypeDefinition();
                var iterVal = inputs.Select(i => i.Parse(genType));

                // TODO: Check!!
                retVal.Add(iterVal);
            }
            else if (inputs.Count > 1)
            {
                errors[param] = "not array";
            }
            else
            {
                retVal.Add(inputs[0].Parse(param.ParameterType));
            }
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
