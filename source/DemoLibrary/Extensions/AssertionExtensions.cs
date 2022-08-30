using System;
using System.Collections.Generic;
using System.Text.Json;
using DemoLibrary.Errors;
using DemoLibrary.Validation;

namespace DemoLibrary.Extensions
{
    /// <summary>
    /// Provides fluent semantics for raising pre-emptive exception types.
    /// </summary>
    public static class AssertionExtensions
    {
        /// <summary>
        /// Asserts that an instance must be a default value (or a null
        /// reference).
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="DataStateError">Assertion failed.</exception>
        public static void MustBeUnpopulated<T>(this T obj, string? message = null, Func<bool>? unless = null)
            => obj.IsDefault().MustBe(true, message, unless);

        /// <summary>
        /// Asserts that an instance must not be a default value (or a null
        /// reference).
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="DataStateError">Assertion failed.</exception>
        public static void MustBePopulated<T>(this T obj, string? message = null, Func<bool>? unless = null)
            => obj.IsDefault().MustBe(false, message, unless);

        /// <summary>
        /// Asserts that a reference must be equivalent to another, by way of
        /// comparing default serialization results.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="DataStateError">Assertion failed.</exception>
        public static void MustSerializeAs<T>(this T obj, T expected, string? message = null, Func<bool>? unless = null)
            where T : class
            => obj.SerializesAs(expected).MustBe(true, message, unless);

        /// <summary>
        /// Asserts that a reference must not be equivalent to another, by way of
        /// comparing default serialization results.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="unexpected">An unexpected value.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="DataStateError">Assertion failed.</exception>
        public static void MustNotSerializeAs<T>(this T obj, T unexpected, string? message = null, Func<bool>? unless = null)
            where T : class
            => obj.SerializesAs(unexpected).MustBe(false, message, unless);

        /// <summary>
        /// Asserts that a value must not match an unexpected value.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="unexpected">An unexpected value.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="DataStateError">Assertion failed.</exception>
        public static void MustNotBe<T>(this T obj, T unexpected, string? message = null, Func<bool>? unless = null)
            where T : struct
            => Equals(obj, unexpected).MustBe(false, message, unless);

        /// <summary>
        /// Asserts that a value must match the expected value.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="expected">The expected value.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="DataStateError">Assertion failed.</exception>
        public static void MustBe<T>(this T obj, T expected, string? message = null, Func<bool>? unless = null)
            where T : struct
            => Equals(obj, expected).MustBeInGoodState(message, unless);

        /// <summary>
        /// Asserts that an object passes the supplied validation criteria.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="validator">The validator.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="ValidationError">Assertion failed.</exception>
        public static void MustAdhereTo<T>(this T obj, IItemValidator<T> validator, Func<bool>? unless = null)
        {
            if (unless?.Invoke() != true)
            {
                validator.AssertValid(obj);
            }
        }

        /// <summary>
        /// Asserts that a resource must not be null, or otherwise default.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="ResourceMissingError">Assertion failed.</exception>
        public static void MustExist<T>(this T obj, string? message = null, Func<bool>? unless = null)
        {
            if (unless?.Invoke() != true && obj.IsDefault())
            {
                throw new ResourceMissingError(message);
            }
        }

        /// <summary>
        /// Asserts that an operation is authorised.
        /// </summary>
        /// <param name="allowed">Whether an operation is allowed.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="AuthorisationError">Assertion failed.</exception>
        public static void MustBeAllowed(this bool allowed, string? message = null, Func<bool>? unless = null)
        {
            if (unless?.Invoke() != true && !allowed)
            {
                throw new AuthorisationError(message);
            }
        }

        /// <summary>
        /// Asserts that a state check yields a true value.
        /// </summary>
        /// <param name="inGoodState">Whether state is considered good.</param>
        /// <param name="message">Used if the check fails.</param>
        /// <param name="unless">Exemption criteria.</param>
        /// <exception cref="DataStateError">Assertion failed.</exception>
        private static void MustBeInGoodState(this bool inGoodState, string? message, Func<bool>? unless = null)
        {
            if (unless?.Invoke() != true && !inGoodState)
            {
                throw new DataStateError(message);
            }
        }

        /// <summary>
        /// Returns <see langword="true"/> if a value type is
        /// <see langword="default"/> (or a reference type is
        /// <see langword="null"/>).
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>Whether the object is default.</returns>
        private static bool IsDefault<T>(this T obj)
            => EqualityComparer<T>.Default.Equals(obj, default!);

        /// <summary>
        /// Compares resulting strings following default serialization.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="comparison">A comparison object.</param>
        /// <returns>Whether the objects serialize to the same result.</returns>
        private static bool SerializesAs<T>(this T obj, T comparison)
            where T : class
            => JsonSerializer.Serialize(obj) == JsonSerializer.Serialize(comparison);
    }
}
