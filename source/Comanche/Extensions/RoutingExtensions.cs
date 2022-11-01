// <copyright file="RoutingExtensions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Comanche.Models;

/// <summary>
/// Extensions relating to command routing.
/// </summary>
public static class RoutingExtensions
{
    /// <summary>
    /// Routes arguments for a pre-loaded session.
    /// </summary>
    /// <param name="session">The Comanche session.</param>
    /// <param name="args">The command line arguments.</param>
    /// <returns>The result.</returns>
    public static object? Route(this ComancheSession session, string[]? args = null)
    {
        args ??= Environment.GetCommandLineArgs().Skip(1).ToArray();

        throw new NotImplementedException();
    }
}
