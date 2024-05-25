// <copyright file="TestHelper.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;

internal static class TestHelper
{
    public static readonly ComanchePalette DefaultPalette = new();
    public static readonly Regex WhiteSpace = new(@"\s+");

    public static object? Run(
        string? command = null,
        IConsole? console = null,
        ServiceCollection? services = null,
        Assembly? asm = null,
        ComanchePalette? palette = null)
    {
        palette ??= new();
        console ??= palette.GetMockConsole().Object;
        asm ??= Assembly.GetAssembly(typeof(TestHelper));
        services ??= new ServiceCollection();
        services.AddSingleton(console);
        services.AddSingleton(palette);

        return Discover.Go(services, command?.Split(' '), asm);
    }

    public static Mock<IConsole> GetMockConsole(
        this ComanchePalette palette)
    {
        var mockConsole = new Mock<IConsole>();
        mockConsole.Setup(m => m.Palette).Returns(palette);
        return mockConsole;
    }

    public static string Normalise(this string s, bool allSpaces) => allSpaces
        ? WhiteSpace.Replace(s, " ").Trim()
        : s.Replace("\r\n", "\n", StringComparison.Ordinal);

    public sealed class StacklessException : Exception
    {
        public override string? StackTrace => null;
    }

    public sealed class InfolessAssembly : Assembly
    {
        private readonly Assembly inner = GetAssembly(typeof(TestHelper))!;

        public override Type[] GetExportedTypes() => this.inner.GetExportedTypes();

        public override AssemblyName GetName() => this.inner.GetName();

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
            => attributeType == typeof(AssemblyInformationalVersionAttribute)
                ? Array.Empty<Attribute>()
                : this.inner.GetCustomAttributes(attributeType, inherit);
    }
}

public struct TestStruct
{ }