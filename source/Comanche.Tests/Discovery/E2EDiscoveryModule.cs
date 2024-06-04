// <copyright file="E2EDiscoveryModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Discovery;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Comanche;
using Microsoft.Extensions.Configuration;

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

[Alias("ctors")]
public class E2EMultiCtorsModule : E2EDocumentedModule
{
    public E2EMultiCtorsModule()
        => throw new NotImplementedException();

    public E2EMultiCtorsModule(IConfiguration config)
    { }

    public E2EMultiCtorsModule(IConfiguration config, int notInjected)
        => throw new NotImplementedException();

    public sbyte Test() => -12;
}

[Alias("none")]
public sealed class E2ENoCtorsModule : E2EMultiCtorsModule
{
    internal E2ENoCtorsModule()
        : base(null!)
    { }

    public static int Do() => 2;
}