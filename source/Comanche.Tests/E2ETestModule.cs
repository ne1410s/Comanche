// <copyright file="E2ETestModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comanche.Attributes;
using Comanche.Services;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Module for end to end tests.
/// </summary>
[Module("e2e")]
public static class E2ETestModule
{
    /// <summary>
    /// Commented module.
    /// </summary>
    [Module("9commented:")]
    public sealed class CommentedModule
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

        public static IOutputWriter PassThru(IOutputWriter writer) => writer;

        public static IOutputWriter PassThruHidden([Hidden]IOutputWriter writer) => writer;

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

        public static class StaticModule
        {
            public static async Task Delay(int ms) => await Task.Delay(ms);

            public static class SingleMod
            {
                public static void Do()
                {
                    // Empty
                }
            }
        }

        [Module("empty")]
        public static class EmptyModule
        {
            [Hidden]
            public static int Do() => 42;
        }

        [Module("param-test")]
        public static class ParamTestModule
        {
            public static DateTime Change(
                DateTime d1,
                decimal m1 = 3.2m,
                long? i1 = 1) => d1.AddHours(i1 ?? (double)m1);

            public static int[] GetNums() => [1, 2, 3];
        }

        [Module("enumz")]
        public static class EnumzModule
        {
            public static EnumzModel GetNested() => new(DayOfWeek.Friday);

            public static DayOfWeek GetDirect() => DayOfWeek.Friday;

            public static int Set(DayOfWeek day) => (int)day;

            public static string? GetConfig([Hidden] IConfiguration config, string key) => config[key];
        }

        [Module("di")]
        public class DIModule(IConfiguration config)
        {
            public DIModule()
                : this(null!) { }

            public string GetName() => config?["ConfigName"]!;
        }

        [Module("missing-di")]
        public class MissingDIModule(IList<string> notInjected)
        {
            public IList<string> Get() => notInjected;
        }
    }
}

[Hidden]
public record EnumzModel(DayOfWeek Day);
