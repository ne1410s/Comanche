using System.Reflection;

namespace Comanche.Models
{
    /// <summary>
    /// A result for a method help route operation.
    /// </summary>
    public class MethodHelp : HelpRoute
    {
        /// <summary>
        /// Gets the method.
        /// </summary>
        public MethodInfo Method { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="MethodHelp"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        public MethodHelp(MethodInfo method)
        {
            Method = method;
        }
    }
}