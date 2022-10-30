// <copyright file="NumbersModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.CliTest
{
    using Comanche.AttributesV2;

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
            return double.TryParse(myStr, out _);
        }

        /// <summary>
        /// Does.
        /// </summary>
        /// <returns>Unity.</returns>
        [Alias("d")]
        public static int Do() => 1;

        /// <summary>
        /// Something special.
        /// </summary>
        [Module("alg!!")]
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
        /// Should not be exported.
        /// </summary>
        public abstract class Unexposed
        {
            /// <summary>
            /// Does.
            /// </summary>
            /// <returns>Unity.</returns>
            public static int Do() => 1;
        }

        /// <summary>
        /// Should not be exported.
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
}