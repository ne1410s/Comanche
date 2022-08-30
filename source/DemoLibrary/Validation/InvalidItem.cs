namespace DemoLibrary.Validation
{
    /// <summary>
    /// Represents an invalid item.
    /// </summary>
    public class InvalidItem
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="InvalidItem"/> class.
        /// </summary>
        /// <param name="property">The property name.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="attemptedValue">The attempted value.</param>
        public InvalidItem(string property, string errorMessage, object? attemptedValue)
        {
            Property = property;
            ErrorMessage = errorMessage;
            AttemptedValue = attemptedValue;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets the attempted value.
        /// </summary>
        public object? AttemptedValue { get; }
    }
}