// <copyright file="ModuleHelp.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A result for a module help route operation.
    /// </summary>
    public class ModuleHelp : HelpRoute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ModuleHelp"/> class.
        /// </summary>
        /// <param name="modules">The modules.</param>
        public ModuleHelp(HashSet<string> modules)
        {
            this.Modules = modules;
        }

        /// <summary>
        /// Gets the modules.
        /// </summary>
        public HashSet<string> Modules { get; }
    }
}