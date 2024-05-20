// <copyright file="IConsoleExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services;

using Comanche.Models;

/// <summary>
/// Extensions for <see cref="IConsole"/>.
/// </summary>
public static class IConsoleExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="console"></param>
    /// <param name="prefix"></param>
    /// <param name="main"></param>
    /// <param name="extra"></param>
    /// <param name="suffix"></param>
    internal static void WriteStructured(
        this IConsole console, string main, string? prefix = null, string? extra = null, string? suffix = null)
    {
        console.Write(prefix, WriteStyle.Highlight1);
        console.Write(main);
        console.Write(extra, WriteStyle.Highlight2);
        console.Write(suffix, WriteStyle.Highlight3);
        console.WriteLine();
    }
}
