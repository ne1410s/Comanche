// <copyright file="Discover.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Comanche.Attributes;
using Comanche.Extensions;
using Comanche.Services;

/// <summary>
/// Discover Comanche.
/// </summary>
public static class Discover
{
    /// <summary>
    /// Invokes Comanche.
    /// </summary>
    /// <param name="moduleOptIn">If true, modules are only included if they
    /// possess a <see cref="ModuleAttribute"/>.</param>
    /// <param name="asm">An assembly.</param>
    /// <param name="args">Command arguments.</param>
    /// <param name="writer">An output writer.</param>
    /// <returns>The result of the invocation.</returns>
    public static async Task<object?> GoAsync(
        bool moduleOptIn = false,
        Assembly? asm = null,
        string[]? args = null,
        IOutputWriter? writer = null)
    {
        asm ??= Assembly.GetEntryAssembly();
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        writer ??= new ConsoleWriter();

        var session = asm.GetSession(moduleOptIn);
        return await session.FulfilAsync(args, writer);
    }
}
