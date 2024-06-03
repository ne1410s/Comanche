// <copyright file="E2EParametersModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;

[Alias("paramz")]
public class E2EParametersModule : IModule
{
    public static int SumDictoValues(IDictionary<string, int> dicto)
        => dicto.Sum(d => d.Value);

    public static int SumHashSet(HashSet<int> n)
        => n.Sum();

    public static int SumBadHashSet(BadHashSet<int> n)
        => n.Sum();

    public static int? SumOptionalArray(int[]? n = null)
        => n?.Sum() ?? -1;

    public static int? SumGenericInterface(ICollection<int> n)
        => n.Sum();

    public static void CorrectDI([Hidden] IConsole console)
        => console.Write("woot");

    public static void IncorrectDI(IConsole console)
        => console.Write("woot");

    public static string OptionalBool(bool force = false)
        => force.ToString();

    public static DayOfWeek NextDay([Alias("d")] DayOfWeek day)
        => (DayOfWeek)(((int)day + 1) % 7);

    public static string ReformatGuid(Guid id)
        => id.ToString("P");
}

public class BadHashSet<T> : HashSet<T>
{
    public BadHashSet() { }

    public BadHashSet(IEnumerable<int> values)
        => throw new NotImplementedException();
}