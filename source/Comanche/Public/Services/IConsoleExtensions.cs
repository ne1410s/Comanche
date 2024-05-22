// <copyright file="IConsoleExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche;

using System;
using System.Linq;

/// <summary>
/// Extensions for <see cref="IConsole"/>.
/// </summary>
public static class IConsoleExtensions
{
    /// <summary>
    /// Writes text in the <see cref="ComanchePalette.Primary"/> colour.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public static void WritePrimary(this IConsole console, string text, bool line = false)
        => console.NotNull().Write(text.NotNull(), line, console.Palette.Primary);

    /// <summary>
    /// Writes text in the <see cref="ComanchePalette.Secondary"/> colour.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public static void WriteSecondary(this IConsole console, string text, bool line = false)
        => console.NotNull().Write(text.NotNull(), line, console.Palette.Secondary);

    /// <summary>
    /// Writes text in the <see cref="ComanchePalette.Tertiary"/> colour.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public static void WriteTertiary(this IConsole console, string text, bool line = false)
        => console.NotNull().Write(text.NotNull(), line, console.Palette.Tertiary);

    /// <summary>
    /// Writes error text in the <see cref="ComanchePalette.Error"/> colour.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="line">Whether to terminate with a new line.</param>
    public static void WriteError(this IConsole console, string text, bool line = false)
        => console.NotNull().Write(text.NotNull(), line, console.Palette.Error, true);

    /// <summary>
    /// Writes a new line sequence.
    /// </summary>
    /// <param name="console">The console.</param>
    public static void WriteLine(this IConsole console)
        => console.NotNull().Write(string.Empty, true);

    /// <summary>
    /// Captures a single string.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="prompt">The prompt text.</param>
    /// <param name="mask">Optional mask character.</param>
    /// <returns>The result.</returns>
    public static string? CaptureString(this IConsole console, string prompt = "Input: ", char? mask = null)
        => console.NotNull().CaptureStrings(prompt, mask, 1)?.FirstOrDefault();

    /// <summary>
    /// Writes text in different styles. This is used internally by Comanche.
    /// </summary>
    /// <param name="console">The console writer.</param>
    /// <param name="prefix">Prefix text.</param>
    /// <param name="main">The main text.</param>
    /// <param name="extra">Extra text.</param>
    /// <param name="suffix">Suffix text.</param>
    public static void WriteStructured(
        this IConsole console, string? prefix = null, string? main = null, string? extra = null, string? suffix = null)
    {
        console.NotNull().WritePrimary(prefix ?? string.Empty);
        console.Write(main);
        console.WriteSecondary(extra ?? string.Empty);
        console.WriteTertiary(suffix ?? string.Empty);
        console.Write(string.Empty, true);
    }

    /// <summary>
    /// Ensures the reference is not null.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="o">The item.</param>
    /// <returns>The original object.</returns>
    /// <exception cref="ArgumentNullException">If null.</exception>
    public static T NotNull<T>(this T o)
        => o ?? throw new ArgumentNullException(nameof(o));
}
