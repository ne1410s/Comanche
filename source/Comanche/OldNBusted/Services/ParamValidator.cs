// <copyright file="ParamValidator.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Comanche.Attributes;
    using Comanche.Exceptions;
    using Comanche.Models;

    /// <inheritdoc cref="IParamValidator"/>
    public class ParamValidator : IParamValidator
    {
        /// <inheritdoc/>
        public object?[]? Validate(MethodRoute methodRoute)
        {
            Dictionary<string, string> userArgs = methodRoute.ActualParams;
            Dictionary<string, bool> inputChecklist = userArgs.ToDictionary(args => args.Key, _ => false);
            List<object?> paramsList = new();
            List<string> badParamErrors = new();
            Dictionary<string, string?> argAliases = new();

            foreach (ParameterInfo? parameter in methodRoute.MethodInfo.GetParameters())
            {
                string? paramAlias = SanitiseName(parameter.GetCustomAttribute<AliasAttribute>()?.Name);
                Type paramType = parameter.ParameterType;
                string? aliasText = paramAlias == null ? null : $" (-{paramAlias})";
                string paramLogName = $"--{parameter.Name}{aliasText}";
                argAliases[parameter.Name!] = paramAlias;
                string? paramRef = paramAlias != null && userArgs.ContainsKey(paramAlias) ? paramAlias
                    : parameter.Name != null && userArgs.ContainsKey(parameter.Name) ? parameter.Name
                    : null;

                if (paramRef != null)
                {
                    inputChecklist[paramRef] = true;
                    string inputValue = userArgs[paramRef];
                    if (string.IsNullOrEmpty(inputValue) && paramType == typeof(bool))
                    {
                        inputValue = $"{true}";
                    }

                    if (this.TryConvert(inputValue, paramType, out object? result, out string? error))
                    {
                        paramsList.Add(result);
                    }
                    else
                    {
                        badParamErrors.Add($"{paramLogName} is invalid. {error}".Trim());
                    }
                }
                else if (parameter.HasDefaultValue)
                {
                    paramsList.Add(parameter.DefaultValue);
                }
                else
                {
                    badParamErrors.Add($"{paramLogName} is required");
                }
            }

            badParamErrors.AddRange(inputChecklist.Where(d => !d.Value).Select(d =>
            {
                bool exists = argAliases.ContainsKey(d.Key) || argAliases.ContainsValue(d.Key);
                string errorType = exists ? "is already supplied" : "is unrecognised";
                return $"'{d.Key}' {errorType}.";
            }));

            if (badParamErrors.Count > 0)
            {
                throw new ParamsException(badParamErrors);
            }

            return paramsList.Count > 0 ? paramsList.ToArray() : null;
        }

        /// <inheritdoc/>
        public bool TryConvert(string arg, Type type, out object? result, out string? errorMsg)
        {
            try
            {
                result = Convert.ChangeType(arg, type, CultureInfo.InvariantCulture);
                errorMsg = null;
                return true;
            }
            catch (FormatException formatEx)
            {
                errorMsg = formatEx.Message;
                result = null;
                return false;
            }
        }

        private static string? SanitiseName(string? name) => name == null ? null
            : Regex.Replace(name, "[^a-zA-Z0-9-_]+", string.Empty)
                .ToLower(CultureInfo.InvariantCulture);
    }
}