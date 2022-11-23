// <copyright file="ExecutionExtensionsTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Extensions;

using System;
using System.Threading.Tasks;
using Comanche.Exceptions;
using Comanche.Models;

/// <summary>
/// Tests ExecutionExtensions.
/// </summary>
public class ExecutionExtensionsTests
{
    private static readonly Type Void = typeof(void);

    [Theory]
    [InlineData(2)]
    [InlineData("")]
    [InlineData(false)]
    [InlineData(-10.55)]
    public async Task Execute_Inline_ReturnsExpected(object? expected)
    {
        // Arrange
        var sut = new ComancheMethod("m1", null, null, Void, () => null, (_, _) => Task.FromResult(expected), new());

        // Act
        var result = await sut.CallAsync(Array.Empty<object?>());

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task Execute_DerivedStatic_ReturnsExpected()
    {
        // Arrange
        var info = typeof(SampleClass).GetMethod(nameof(SampleClass.ParseBool));
        var taskCall = (object? _, object?[] p) => Task.FromResult(info!.Invoke(null, p));
        var sut = new ComancheMethod("m1", null, null, Void, () => null, taskCall, new());

        // Act
        var result = await sut.CallAsync(new object?[] { "true" });

        // Assert
        result.Should().Be(true);
    }

    [Fact]
    public async Task Execute_DerivedInstance_ReturnsExpected()
    {
        // Arrange
        var info = typeof(SampleClass).GetMethod(nameof(SampleClass.Add));
        var taskCall = (object? inst, object?[] p) => Task.FromResult(info!.Invoke(inst, p));
        var sut = new ComancheMethod("m1", null, null, Void, () => new SampleClass { Seed = 12 }, taskCall, new());

        // Act
        var result = await sut.CallAsync(new object?[] { 4 });

        // Assert
        result.Should().Be(16);
    }

    [Fact]
    public async Task Execute_DerivedAsync_ReturnsExpected()
    {
        // Arrange
        var info = typeof(SampleClass).GetMethod(nameof(SampleClass.GetAsync));
        async Task<object?> TaskCall(object? inst, object?[] p)
        {
            var task = (Task)info!.Invoke(inst, p)!;
            await task.ConfigureAwait(false);
            return (object?)((dynamic)task).Result;
        }

        var sut = new ComancheMethod("m1", null, null, Void, () => new SampleClass { Seed = 2 }, TaskCall, new());

        // Act
        var result = await sut.CallAsync(Array.Empty<object?>());

        // Assert
        result.Should().Be("Seed: 2");
    }

    [Fact]
    public async Task Execute_CausingError_ThrowsExpected()
    {
        // Arrange
        var info = typeof(SampleClass).GetMethod(nameof(SampleClass.ParseBool));
        var taskCall = (object? _, object?[] p) => Task.FromResult(info!.Invoke(null, p));
        var sut = new ComancheMethod("m1", null, null, Void, () => null, taskCall, new());

        // Act
        var act = () => sut.CallAsync(new object?[] { "lol-bool" });

        // Assert
        await act.Should().ThrowAsync<ExecutionException>()
            .WithMessage("Unexpected error calling method 'm1'");
    }

    private class SampleClass
    {
        public int Seed { get; set; }

        public static bool ParseBool(string str) => bool.Parse(str);

        public int Add(int num) => this.Seed + num;

        public async Task<string> GetAsync() => await Task.FromResult("Seed: " + this.Seed);
    }
}
