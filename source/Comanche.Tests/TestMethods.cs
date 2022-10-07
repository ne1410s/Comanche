// <copyright file="TestMethods.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

using System.Reflection;

namespace Comanche.Tests
{
    /// <summary>
    /// Test methods.
    /// </summary>
    public static class TestMethods
    {
        /// <summary>
        /// Does some parsing.
        /// Nothing more.
        /// </summary>
        /// <param name="text">Text param.</param>
        /// <returns>The parsed value.</returns>
        [Alias("p")]
        public static double? Parse(string text) => double.TryParse(text, out double r) ? r : null;

        /// <summary>
        /// Adds two numbers.
        /// </summary>
        /// <param name="num1">The first number.</param>
        /// <param name="num2">The second number.</param>
        /// <param name="num3">The third number.</param>
        /// <returns>The sum.</returns>
        public static double Add(
            [Alias("n1")] double num1,
            double num2,
            [Alias("n 3 :@")] double num3 = 0) => num1 + num2 + num3;

        /// <summary>
        /// Provides a method for empty / default tests.
        /// </summary>
        /// <param name="p1">Param 1.</param>
        /// <param name="p2">Param 2.</param>
        /// <returns>The result.</returns>
        public static string Empty(string p1 = "default", bool p2 = false) => p1 + p2;

        /// <summary>
        /// Returns true!
        /// </summary>
        /// <returns><see langword="true"/></returns>
        public static bool ParamlessMethod() => true;

        internal static MethodInfo Parse_Info
            => typeof(TestMethods).GetMethod(nameof(TestMethods.Parse))!;

        internal static MethodInfo Add_Info
            => typeof(TestMethods).GetMethod(nameof(TestMethods.Add))!;

        internal static MethodInfo Empty_Info
            => typeof(TestMethods).GetMethod(nameof(TestMethods.Empty))!;

        internal static MethodInfo ParamlessMethod_Info
            => typeof(TestMethods).GetMethod(nameof(TestMethods.ParamlessMethod))!;
    }
}