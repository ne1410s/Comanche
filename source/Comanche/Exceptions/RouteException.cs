namespace Comanche.Exceptions;

/// <summary>
/// Represents errors occuring during the processing of routes.
/// </summary>
public class RouteException : ComancheException
{
    /// <summary>
    /// Gets the requested route.
    /// </summary>
    public string? RequestedRoute { get; }

    /// <summary>
    /// Gets the closest route.
    /// </summary>
    public string? ClosestRoute { get; }

    /// <summary>
    /// Gets the closest route options.
    /// </summary>
    public HashSet<string> ClosestRouteOpts { get; }

    /// <summary>
    /// Initialises a new instance of the <see cref="RouteException"/> class.
    /// </summary>
    /// <param name="requestedRoutePsv">The requested route.</param>
    /// <param name="closestRoutePsv">The closest route.</param>
    /// <param name="closestRouteOpts">The closest route options.</param>
    public RouteException(
        string? requestedRoutePsv,
        string? closestRoutePsv,
        HashSet<string> closestRouteOpts)
            : base(GetMessage(closestRouteOpts))
    {
        RequestedRoute = requestedRoutePsv;
        ClosestRoute = closestRoutePsv;
        ClosestRouteOpts = closestRouteOpts;
    }

    private static string GetMessage(HashSet<string> fqOpts)
        => $"Command not recognised. Suggestions:{Environment.NewLine}  > "
            + string.Join($"{Environment.NewLine}  > ", fqOpts);
}