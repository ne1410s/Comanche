// <copyright file="RootModules.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Simulation;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

/// <summary>
/// Module for end to end tests.
/// </summary>
[Alias("e2e")]
public class E2ETestModule : IModule
{
    /// <summary>
    /// Commented module.
    /// </summary>
    [Alias("9commented:")]
    public class CommentedModule : E2ETestModule
    {
        private int Seed { get; } = 12;

        /// <summary>
        /// Join array.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="x">The x.</param>
        /// <returns>Val.</returns>
        public static async Task<string> JoinArray(string[] s, string x = "!")
        {
            await Task.CompletedTask;
            return string.Join(", ", s) + x;
        }

        /// <summary>
        /// Throws a thing.
        /// </summary>
        /// <param name="test">Test.</param>
        public static void Throw(bool test = false) =>
            throw new ArgumentException(test ? "1" : "2", nameof(test));

        public static int SumArray([Alias("numbers")] int[] n) => n.Sum();

        public static IConsole PassThru(IConsole writer) => writer;

        public static IConsole PassThruHidden([Hidden] IConsole writer) => writer;

        public static short Next([Alias(null!)] byte? b) => (short)((b ?? byte.MaxValue) + 1);

        public static int SumDicto(Dictionary<string, int>? d = null) => (d ?? []).Values.Sum();

        /// <summary>
        /// Sums ints.
        /// </summary>
        /// <param name="numbers">Integers.</param>
        /// <param name="n">N.</param>
        /// <param name="otherSeed">Other seed.</param>
        /// <returns>Sum plus a seed.</returns>
        [Alias("sum")]
        public int SumList(
            [Alias("n")] List<int> numbers,
            [Alias("numbers")] int n = 34,
            [Hidden] int otherSeed = 0) => this.Seed + n + otherSeed + numbers.Sum();

        public class NestedModule : CommentedModule
        {
            public static async Task Delay(int ms) => await Task.Delay(ms);

            public static void CanThrow([Hidden] IConsole console) => console.Write("throw bro");

            public class SingleMod : NestedModule
            {
                public static void Do()
                {
                    // Empty
                }
            }
        }
    }
}