// <copyright file="DiscoveryExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
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
internal static class DiscoveryExtensions
{
    private const string XPathParameterMethodFormat = "./doc/members/member[starts-with(@name, '{0}(')]";
    private const string XPathMemberFormat = "./doc/members/member[@name='{0}']";
    private const string Space = " ";

    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
    private static readonly Regex TermRemovalRegex = new("[^a-zA-Z0-9-]+");
    private static readonly Regex HeadRemovalRegex = new("^[^a-zA-Z]+");
    private static readonly Regex TermRespaceRegex = new("\\s{2,}");
    private static readonly Regex DashPrependRegex = new("([^A-Z0-9])([A-Z])");
    private static readonly Regex ModuleElideRegex = new("[Mm]odule$");

    /// <summary>
    /// Obtains Comanche capability metadata.
    /// </summary>
    /// <param name="asm">The assembly.</param>
    /// <param name="moduleOptIn">If true, modules are only included if they
    /// possess a <see cref="ModuleAttribute"/>.</param>
    /// <returns>Comanche metadata.</returns>
    internal static ComancheSession GetSession(this Assembly asm, bool moduleOptIn)
    {
        var xDoc = asm.LoadXDoc();

        var topLevelModules = asm.ExportedTypes
            .Where(t => t.DeclaringType == null)
            .Select(t => t.ToModule(xDoc, moduleOptIn))
            .Where(m => m != null)
            .ToDictionary(m => m!.Name, m => m!);

        var cliVersion = asm.GetName().Version.ToString(3);
        var comancheVersion = Assembly.GetCallingAssembly().GetName().Version.ToString(3);

        return new(topLevelModules, cliVersion, comancheVersion);
    }

    private static ComancheModule? ToModule(this Type t, XDocument xDoc, bool moduleOptIn)
    {
        var moduleName = t.GetCustomAttribute<ModuleAttribute>()?.Name;
        if (!moduleOptIn && t.GetCustomAttribute<HiddenAttribute>() == null && moduleName == null)
        {
            moduleName = ModuleElideRegex.Replace(t.Name, string.Empty);
        }

        if (moduleName == null)
        {
            return null;
        }

        moduleName = moduleName.Sanitise();

        var xmlMemberName = t.FullName.Replace("+", ".", StringComparison.OrdinalIgnoreCase);
        var xPath = string.Format(Invariant, XPathMemberFormat, $"T:{xmlMemberName}");
        var xmlType = xDoc.XPathSelectElement(xPath);
        var xmlSummary = GetNodeText(xmlType, "summary");
        var isStatic = t.IsAbstract && t.IsSealed;
        var resolver = () => isStatic ? null : Activator.CreateInstance(t);

        var methods = t.GetMethods()
            .Where(m => m.DeclaringType != typeof(object) && m.GetCustomAttribute<HiddenAttribute>() == null)
            .Select(m => m.ToMethod(resolver, xDoc))
            .ToDictionary(m => m.Name, m => m);

        var subModules = t.GetNestedTypes()
            .Select(n => n.ToModule(xDoc, moduleOptIn))
            .Where(m => m != null)
            .ToDictionary(m => m!.Name, m => m!);

        if (methods.Count + subModules.Count == 0)
        {
            return null;
        }

        return new(moduleName, xmlSummary, methods, subModules);
    }

    private static ComancheMethod ToMethod(this MethodInfo m, Func<object?> resolver, XDocument xDoc)
    {
        var paramInfos = m.GetParameters();
        var xmlMemberName = m.DeclaringType.FullName.Replace("+", ".", StringComparison.OrdinalIgnoreCase);
        var xPathFormat = paramInfos.Length == 0 ? XPathMemberFormat : XPathParameterMethodFormat;
        var xPath = string.Format(Invariant, xPathFormat, $"M:{xmlMemberName}.{m.Name}");
        var xmlMethod = xDoc.XPathSelectElement(xPath);
        var xmlSummary = xmlMethod.GetNodeText("summary");
        var xmlReturns = xmlMethod.GetNodeText("returns");
        var methodName = (m.GetCustomAttribute<AliasAttribute>()?.Name ?? m.Name).Sanitise();

        var parameters = paramInfos
            .Select(p => p.ToParam(xmlMethod))
            .ToList();

        object? TaskCall(object? inst, object?[] parms)
        {
            if (typeof(Task).IsAssignableFrom(m.ReturnType))
            {
                var task = Task.Run(() => m.Invoke(inst, parms)).Result;
                return m.ReturnType.IsGenericType
                    ? task.GetType().GetProperty("Result").GetValue(task)
                    : null;
            }
            else
            {
                return m.Invoke(inst, parms);
            }
        }

        return new(methodName!, xmlSummary, xmlReturns, m.ReturnType, resolver, TaskCall, parameters);
    }

    private static ComancheParam ToParam(this ParameterInfo p, XElement? xmlMethod)
    {
        var xmlSummary = xmlMethod.GetNodeText($"param[@name='{p.Name}']");
        var alias = p.GetCustomAttribute<AliasAttribute>()?.Name?.Sanitise();
        var hidden = p.GetCustomAttribute<HiddenAttribute>() != null;
        var term = p.Name.Sanitise();

        return new(term!, xmlSummary, alias, p.ParameterType, hidden, p.HasDefaultValue, p.DefaultValue);
    }

    private static XDocument LoadXDoc(this Assembly asm)
    {
        // IL3000: Assembly.Location is empty in apps built for single-file
        // This approach appears to support single-file and regular builds
        var xmlPath = Path.Combine(AppContext.BaseDirectory, asm.GetName().Name + ".xml");
        return File.Exists(xmlPath) ? XDocument.Load(xmlPath) : new();
    }

    private static string? GetNodeText(this XElement? parent, string xPath)
    {
        var rawXmlValue = parent?.XPathSelectElement(xPath)?.Value;
        return !string.IsNullOrWhiteSpace(rawXmlValue)
            ? TermRespaceRegex.Replace(rawXmlValue, Space).Trim()
            : null;
    }

    private static string Sanitise(this string term)
    {
        term = TermRemovalRegex.Replace(term, string.Empty);
        term = HeadRemovalRegex.Replace(term, string.Empty);
        term = TermRespaceRegex.Replace(term, Space);
        term = DashPrependRegex.Replace(term, "$1-$2");
        term = term.ToLower(Invariant);

        return term.Trim();
    }
}
