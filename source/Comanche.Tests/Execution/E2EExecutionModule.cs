// <copyright file="E2EExecutionModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Execution;

using System;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using E2E = TestHelper;

[Alias("exec")]
public class E2EExecutionModule(IConfiguration config) : IModule
{
    public static void Throw() => throw new ArithmeticException();

    public static void ThrowStackless() => throw new E2E.StacklessException();

    public static ComplexObject WriteJson(int myInt) => new(myInt, $"'{myInt}'");

    public static JsonError JsonErr() => new() { MyBool = true, MyString = "hi" };

    public string? GetVar() => config["ConfigName"];
}

public record ComplexObject(int MyInt, string MyString);

public class JsonError
{
    [JsonPropertyName("fail")]
    public string? MyString { get; set; }

    [JsonPropertyName("fail")]
    public bool MyBool { get; set; }
}
