using System.Reflection;

namespace Comanche.Models;

/// <summary>
/// A result for a method route operation.
/// </summary>
public class MethodRoute : RouteResult
{
    /// <summary>
    /// Gets the method info.
    /// </summary>
    public MethodInfo MethodInfo { get; }

    /// <summary>
    /// Gets a map of actual parameters.
    /// </summary>
    public Dictionary<string, string> ActualParams { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="MethodRoute"/> class.
    /// </summary>
    /// <param name="methodInfo">The method info.</param>
    /// <param name="actualParams">The actual parameters.</param>
    public MethodRoute(
        MethodInfo methodInfo,
        Dictionary<string, string> actualParams)
    {
        MethodInfo = methodInfo;
        ActualParams = actualParams;
    }
}
