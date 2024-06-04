// <copyright file="E2EDiscoveryModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Discovery;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [Alias("9greet+")]
    public static string GetGreeting(
        [Alias("n")] string name,
        IDictionary<string, int>? dicto = null)
            => $"Hi, {name} #{dicto?.Count}!";

    public static async Task<string> UberDefaults(
        int myInt = 3,
        string? myStr = "hiya",
        DayOfWeek myDay = DayOfWeek.Friday,
        TestStruct myStruct = new(),
        int?[]? myArr = null)
    {
        await Task.CompletedTask;
        return $"{myInt}{myStr}{myDay}{myStruct}{myArr}";
    }
}

public class E2ENoAliasModule : E2EDocumentedModule
{
    public static bool Invert(bool b, long l) => !b;

    public static string Nullable(long? id = 4300) => $"{id}";

    /// <summary>
    /// Nested module.
    /// </summary>
    [Alias("nested")]
    public class E2ENestedModule : E2ENoAliasModule
    {
        /// <summary>
        /// JDI.
        /// </summary>
        public static void Do()
        {
            // It's empty
        }
    }
}

[Alias("empty")]
public class E2EEmptyModule : E2EDocumentedModule { }