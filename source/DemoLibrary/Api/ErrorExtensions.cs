using System;
using System.Linq;
using DemoLibrary.Api.Models;
using DemoLibrary.Errors;
using DemoLibrary.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DemoLibrary.Api
{
    /// <summary>
    /// Error handling extensions.
    /// </summary>
    public static class ErrorExtensions
    {
        /// <summary>
        /// Maps an error to a suggested http outcome.
        /// </summary>
        /// <param name="ex">The error.</param>
        /// <returns>The outcome.</returns>
        public static HttpErrorOutcome ToOutcome(this Exception ex)
        {
            var errorCode = ex.ToErrorCode();
            var errorMessage = errorCode == 500 ? "An unexpected error occurred" : ex.Message;
            var invalidItems = ex is ValidationError valEx ? valEx.InvalidItems : null;

            return new HttpErrorOutcome(
                errorCode,
                new HttpErrorBody(ex.GetType().Name, errorMessage, invalidItems));
        }

        /// <summary>
        /// Maps framework validation failures to a list of invalid items.
        /// </summary>
        /// <param name="state">The state containing the raw errors.</param>
        /// <returns>A mapped array.</returns>
        public static InvalidItem[] ToItems(this ModelStateDictionary state)
            => state.Select(e => new InvalidItem(
                e.Key,
                string.Join(", ", e.Value.Errors.Select(m => m.ErrorMessage)),
                e.Value.RawValue)).ToArray();

        /// <summary>
        /// Maps exceptions to http error codes.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>An http error code.</returns>
        public static int ToErrorCode(this Exception ex) => ex switch
        {
            StaticValidationError _ => 400,
            AuthorisationError _ => 403,
            ResourceMissingError _ => 404,
            ServiceOrchestrationError _ => 422,
            _ => 500,
        };
    }
}