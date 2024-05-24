// <copyright file="E2EDiscoveryModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Discovery;

using System;
using System.Collections.Generic;
using Comanche;

/// <summary>
/// Discovery module.
/// </summary>
[Alias("disco")]
public class E2EDiscoveryModule : IModule
{ }

/// <summary>
/// Tests doc gen.
/// </summary>
[Alias("dox")]
public class E2EDocumentedModule : E2EDiscoveryModule
{
    /// <summary>
    /// Send a greeting.
    /// </summary>
    /// <param name="name">The greetee.</param>
    /// <param name="dicto">The numbers dictionary.</param>
    /// <returns>A greeting.</returns>
    [Alias("greet")]
    public static string GetGreeting(
        [Alias("n")] string name,
        IDictionary<string, int>? dicto = null)
            => $"Hi, {name} #{dicto?.Count}!";

    public static void UberDefaults(
        int myInt = 3,
        string myStr = "hiya",
        DayOfWeek myDay = DayOfWeek.Friday)
    {
        // It's just empty, you pesky analysers ok
    }
}