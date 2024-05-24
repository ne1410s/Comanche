// <copyright file="GlobalSuppressions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1002:Do not expose generic lists",
    Justification = "Example code",
    Scope = "namespaceanddescendants",
    Target = "~N:Comanche.Tests.Simulation")]
[assembly: SuppressMessage(
    "Design",
    "CA1034:Nested types should not be visible",
    Justification = "Example code",
    Scope = "namespaceanddescendants",
    Target = "~N:Comanche.Tests.Simulation")]
[assembly: SuppressMessage(
    "Reliability",
    "CA2000:Dispose objects before losing scope",
    Justification = "Testing requirement",
    Scope = "namespaceanddescendants",
    Target = "~N:Comanche.Tests.Features.Console")]
