// <copyright file="SubModules.cs" company="ne1410s">
// Copyright (c) ne1410s. All rights reserved.
// </copyright>

////namespace Comanche.Tests.Simulation;

////using System;
////using System.Collections.Generic;
////using System.Collections.ObjectModel;
////using System.Linq;
////using Microsoft.Extensions.Configuration;

////[Alias("empty")]
////public class EmptyModule : E2ETestModule.CommentedModule
////{
////    [Hidden]
////    public static int Do() => 42;
////}

////[Alias("param-test")]
////public class ParamTestModule : E2ETestModule.CommentedModule
////{
////    public static DateTime Change(
////        DateTime d1,
////        decimal m1 = 3.2m,
////        long? i1 = 1) => d1.AddHours(i1 ?? (double)m1);

////    public static int[] GetNums() => [1, 2, 3];
////}

////[Alias("guidz")]
////public class GuidzModule : E2ETestModule.CommentedModule
////{
////    public static string StringifyGuid(Guid id) => id.ToString();

////    public static string? StringifyOptionalGuid(Guid? id = null) => id?.ToString();

////    [Hidden]
////    public class HiddenThing : GuidzModule
////    {
////        public class NonHidden : HiddenThing
////        {
////            public static int Do() => 11;
////        }
////    }
////}

////[Alias("enumz")]
////public class EnumzModule : E2ETestModule.CommentedModule
////{
////    public static EnumzModel GetNested() => new(DayOfWeek.Friday);

////    public static DayOfWeek GetDirect() => DayOfWeek.Friday;

////    public static int Set(DayOfWeek day) => (int)day;

////    public static string? GetConfig([Hidden] IConfiguration config, string key) => config[key];
////}

////[Alias("di")]
////public class DIModule(IConfiguration config) : E2ETestModule.CommentedModule
////{
////    public DIModule()
////        : this(null!) { }

////    public string GetName() => config?["ConfigName"]!;
////}

////[Alias("missing-di")]
////public class MissingDIModule(IList<string> notInjected) : E2ETestModule.CommentedModule
////{
////    public IList<string> Get() => notInjected;
////}

////public class SequenceModule : E2ETestModule.CommentedModule
////{
////    public static new int SumArray(int[] n) => n.Sum();

////    public static int SumCollection(Collection<int> n) => n.Sum();

////    public static int SumHashSet(HashSet<int> n) => n.Sum();

////    public static int SumICollection(ICollection<int> n) => n.Sum();

////    public static int SumIEnumerable(IEnumerable<int> n) => n.Sum();

////    public static int SumIList(IList<int> n) => n.Sum();

////    public static int SumLinkedList(LinkedList<int> n) => n.Sum();

////    public static int SumList(List<int> n) => n.Sum();

////    public static int SumQueue(Queue<int> n) => n.Sum();

////    public static int SumStack(Stack<int> n) => n.Sum();

////    public static int SumUnsupported(UnsupportedSequence<int> n) => 42;
////}

////public record EnumzModel(DayOfWeek Day);

////public sealed class UnsupportedSequence<T> : List<T>
////{
////    public UnsupportedSequence(IEnumerable<T> sequence)
////    {
////        throw new NotImplementedException();
////    }
////}