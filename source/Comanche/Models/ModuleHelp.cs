namespace Comanche.Models;

/// <summary>
/// A result for a module help route operation.
/// </summary>
public class ModuleHelp : HelpRoute
{
    /// <summary>
    /// Gets the modules.
    /// </summary>
    public HashSet<string> Modules { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="ModuleHelp"/> class.
    /// </summary>
    /// <param name="modules">The modules.</param>
    public ModuleHelp(HashSet<string> modules)
    {
        Modules = modules;
    }
}
