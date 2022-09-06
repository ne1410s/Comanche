using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Comanche.Exceptions;
using Comanche.Models;

namespace Comanche.Services
{
    /// <inheritdoc cref="IParamValidator"/>
    public class ParamValidator : IParamValidator
    {
        /// <inheritdoc/>
        public object?[]? Validate(MethodRoute methodRoute)
        {
            var userArgs = methodRoute.ActualParams;
            var inputChecklist = userArgs.ToDictionary(args => args.Key, _ => false);
            var paramsList = new List<object?>();
            var badParamErrors = new List<string>();
            var argAliases = new Dictionary<string, string?>();

            foreach (var parameter in methodRoute.MethodInfo.GetParameters())
            {
                var paramAlias = SanitiseName(parameter.GetCustomAttribute<AliasAttribute>()?.Name);
                var paramType = parameter.ParameterType;
                var aliasText = paramAlias == null ? null : $" (-{paramAlias})";
                var paramLogName = $"--{parameter.Name}{aliasText}";
                argAliases[parameter.Name!] = paramAlias;
                var paramRef = paramAlias != null && userArgs.ContainsKey(paramAlias) ? paramAlias
                    : parameter.Name != null && userArgs.ContainsKey(parameter.Name) ? parameter.Name
                    : null;

                if (paramRef != null)
                {
                    inputChecklist[paramRef] = true;
                    var inputValue = userArgs[paramRef];
                    if (string.IsNullOrEmpty(inputValue) && paramType == typeof(bool))
                    {
                        inputValue = $"{true}";
                    }

                    if (TryConvert(inputValue, paramType, out var result, out var error))
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
                var exists = argAliases.ContainsKey(d.Key) || argAliases.ContainsValue(d.Key);
                var errorType = exists ? "is already supplied" : "is unrecognised";
                return $"'{d.Key}' {errorType}.";
            }));

            if (badParamErrors.Count > 0)
            {
                throw new ParamsException(badParamErrors);
            }

            return paramsList.Count > 0 ? paramsList.ToArray() : null;
        }

        /// <inheritdoc/>
        public bool TryConvert(string arg, Type type, out object? result, out string? error)
        {
            try
            {
                result = Convert.ChangeType(arg, type);
                error = null;
                return true;
            }
            catch (FormatException formatEx)
            {
                error = formatEx.Message;
                result = null;
                return false;
            }
        }

        private static string? SanitiseName(string? name) => name == null ? null
            : Regex.Replace(name, "[^a-zA-Z0-9-_]+", "").ToLower();
    }
}