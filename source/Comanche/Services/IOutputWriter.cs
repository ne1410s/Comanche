namespace Comanche.Services;

/// <summary>
/// Writes output.
/// </summary>
public interface IOutputWriter
{
    /// <summary>
    /// Outputs a line of text.
    /// </summary>
    /// <param name="text">The line of text.</param>
    /// <param name="error">True if the text represents an error.</param>
    public void WriteLine(string text, bool error = false);
}
