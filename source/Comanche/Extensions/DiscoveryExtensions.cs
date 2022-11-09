// <copyright file="DiscoveryExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Comanche.Attributes;
using Comanche.Models;

/// <summary>
/// Extensions relating to Comanche discovery.
/// </summary>
public static class DiscoveryExtensions
{
    private const string XPathParameterMethodFormat = "./doc/members/member[starts-with(@name, '{0}(')]";
    private const string XPathMemberFormat = "./doc/members/member[@name='{0}']";
    private const string Space = " ";

    private static readonly IFormatProvider Invariant = CultureInfo.InvariantCulture;
    private static readonly Regex TermRemovalRegex = new("[^a-zA-Z0-9-_]+");
    private static readonly Regex HeadRemovalRegex = new("^[^a-zA-Z]+");
    private static readonly Regex TermRespaceRegex = new("\\s{2,}");

    /// <summary>
    /// Obtains Comanche capability metadata.
    /// </summary>
    /// <param name="asm">The assembly.</param>
    /// <returns>Comanche metadata.</returns>
    public static ComancheSession Discover(this Assembly asm)
    {
        var xDoc = asm.LoadXDoc();

        var topLevelModules = asm.ExportedTypes
            .Where(t => t.DeclaringType == null)
            .Select(t => t.ToModule(xDoc))
            .Where(m => m != null)
            .ToDictionary(m => m!.Name, m => m!);

        return new(topLevelModules);
    }

    private static ComancheModule? ToModule(this Type t, XDocument? xDoc)
    {
        var moduleName = t.GetCustomAttribute<ModuleAttribute>()?.Name?.Sanitise();
        if (moduleName == null)
        {
            return null;
        }

        var xPath = string.Format(Invariant, XPathMemberFormat, $"T:{t.FullName}");
        var xmlType = xDoc?.XPathSelectElement(xPath);
        var xmlSummary = GetNodeText(xmlType, "summary");
        var isStatic = t.IsAbstract && t.IsSealed;
        var resolver = () => isStatic ? null : Activator.CreateInstance(t);

        var methods = t.GetMethods()
            .Where(m => m.DeclaringType != typeof(object) && m.GetCustomAttribute<HiddenAttribute>() == null)
            .Select(m => m.ToMethod(resolver, xDoc))
            .ToDictionary(m => m.Name, m => m);

        var subModules = t.GetNestedTypes()
            .Select(n => n.ToModule(xDoc))
            .Where(m => m != null)
            .ToDictionary(m => m!.Name, m => m!);

        return new(moduleName, xmlSummary, methods, subModules);
    }

    private static ComancheMethod ToMethod(this MethodInfo m, Func<object?> resolver, XDocument? xDoc)
    {
        var paramInfos = m.GetParameters();
        var xmlMemberName = m.DeclaringType.FullName.Replace("+", ".", StringComparison.OrdinalIgnoreCase);
        var xPathFormat = paramInfos.Length == 0 ? XPathMemberFormat : XPathParameterMethodFormat;
        var xPath = string.Format(Invariant, xPathFormat, $"M:{xmlMemberName}.{m.Name}");
        var xmlMethod = xDoc?.XPathSelectElement(xPath);
        var xmlSummary = xmlMethod.GetNodeText("summary");
        var methodName = (m.GetCustomAttribute<AliasAttribute>()?.Name ?? m.Name).Sanitise();

        var parameters = paramInfos
            .Select(p => p.ToParam(xmlMethod))
            .ToList();

        Func<object?, object?[], Task<object?>> taskCall = async (inst, parms) =>
        {
            var result = m.Invoke(inst, parms);
            if (result is Task t)
            {
                await t;
                return Task.FromResult((object?)null);
            }
            else if (m.ReturnType == typeof(Task<>))
            {
                throw new NotImplementedException();
            }
            else
            {
                return Task.FromResult(result);
            }
        };

        return new(methodName!, xmlSummary, resolver, taskCall, parameters);
    }

    private static ComancheParam ToParam(this ParameterInfo p, XElement? xmlMethod)
    {
        var xmlSummary = xmlMethod.GetNodeText($"param[@name='{p.Name}']");
        var alias = p.GetCustomAttribute<AliasAttribute>()?.Name?.Sanitise();
        var hidden = p.GetCustomAttribute<HiddenAttribute>() != null;
        var typeName = p.ParameterType.Name;
        var term = p.Name.Sanitise();
        var converter = (string? input) => input == null && p.HasDefaultValue
            ? p.DefaultValue
            : Convert.ChangeType(input, p.ParameterType, Invariant);

        return new(term!, xmlSummary, converter, alias, typeName, hidden, p.HasDefaultValue, p.DefaultValue);
    }

    private static XDocument? LoadXDoc(this Assembly asm)
    {
        // IL3000: Assembly.Location is empty in apps built for single-file
        // This approach appears to support single-file and regular builds
        var xmlPath = Path.Combine(AppContext.BaseDirectory, asm.GetName().Name + ".xml");
        return File.Exists(xmlPath) ? XDocument.Load(xmlPath) : null;
    }

    private static string? GetNodeText(this XElement? parent, string xPath)
    {
        var rawXmlValue = parent?.XPathSelectElement(xPath)?.Value;
        return !string.IsNullOrWhiteSpace(rawXmlValue)
            ? TermRespaceRegex.Replace(rawXmlValue, Space).Trim()
            : null;
    }

    private static string? Sanitise(this string? term)
    {
        if (term != null)
        {
            term = TermRemovalRegex.Replace(term, string.Empty);
            term = HeadRemovalRegex.Replace(term, string.Empty);
            term = TermRespaceRegex.Replace(term, Space);
        }

        return term?.Trim();
    }
}
