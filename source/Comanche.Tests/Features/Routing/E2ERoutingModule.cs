// <copyright file="E2ERoutingModule.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

namespace Comanche.Tests.Features.Routing;

[Alias("routez")]
public class E2ERoutingModule : IModule { }

[Hidden]
public class E2EHiddenSubRouteModule : E2ERoutingModule { }

[Alias("revealed")]
public class E2EUnhiddenSubRouteModule : E2EHiddenSubRouteModule
{
    public static int Do() => 42;
}