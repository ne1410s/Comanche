// <copyright file="Discover.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;
using System.Linq;
using System.Reflection;
using Comanche.Extensions;
using Comanche.Models;
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
    /// <param name="services">The services.</param>
    /// <param name="palette">The colour palette.</param>
    /// <param name="args">Command arguments.</param>
    /// <param name="asm">An assembly.</param>
    /// <returns>The result of the invocation.</returns>
    public static object? Go(
        IServiceCollection? services = null,
        ComanchePalette? palette = null,
        string[]? args = null,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetEntryAssembly();
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        services ??= new ServiceCollection();
        services.AddSingleton(_ => BuildConfig());
        services.AddSingleton(_ => palette ?? new());
        services.AddSingleton(sp =>
        {
            var palette = sp.GetRequiredService<ComanchePalette>();
            return sp.GetService<IConsole>() ?? new ConsoleWriter(palette);
        });

        var provider = services.BuildServiceProvider();
        var writer = provider.GetRequiredService<IConsole>();
        var session = asm.GetSession(provider);
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
