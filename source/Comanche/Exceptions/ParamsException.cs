using System;
using System.Collections.Generic;
using System.Linq;

namespace Comanche.Exceptions
{
    /// <summary>
    /// Represents errors that occur processing parameters.
    /// </summary>
    public class ParamsException : ComancheException
    {
        /// <summary>
        /// A list of errors.
        /// </summary>
        public IReadOnlyList<string> Errors { get; }

        /// <summary>
        /// Initialises a new version of the <see cref="ParamsException"/> class.
        /// </summary>
        /// <param name="errors">A sequence of errors.</param>
        public ParamsException(IEnumerable<string> errors)
            : base($"Parameters not valid:{Environment.NewLine}  > "
                + $"{string.Join($"{Environment.NewLine}  > ", errors)}")
        {
            Errors = errors.ToList();
        }
    }
}