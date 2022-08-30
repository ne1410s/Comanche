using DemoLibrary.Validation;

namespace DemoLibrary.Errors
{

    /// <summary>
    /// Represents errors occuring during static validation.
    /// </summary>
    public class StaticValidationError : ValidationError
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="StaticValidationError"/>
        /// class.
        /// </summary>
        /// <param name="invalidItems">Invalid items.</param>
        public StaticValidationError(params InvalidItem[] invalidItems)
            : base(invalidItems)
        { }
    }
}