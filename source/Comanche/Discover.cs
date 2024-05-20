// <copyright file="Discover.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;
using System.Linq;
using System.Reflection;
using Comanche.Attributes;
using Comanche.Extensions;
using Comanche.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Discover Comanche.
/// </summary>
public static class Discover
{
    /// <summary>
    /// The Comanche environment key.
    /// </summary>
    public const string EnvironmentKey = "COMANCHE_ENVIRONMENT";

    private const string DefaultEnv = "Development";

    /// <summary>
    /// Invokes Comanche.
    /// </summary>
    /// <param name="moduleOptIn">If true, modules are only included if they
    /// possess a <see cref="ModuleAttribute"/>.</param>
    /// <param name="asm">An assembly.</param>
    /// <param name="args">Command arguments.</param>
    /// <param name="writer">An output writer.</param>
    /// <param name="services">The services.</param>
    /// <returns>The result of the invocation.</returns>
    public static object? Go(
        bool moduleOptIn = false,
        Assembly? asm = null,
        string[]? args = null,
        IConsole? writer = null,
        IServiceCollection? services = null)
    {
        asm ??= Assembly.GetEntryAssembly();
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        writer ??= new ConsoleWriter();
        services ??= new ServiceCollection();
        services.AddTransient(_ => writer);
        services.AddTransient(_ => BuildConfig());

        var provider = services.BuildServiceProvider();
        var session = asm.GetSession(moduleOptIn, provider);
        return session.Fulfil(args, writer, provider);
    }

    private static IConfiguration BuildConfig()
    {
        var environmentName = Environment.GetEnvironmentVariable(EnvironmentKey) ?? DefaultEnv;
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .AddEnvironmentVariables()
            .Build();
    }
}
