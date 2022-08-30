namespace DemoLibrary.Api.Models
{
    /// <summary>
    /// An http error outcome.
    /// </summary>
    public class HttpErrorOutcome
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="HttpErrorOutcome"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorBody">The error body.</param>
        public HttpErrorOutcome(int errorCode, HttpErrorBody errorBody)
        {
            ErrorCode = errorCode;
            ErrorBody = errorBody;
        }

        /// <summary>
        /// Gets the error code.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Gets the error body.
        /// </summary>
        public HttpErrorBody ErrorBody { get; }
    }
}