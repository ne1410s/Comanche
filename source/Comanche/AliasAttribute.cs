using System;

namespace Comanche
{
    /// <summary>
    /// Sets an alias.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter)]
    public class AliasAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AliasAttribute"/> class.
        /// </summary>
        /// <param name="name">The alias name.</param>
        public AliasAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the alias name.
        /// </summary>
        public string Name { get; }
    }
}