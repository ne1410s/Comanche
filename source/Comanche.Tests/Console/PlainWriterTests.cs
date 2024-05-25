// <copyright file="PlainWriterTests.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Console;

/// <summary>
/// Tests for the <see cref="PlainWriter"/> class.
/// </summary>
public class PlainWriterTests
{
    [Fact]
    public void CaptureStrings_WhenCalled_ReturnsNull()
    {
        // Arrange, Act & Assert
        new PlainWriter().CaptureStrings().Should().BeNull();
    }
}
