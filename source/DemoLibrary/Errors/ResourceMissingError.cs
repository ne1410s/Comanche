namespace DemoLibrary.Errors
{
    /// <summary>
    /// Missing resource errors.
    /// </summary>
    public class ResourceMissingError : DataStateError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMissingError"/>
        /// class.
        /// </summary>
        /// <param name="message">A description of the exception.</param>
        public ResourceMissingError(string? message = null)
            : base(message ?? "Resource not found")
        { }
    }
}