// <copyright file="IHelpGenerator.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Services
{
    using Comanche.Models;

    /// <summary>
    /// Generates help.
    /// </summary>
    public interface IHelpGenerator
    {
        /// <summary>
        /// Generate help from a route.
        /// </summary>
        /// <param name="helpRoute">The route.</param>
        /// <returns>Help text.</returns>
        public string GenerateHelp(HelpRoute helpRoute);
    }
}