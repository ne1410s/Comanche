using Comanche.Models;

namespace Comanche.Services;

/// <summary>
/// Generates help.
/// </summary>
public interface IHelpGenerator
{
    /// <summary>
    /// Generate help from a route.
    /// </summary>
    /// <param name="helpRoute">The route.</param>
    /// <returns>Help text.</returns>
    public string GenerateHelp(HelpRoute helpRoute);
}
