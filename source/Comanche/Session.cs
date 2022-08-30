using System.Reflection;
using Comanche.Exceptions;
using Comanche.Models;
using Comanche.Services;

namespace Comanche;

/// <summary>
/// A session.
/// </summary>
public static class Session
{
    /// <summary>
    /// Routes the supplied arguments within the assembly.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="assembly">The assembly.</param>
    /// <param name="outputWriter">The output writer.</param>
    /// <returns>The route result.</returns>
    public static object? Route(
        string[]? args = null,
        Assembly? assembly = null,
        IOutputWriter? outputWriter = null)
    {
        outputWriter ??= new ConsoleWriter();
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        try
        {
            assembly ??= Assembly.GetEntryAssembly()!;
            var routes = new RouteBuilder().BuildRoutes(assembly);
            var routeResult = new MethodRouter().LocateMethod(args, routes);
            if (routeResult is HelpRoute helpRoute)
            {
                var help = new HelpGenerator(assembly).GenerateHelp(helpRoute);
                outputWriter.WriteLine(help);
                return null;
            }
            else
            {
                var methodResult = (MethodRoute)routeResult;
                var parameters = new ParamValidator().Validate(methodResult);
                var outcome = methodResult.MethodInfo.Invoke(null, parameters);
                outputWriter.WriteLine($"{outcome}");
                return outcome;
            }
        }
        catch (ComancheException appEx)
        {
            outputWriter.WriteLine($"Error invoking command '{string.Join(' ', args)}'. {appEx.Message}", true);
            Environment.ExitCode = 1;
            return null;
        }
    }
}