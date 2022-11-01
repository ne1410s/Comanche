// <copyright file="ComancheException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions
{
    using System;

    /// <summary>
    /// Represents errors that occur in Comanche operations.
    /// </summary>
    public class ComancheException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ComancheException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ComancheException(string message)
            : base(message)
        { }
    }
}