// <copyright file="Discover.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;
using System.Linq;
using System.Reflection;
using Comanche.Extensions;
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
    /// <param name="args">Command arguments.</param>
    /// <param name="asm">An assembly.</param>
    /// <returns>The result of the invocation.</returns>
    public static object? Go(
        IServiceCollection? services = null,
        string[]? args = null,
        Assembly? asm = null)
    {
        asm ??= Assembly.GetEntryAssembly();
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();
        services ??= new ServiceCollection();
        services.AddSingleton(_ => BuildConfig());
        services.PlugSingleton<ComanchePalette>();
        services.PlugSingleton<IConsole, ConsoleWriter>();

        var provider = services.BuildServiceProvider();
        var console = provider.GetRequiredService<IConsole>();
        var session = asm.GetSession(provider);
        return session.Fulfil(args, console, provider);
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

    private static void PlugSingleton<T>(this IServiceCollection services)
        where T : class => services.PlugSingleton<T, T>();

    private static void PlugSingleton<T, TImpl>(this IServiceCollection services)
        where T : class
        where TImpl : class, T
    {
        if (!services.Any(s => s.ServiceType == typeof(T)))
        {
            services.AddSingleton<T, TImpl>();
        }
    }
}
