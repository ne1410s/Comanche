using System.Collections.Generic;
using DemoLibrary.Validation;

namespace DemoLibrary.Api.Models
{
    /// <summary>
    /// Response returned from a unsuccessful http call.
    /// </summary>
    public class HttpErrorBody
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HttpErrorBody"/> class.
        /// </summary>
        /// <param name="type">The error type.</param>
        /// <param name="message">The error message.</param>
        /// <param name="errors">Any invalid items associated.</param>
        public HttpErrorBody(string type, string message, IEnumerable<InvalidItem>? errors = null)
        {
            Type = type;
            Message = message;
            Errors = errors;
        }

        /// <summary>
        /// Gets the error type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets a sequence of invalid items, if present.
        /// </summary>
        public IEnumerable<InvalidItem>? Errors { get; }
    }
}
