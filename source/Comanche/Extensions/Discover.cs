// <copyright file="Discover.cs" company="ne1410s">
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
using Comanche.Services;

/// <summary>
/// Extensions relating to Comanche discovery.
/// </summary>
public static class Discover
{
    private const string XPathParameterMethodFormat = "./doc/members/member[starts-with(@name, '{0}(')]";
    private const string XPathMemberFormat = "./doc/members/member[@name='{0}']";
    private const string Space = " ";

    private static readonly IFormatProvider Invariant = CultureInfo.InvariantCulture;
    private static readonly Regex TermRemovalRegex = new("[^a-zA-Z0-9-_]+");
    private static readonly Regex HeadRemovalRegex = new("^[^a-zA-Z]+");
    private static readonly Regex TermRespaceRegex = new("\\s{2,}");
    private static readonly Regex ModuleElideRegex = new("module$");

    /// <summary>
    /// Invokes Comanche.
    /// </summary>
    /// <param name="moduleOptIn">If true, modules are only included if they
    /// possess a <see cref="ModuleAttribute"/>.</param>
    /// <param name="asm">An assembly.</param>
    /// <param name="args">Command arguments.</param>
    /// <param name="writer">An output writer.</param>
    /// <returns>The result of the invocation.</returns>
    public static async Task<object?> GoAsync(
        bool moduleOptIn = false,
        Assembly? asm = null,
        string[]? args = null,
        IOutputWriter? writer = null)
    {
        asm ??= Assembly.GetEntryAssembly();
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        writer ??= new ConsoleWriter();

        var session = asm.GetSession(moduleOptIn);
        return await session.FulfilAsync(args, writer);
    }

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

        return new(topLevelModules);
    }

    private static ComancheModule? ToModule(this Type t, XDocument? xDoc, bool moduleOptIn)
    {
        var moduleName = t.GetCustomAttribute<ModuleAttribute>()?.Name?.Sanitise();
        if (!moduleOptIn && t.GetCustomAttribute<HiddenAttribute>() != null && moduleName == null)
        {       
            moduleName = ModuleElideRegex.Replace(t.Name.ToLower().Sanitise(), string.Empty);
        }

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
            .Select(n => n.ToModule(xDoc, moduleOptIn))
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
        var methodName = (m.GetCustomAttribute<AliasAttribute>()?.Name ?? m.Name.ToLower()).Sanitise();

        var parameters = paramInfos
            .Select(p => p.ToParam(xmlMethod))
            .ToList();

        async Task<object?> TaskCall(object? inst, object?[] parms)
        {
            var result = m.Invoke(inst, parms);
            if (result is Task task)
            {
                await task.ConfigureAwait(false);
                return m.ReturnType.IsGenericType
                    ? ((dynamic)task).Result
                    : null;
            }
            else
            {
                return result;
            }
        }

        return new(methodName!, xmlSummary, resolver, TaskCall, parameters);
    }

    private static ComancheParam ToParam(this ParameterInfo p, XElement? xmlMethod)
    {
        var xmlSummary = xmlMethod.GetNodeText($"param[@name='{p.Name}']");
        var alias = p.GetCustomAttribute<AliasAttribute>()?.Name?.Sanitise();
        var hidden = p.GetCustomAttribute<HiddenAttribute>() != null;
        var term = p.Name.Sanitise();

        return new(term!, xmlSummary, alias, p.ParameterType, hidden, p.HasDefaultValue, p.DefaultValue);
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
