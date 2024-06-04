// <copyright file="E2EExecutionModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Execution;

using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using E2E = TestHelper;

[Alias("exec")]
public class E2EExecutionModule(IConfiguration config) : IModule
{
    public static void Throw() => throw new ArithmeticException();

    public static void ThrowStackless() => throw new E2E.StacklessException();

    public static ComplexObject WriteJson(int myInt) => new(myInt, $"'{myInt}'");

    public static JsonError JsonErr() => new() { MyBool = true, MyString = "hi" };

    public static async Task<int> GetAsync() => await Task.FromResult(43);

    public static async Task DoAsync() => await Task.CompletedTask;

    public string? GetVar() => config["ConfigName"];
}

[Alias("ctors")]
public class E2EMultiCtorsModule : E2EExecutionModule
{
    public E2EMultiCtorsModule()
        : base(null!) => throw new NotImplementedException();

    public E2EMultiCtorsModule(IConfiguration config)
        : base(config)
    { }

    public E2EMultiCtorsModule(IConfiguration config, int notInjected)
        : base(config) => throw new NotImplementedException();

    public sbyte Test() => -12;
}

public record ComplexObject(int MyInt, string MyString);

public class JsonError
{
    [JsonPropertyName("fail")]
    public string? MyString { get; set; }

    [JsonPropertyName("fail")]
    public bool MyBool { get; set; }
}
