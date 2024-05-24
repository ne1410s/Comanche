// <copyright file="E2EExecutionModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Execution;

using System;
using Microsoft.Extensions.Configuration;
using E2E = TestHelper;

[Alias("exec")]
public class E2EExecutionModule(IConfiguration config) : IModule
{
    public static void Throw() => throw new ArithmeticException();

    public static void ThrowStackless() => throw new E2E.StacklessException();

    public string? GetVar() => config["ConfigName"];
}
