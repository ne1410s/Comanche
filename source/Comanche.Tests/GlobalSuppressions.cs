// <copyright file="GlobalSuppressions.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design",
    "CA1002:Do not expose generic lists",
    Justification = "Example code",
    Scope = "type",
    Target = "~T:Comanche.Tests.E2ETestModule.CommentedModule")]
[assembly: SuppressMessage(
    "Design",
    "CA1034:Nested types should not be visible",
    Justification = "Example code",
    Scope = "type",
    Target = "~T:Comanche.Tests.E2ETestModule.CommentedModule")]
[assembly: SuppressMessage(
    "Reliability",
    "CA2000:Dispose objects before losing scope",
    Justification = "Required",
    Scope = "type",
    Target = "~T:Comanche.Tests.Services.ConsoleWriterTests")]
