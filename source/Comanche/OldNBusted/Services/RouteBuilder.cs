// <copyright file="RouteBuilder.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Comanche.Attributes;

    /// <inheritdoc cref="IRouteBuilder"/>
    public class RouteBuilder : IRouteBuilder
    {
        /// <inheritdoc/>
        public Dictionary<string, MethodInfo> BuildRoutes(Assembly assembly)
        {
            // TODO: Remove static/abstract/sealed constraints and add constraint to require [Module] attrib

            var abstractTypes = assembly.GetExportedTypes().Where(t => t.IsAbstract);
            var sealedTypes = assembly.GetExportedTypes().Where(t => t.IsSealed);
            return abstractTypes
                .Where(t => sealedTypes.Contains(t))

                // TODO: Remove static constraint and add constraint to not have [Hidden] attrib

                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
                .ToDictionary(m => DerivePath(m), m => m);
        }

        private static string DerivePath(MemberInfo member, string? leaf = null)
        {
            string? alias = member.GetCustomAttribute<AliasAttribute>()?.Name;
            string divider = string.IsNullOrEmpty(leaf) ? string.Empty : "|";
            string currentPath = SanitiseName(alias ?? member.Name) + divider + leaf;
            System.Type parent = member.DeclaringType;
            return parent == null ? currentPath : DerivePath(parent, currentPath);
        }

        private static string SanitiseName(string name) =>
            Regex.Replace(name, "[^a-zA-Z0-9-_]+", string.Empty)
                .ToLower(CultureInfo.InvariantCulture);
    }
}