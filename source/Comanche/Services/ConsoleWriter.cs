namespace Comanche.Services;

/// <inheritdoc cref="IOutputWriter"/>
public class ConsoleWriter : IOutputWriter
{
    private readonly Dictionary<ConsoleColor, List<string>> log = new()
    {
        { ConsoleColor.Red, new() },
        { ConsoleColor.White, new() },
    };

    /// <summary>
    /// Gets a list of standard entries, most recent first.
    /// </summary>
    public IReadOnlyList<string> Entries => log[ConsoleColor.White];

    /// <summary>
    /// Gets a list of error entries, most recent first.
    /// </summary>
    public IReadOnlyList<string> ErrorEntries => log[ConsoleColor.Red];

    /// <inheritdoc/>
    public void WriteLine(string text, bool error = false) =>
        WriteLineInternal(text, error);

    private void WriteLineInternal(string text, bool error)
    {
        var priorForeground = Console.ForegroundColor;
        var foreground = error ? ConsoleColor.Red : ConsoleColor.White;
        Console.ForegroundColor = foreground;
        if (error)
        {
            Console.Error.WriteLine(text);
        }
        else
        {
            Console.WriteLine(text);
        }

        log[foreground].Insert(0, text);
        Console.ForegroundColor = priorForeground;
    }
}
