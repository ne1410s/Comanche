// <copyright file="DiscoveryService.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.ServicesV2;

using System;
using System.Collections.Generic;
using System.Reflection;
using Comanche.AttributesV2;
using Comanche.ModelsV2;

/// <summary>
/// Discovers functionality exposed to Comanche.
/// </summary>
public class DiscoveryService
{
    /// <summary>
    /// Discovers public modules in an assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>A list of modules.</returns>
    public Dictionary<string, ComancheModule> Discover(Assembly assembly)
    {
        var discoModules = new Dictionary<string, ComancheModule>(StringComparer.OrdinalIgnoreCase);
        foreach (var exported in assembly.GetExportedTypes())
        {
            var moduleAttrib = exported.GetCustomAttribute<ModuleAttribute>();
            if (moduleAttrib != null)
            {
                var module = new ComancheModule(moduleAttrib.Name, exported, new(StringComparer.OrdinalIgnoreCase));
                foreach (var methodInfo in exported.GetMethods())
                {
                    var showMethod = methodInfo.GetCustomAttribute<HiddenAttribute>() == null;
                    if (showMethod)
                    {
                        var methodParams = new List<ComancheParam>();
                        var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                        foreach (var param in methodInfo.GetParameters())
                        {
                            var showParam = param.GetCustomAttribute<HiddenAttribute>() == null;
                            if (!showParam && !param.HasDefaultValue)
                            {
                                throw new ArgumentException(
                                    $"Parameter '{param.Name}' is hidden but no default value is specified");
                            }

                            var paramAlias = param.GetCustomAttribute<AliasAttribute>()?.Name;

                            if (paramAlias != null)
                            {
                                if (aliases.ContainsKey(paramAlias))
                                {
                                    throw new ArgumentException(
                                        $"Parameter '{param.Name}' alias, '{paramAlias}' is already used " +
                                            $"for '{aliases[paramAlias]}'");
                                }

                                aliases[paramAlias] = param.Name;
                            }

                            methodParams.Add(
                                new ComancheParam(param.Name, paramAlias, param.ParameterType, param.DefaultValue));
                        }

                        var methodName = methodInfo.GetCustomAttribute<AliasAttribute>()?.Name ?? methodInfo.Name;
                        if (module.Methods.ContainsKey(methodName))
                        {
                            throw new ArgumentException(
                                $"Module '{module.Name}' / Method '{methodName}' is already specified.");
                        }

                        module.Methods[methodName] = new ComancheMethod(
                            methodName, module, methodParams, (t, p) => methodInfo.Invoke(t, p));
                    }
                }

                if (module.Methods.Count > 0)
                {
                    if (discoModules.ContainsKey(module.Name))
                    {
                        throw new ArgumentException($"Module '{module.Name}' is already specified.");
                    }

                    discoModules[module.Name] = module;
                }
            }
        }

        return discoModules;
    }
}
