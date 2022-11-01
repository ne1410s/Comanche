// <copyright file="ParamsException.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents errors that occur processing parameters.
    /// </summary>
    public class ParamsException : ComancheException
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ParamsException"/> class.
        /// </summary>
        /// <param name="errors">A sequence of errors.</param>
        public ParamsException(IEnumerable<string> errors)
            : base($"Parameters not valid:{Environment.NewLine}  > "
                + $"{string.Join($"{Environment.NewLine}  > ", errors)}")
        {
            this.Errors = errors.ToList();
        }

        /// <summary>
        /// Gets a list of errors.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }
    }
}