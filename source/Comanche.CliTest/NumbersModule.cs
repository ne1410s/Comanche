﻿// <copyright file="NumbersModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.CliTest;

using Comanche.Attributes;

/// <summary>
/// All about... numbers.
/// </summary>
[Module("num")]
public static class NumbersModule
{
    /// <summary>
    /// Does a check.
    /// </summary>
    /// <remarks>Need to know basis.</remarks>
    /// <param name="myStr">My string.</param>
    /// <param name="otherThing">Something else.</param>
    /// <returns>True if a number.</returns>
    public static bool Check(
        [Alias("stringy pants")] string myStr,
        [Alias("os")] bool otherThing = true)
    {
        return !otherThing || double.TryParse(myStr, out _);
    }

    /// <summary>
    /// Does.
    /// </summary>
    /// <returns>Unity.</returns>
    [Alias("d")]
    public static int Do() => 1;

    /// <summary>
    /// Does async.
    /// </summary>
    /// <returns>Duality.</returns>
    [Alias("dAsync")]
    public static async Task<int> DoAsync() => await Task.FromResult(2);

    /// <summary>
    /// Does async.
    /// </summary>
    /// <returns>Duality.</returns>
    [Alias("actAsync")]
    public static async Task ActAsync() => await Task.Delay(50);

    /// <summary>
    /// Something special.
    /// </summary>
    [Module("alg!")]
    public static class AlgebraModule
    {
        /// <summary>
        /// Adds some numbers.
        /// </summary>
        /// <param name="firstNumber">A number.</param>
        /// <param name="second">Another number.</param>
        /// <returns>The sum.</returns>
        public static int Derive(int firstNumber, int second)
        {
            return firstNumber + second;
        }
    }

    /// <summary>
    /// A sub-module.
    /// </summary>
    public static class SecretSubModule
    {
        /// <summary>
        /// Joins strings from an array.
        /// </summary>
        /// <param name="items">The strings.</param>
        /// <returns>Joined.</returns>
        public static string JoinArray(string[] items) => string.Join(", ", items);

        /// <summary>
        /// Joins strings from any old enumerable.
        /// </summary>
        /// <param name="items">The strings.</param>
        /// <returns>Joined.</returns>
        public static string JoinSeq(IEnumerable<string> items) => string.Join(", ", items);

        /// <summary>
        /// Joins strings from a list.
        /// </summary>
        /// <param name="items">The strings.</param>
        /// <returns>Joined.</returns>
        public static string JoinList(List<string> items) => string.Join(", ", items);
    }

    /// <summary>
    /// Should not be exported.
    /// </summary>
    [Hidden]
    public abstract class Unexposed
    {
        /// <summary>
        /// Does.
        /// </summary>
        /// <returns>Unity.</returns>
        public static int Do() => 1;
    }

    /// <summary>
    /// Should not be exported in opt-in mode.
    /// </summary>
    public sealed class Unexposed2
    {
        /// <summary>
        /// Does.
        /// </summary>
        /// <returns>Unity.</returns>
        public static int Do() => 1;
    }
}