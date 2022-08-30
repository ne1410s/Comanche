using Comanche.Models;

namespace Comanche.Services;

/// <summary>
/// Validates parameters.
/// </summary>
public interface IParamValidator
{
    /// <summary>
    /// Validates a method route.
    /// </summary>
    /// <param name="methodRoute">A method route.</param>
    /// <returns>An ordered set of typed (boxed) parameters.</returns>
    public object?[]? Validate(MethodRoute methodRoute);

    /// <summary>
    /// Tries to convert a string argument to the supplied type.
    /// </summary>
    /// <param name="arg">The parameter as text.</param>
    /// <param name="type">The parameter type.</param>
    /// <param name="result">Any result.</param>
    /// <param name="error">Any error.</param>
    /// <returns>Whether conversion succeeded.</returns>
    public bool TryConvert(string arg, Type type, out object? result, out string? error);
}
