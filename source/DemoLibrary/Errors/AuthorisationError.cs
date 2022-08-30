namespace DemoLibrary.Errors
{
    /// <summary>
    /// Authorisation errors.
    /// </summary>
    public class AuthorisationError : ServiceOrchestrationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorisationError"/> class.
        /// </summary>
        /// <param name="message">A description of the exception.</param>
        public AuthorisationError(string? message = null)
            : base(message ?? "Forbidden")
        { }
    }
}