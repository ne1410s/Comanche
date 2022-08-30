using System.Reflection;
using System.Text.RegularExpressions;

namespace Comanche.Services;

/// <inheritdoc cref="IRouteBuilder"/>
public class RouteBuilder : IRouteBuilder
{
    /// <inheritdoc/>
    public Dictionary<string, MethodInfo> BuildRoutes(Assembly assembly)
    {
        var abstractTypes = assembly.GetExportedTypes().Where(t => t.IsAbstract);
        var sealedTypes = assembly.GetExportedTypes().Where(t => t.IsSealed);
        return abstractTypes
            .Where(t => sealedTypes.Contains(t))
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .ToDictionary(m => DerivePath(m), m => m);
    }

    private static string DerivePath(MemberInfo member, string? leaf = null)
    {
        var alias = member.GetCustomAttribute<AliasAttribute>()?.Name;
        var divider = string.IsNullOrEmpty(leaf) ? string.Empty : "|";
        var currentPath = SanitiseName(alias ?? member.Name) + divider + leaf;
        var parent = member.DeclaringType;
        return parent == null ? currentPath : DerivePath(parent, currentPath);
    }

    private static string SanitiseName(string name) =>
        Regex.Replace(name, "[^a-zA-Z0-9-_]+", "").ToLower();
}
