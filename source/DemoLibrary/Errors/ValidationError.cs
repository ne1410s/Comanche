using System.Collections.Generic;
using DemoLibrary.Validation;

namespace DemoLibrary.Errors
{
    /// <summary>
    /// Represents errors occuring during validation.
    /// </summary>
    public class ValidationError : ServiceOrchestrationError
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ValidationError"/>
        /// class.
        /// </summary>
        /// <param name="invalidItems">Invalid items.</param>
        public ValidationError(params InvalidItem[] invalidItems) : base("Invalid instance received.")
        {
            InvalidItems = invalidItems;
        }

        /// <summary>
        /// Gets the invalid items.
        /// </summary>
        public IList<InvalidItem> InvalidItems { get; }
    }
}