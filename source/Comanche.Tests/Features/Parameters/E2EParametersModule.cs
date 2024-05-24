// <copyright file="E2EParametersModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Parameters;

using System.Collections.Generic;
using System.Linq;

[Alias("paramz")]
public class E2EParametersModule : IModule
{
    public static int SumDictoValues(IDictionary<string, int> dicto)
        => dicto.Sum(d => d.Value);

    public static int SumHashSet(HashSet<int> n) => n.Sum();
}
