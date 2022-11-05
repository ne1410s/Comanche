﻿// <copyright file="Session.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.OldNBusted
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Comanche.Exceptions;
    using Comanche.Models;
    using Comanche.Services;

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
                RouteResult routeResult = new MethodRouter().LocateMethod(args, routes);
                if (routeResult is HelpRoute helpRoute)
                {
                    string help = new HelpGenerator(assembly).GenerateHelp(helpRoute);
                    outputWriter.WriteLine(help);
                    return null;
                }
                else
                {
                    MethodRoute methodResult = (MethodRoute)routeResult;
                    object?[]? parameters = new ParamValidator().Validate(methodResult);
                    object outcome = methodResult.MethodInfo.Invoke(null, parameters);
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
}