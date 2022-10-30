// <copyright file="HelpGenerator.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Comanche.AttributesV2;
    using Comanche.Models;

    /// <inheritdoc cref="IHelpGenerator"/>
    public class HelpGenerator : IHelpGenerator
    {
        private const string XPathParamMethodPrefixFormat
            = "./doc/members/member[starts-with(@name, '{0}(')]";

        private const string XPathParamlessMethodFormat = "./doc/members/member[@name='{0}']";

        private readonly XDocument? xDoc;

        /// <summary>
        /// Initialises a new instance of the <see cref="HelpGenerator"/> class.
        /// </summary>
        /// <param name="cliAssembly">The assembly.</param>
        public HelpGenerator(Assembly cliAssembly)
        {
            // IL3000: Assembly.Location is empty in apps built for single-file
            // This approach appears to support single-file and regular builds
            string xmlPath = Path.Combine(
                AppContext.BaseDirectory,
                cliAssembly.GetName().Name + ".xml");
            this.xDoc = File.Exists(xmlPath) ? XDocument.Load(xmlPath) : null;
        }

        /// <inheritdoc/>
        public string GenerateHelp(HelpRoute helpRoute)
        {
            if (helpRoute is ModuleHelp moduleHelp)
            {
                return $"The following commands are available:{Environment.NewLine}  > "
                    + $"{string.Join($"{Environment.NewLine}  > ", moduleHelp.Modules)}";
            }

            MethodInfo method = ((MethodHelp)helpRoute).Method;

            StringBuilder sb = new();
            sb.Append("method: ").AppendLine(GetNameWithAlias(method));

            ParameterInfo[] parameters = method.GetParameters();
            bool parameterless = parameters.Length == 0;

            string xPathSelector = GetXPathSelector(method, parameterless);
            XElement? xmlMember = this.xDoc?.XPathSelectElement(xPathSelector);
            if (xmlMember != null)
            {
                AppendLineIfFound(sb, xmlMember, "summary");
                AppendLineIfFound(sb, xmlMember, "remarks");
                AppendLineIfFound(sb, xmlMember, "returns");
            }

            if (!parameterless)
            {
                sb.AppendLine("parameters:");
                foreach (ParameterInfo? parameter in parameters)
                {
                    sb.Append("  name: ").AppendLine(parameter.Name);
                }
            }

            return sb.ToString();
        }

        private static string GetXPathSelector(MethodInfo mi, bool parameterless)
        {
            string entry = $"M:{mi.DeclaringType!.FullName}.{mi.Name}";
            return parameterless
                ? string.Format(CultureInfo.InvariantCulture, XPathParamlessMethodFormat, entry)
                : string.Format(CultureInfo.InvariantCulture, XPathParamMethodPrefixFormat, entry);
        }

        private static string GetNameWithAlias(MemberInfo info)
        {
            var alias = info.GetCustomAttribute<AliasAttribute>()?.Name;
            return string.IsNullOrWhiteSpace(alias) ? info.Name : $"{info.Name} ({alias})";
        }

        private static string? GetXml(XElement parent, string prop)
        {
            string? rawXmlValue = parent.Element(prop)?.Value;
            return !string.IsNullOrWhiteSpace(rawXmlValue)
                ? Regex.Replace(rawXmlValue, "\\s{2,}", " ").Trim()
                : null;
        }

        private static void AppendLineIfFound(StringBuilder sb, XElement parent, string selector)
        {
            string? xmlValue = GetXml(parent, selector);
            if (xmlValue != null)
            {
                sb.Append(selector).Append(": ").AppendLine(xmlValue);
            }
        }
    }
}