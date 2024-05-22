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
    /// <param name="provider">The service provider.</param>
    /// <returns>Comanche metadata.</returns>
    internal static ComancheSession GetSession(this Assembly asm, IServiceProvider provider)
    {
        var xDoc = asm.LoadXDoc();

        var rootModules = asm.ExportedTypes
            .Where(IsRootModule)
            .Select(t => t.ToModule(xDoc, asm, provider))
            .Where(m => m != null)
            .ToDictionary(m => m!.Name, m => m!);

        var asmName = asm.GetName();
        var version = asmName.Version.ToString(3);
        var description = asm.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;
        var comancheVersion = Assembly.GetCallingAssembly().GetName().Version.ToString(3);

        return new(rootModules, asmName.Name, version, description, comancheVersion);
    }

    private static ComancheModule? ToModule(this Type t, XDocument xDoc, Assembly asm, IServiceProvider provider)
    {
        var aliasName = t.GetCustomAttribute<AliasAttribute>(false)?.Name;
        var moduleName = ModuleElideRegex.Replace(aliasName ?? t.Name, string.Empty).Sanitise();
        var xmlMemberName = t.FullName.Replace("+", ".", StringComparison.OrdinalIgnoreCase);
        var xPath = string.Format(Invariant, XPathMemberFormat, $"T:{xmlMemberName}");
        var xmlType = xDoc.XPathSelectElement(xPath);
        var xmlSummary = GetNodeText(xmlType, "summary");

        // Use the first public ctor (by params length) where all dependencies are met
        var ctorProvisions = Array.Empty<object>();
        foreach (var ctor in t.GetConstructors().OrderByDescending(c => c.GetParameters().Length))
        {
            var allParams = ctor.GetParameters();
            var oks = allParams.Select(p => provider.GetService(p.ParameterType)).Where(r => r != null).ToArray();
            if (oks.Length == allParams.Length)
            {
                ctorProvisions = oks;
                break;
            }
        }

        object Resolver() => Activator.CreateInstance(t, ctorProvisions);
        var methods = t.GetMethods()
            .Where(m => m.DeclaringType == t && m.GetCustomAttribute<HiddenAttribute>() == null)
            .Select(m => m.ToMethod(Resolver, xDoc))
            .ToDictionary(m => m.Name, m => m);

        var subModules = asm.ExportedTypes
            .Where(et => et.IsModule() && et.FindParentModule() == t)
            .Select(n => n.ToModule(xDoc, asm, provider))
            .Where(m => m != null)
            .OrderBy(m => m!.Name)
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

    private static bool IsModule(this Type t)
        => typeof(IModule).IsAssignableFrom(t)
            && (t.IsPublic || t.IsNestedPublic)
            && !t.IsAbstract
            && t.GetCustomAttribute<HiddenAttribute>(false) == null;

    private static Type? FindParentModule(this Type t)
    {
        if (t.BaseType == null)
        {
            return null;
        }

        if (t.BaseType.IsModule())
        {
            return t.BaseType;
        }

        return FindParentModule(t.BaseType);
    }

    private static bool IsRootModule(this Type t) => t.IsModule() && t.FindParentModule() == null;
}
