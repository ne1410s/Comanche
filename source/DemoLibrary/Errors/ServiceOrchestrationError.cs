using System;

namespace DemoLibrary.Errors
{
    /// <summary>
    /// Represents errors occuring in general domain processing.
    /// </summary>
    public class ServiceOrchestrationError : Exception
    {
        /// <summary>
        /// Initialises a new instance of the<see cref="DataStateError"/>
        /// class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ServiceOrchestrationError(string? message) : base(message)
        { }
    }
}