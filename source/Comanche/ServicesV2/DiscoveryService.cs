// <copyright file="DiscoveryService.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.ServicesV2;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Comanche.AttributesV2;
using Comanche.ModelsV2;

/// <summary>
/// Discovers functionality exposed to Comanche.
/// </summary>
public class DiscoveryService
{
    private const string XPathParameterMethodFormat = "./doc/members/member[starts-with(@name, '{0}(')]";
    private const string XPathMemberFormat = "./doc/members/member[@name='{0}']";

    private static readonly IFormatProvider Invariant = CultureInfo.InvariantCulture;
    private static readonly IEqualityComparer<string> Ordinal = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Obtains Comanche capability metadata.
    /// </summary>
    /// <param name="asm">The assembly.</param>
    /// <returns>Comanche metadata.</returns>
    public Dictionary<string, ComancheModule> Discover(Assembly asm)
    {
        var xDoc = LoadXDoc(asm);
        return asm.ExportedTypes
            .Select(t => ToModule(t, xDoc))
            .Where(m => m != null)
            .ToDictionary(m => m!.Name, m => m!, Ordinal);
    }

    private static ComancheModule? ToModule(Type t, XDocument? xDoc)
    {
        var moduleName = t.GetCustomAttribute<ModuleAttribute>()?.Name;
        if (moduleName == null)
        {
            return null;
        }

        //TODO: Submodules??

        var xPath = string.Format(Invariant, XPathMemberFormat, $"T:{t.FullName}");
        var xmlMember = xDoc?.XPathSelectElement(xPath);
        var xmlSummary = GetNodeText(xmlMember, "summary");

        var isStatic = t.IsAbstract && t.IsSealed;
        var resolver = () => isStatic ? null : Activator.CreateInstance(t);
        return new(moduleName, xmlSummary, t.GetMethods()
            .Where(m => m.DeclaringType != typeof(object) && m.GetCustomAttribute<HiddenAttribute>() == null)
            .Select(m => ToMethod(m, resolver, xDoc))
            .ToDictionary(m => m.Name, m => m));
    }

    private static ComancheMethod ToMethod(MethodInfo m, Func<object?> resolver, XDocument? xDoc)
    {
        var paramInfos = m.GetParameters();
        var xmlMemberName = m.DeclaringType.FullName.Replace("+", ".");
        var xPathFormat = paramInfos.Length == 0 ? XPathMemberFormat : XPathParameterMethodFormat;
        var xPath = string.Format(Invariant, xPathFormat, $"M:{xmlMemberName}.{m.Name}");
        var xmlMember = xDoc?.XPathSelectElement(xPath);
        var xmlSummary = GetNodeText(xmlMember, "summary");

        var methodName = m.GetCustomAttribute<AliasAttribute>()?.Name ?? m.Name;
        return new(methodName, xmlSummary, resolver, m.Invoke, paramInfos
            .Select(p => ToParam(p, xmlMember))
            .ToList());
    }

    private static ComancheParam ToParam(ParameterInfo p, XElement? xmlMethod)
    {
        //TODO: Read xml!!

        var alias = p.GetCustomAttribute<AliasAttribute>()?.Name;
        var hidden = p.GetCustomAttribute<HiddenAttribute>() != null;
        var typeName = p.ParameterType.Name;
        var converter = (string? input) => input == null && p.HasDefaultValue
            ? p.DefaultValue
            : Convert.ChangeType(input, p.ParameterType, Invariant);
        return new ComancheParam(p.Name, converter, alias, typeName, hidden, p.HasDefaultValue, p.DefaultValue);
    }

    private static XDocument? LoadXDoc(Assembly asm)
    {
        // IL3000: Assembly.Location is empty in apps built for single-file
        // This approach appears to support single-file and regular builds
        var xmlPath = Path.Combine(AppContext.BaseDirectory, asm.GetName().Name + ".xml");
        return File.Exists(xmlPath) ? XDocument.Load(xmlPath) : null;
    }

    private static string? GetNodeText(XElement? parent, string prop)
    {
        var rawXmlValue = parent?.Element(prop)?.Value;
        return !string.IsNullOrWhiteSpace(rawXmlValue)
            ? Regex.Replace(rawXmlValue, "\\s{2,}", " ").Trim()
            : null;
    }
}
