// <copyright file="IParamValidator.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services
{
    using System;
    using Comanche.Models;

    /// <summary>
    /// Validates parameters.
    /// </summary>
    public interface IParamValidator
    {
        /// <summary>
        /// Validates a method route.
        /// </summary>
        /// <param name="methodRoute">A method route.</param>
        /// <returns>An ordered set of typed (boxed) parameters.</returns>
        public object?[]? Validate(MethodRoute methodRoute);

        /// <summary>
        /// Tries to convert a string argument to the supplied type.
        /// </summary>
        /// <param name="arg">The parameter as text.</param>
        /// <param name="type">The parameter type.</param>
        /// <param name="result">Any result.</param>
        /// <param name="errorMsg">Any error.</param>
        /// <returns>Whether conversion succeeded.</returns>
        public bool TryConvert(string arg, Type type, out object? result, out string? errorMsg);
    }
}