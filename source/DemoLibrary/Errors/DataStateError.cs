namespace DemoLibrary.Errors
{
    /// <summary>
    /// Represents errors occuring due to the state of data in the store.
    /// </summary>
    public class DataStateError : ServiceOrchestrationError
    {
        /// <summary>
        /// Initialises a new instance of the<see cref="DataStateError"/>
        /// class.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataStateError(string? message) : base(message)
        { }
    }
}