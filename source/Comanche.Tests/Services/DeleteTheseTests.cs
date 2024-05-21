using System;
using System.Reflection;
using Comanche.Attributes;

namespace Comanche.Tests.Services;

public class DeleteTheseTests
{
    [Theory]
    [InlineData(typeof(ITestMod), false, false)]
    [InlineData(typeof(NoSkills), false, false)]
    [InlineData(typeof(RootMod1), true, false)]
    [InlineData(typeof(RootMod2), true, false)]
    [InlineData(typeof(RootMod3Base), false, false)]
    [InlineData(typeof(RootMod3), true, false)]
    [InlineData(typeof(RootMod1_Sub1), true, true)]
    [InlineData(typeof(RootMod1_Sub1_Sub), true, true)]
    [InlineData(typeof(WootieTest), false, true)]
    [InlineData(typeof(BackInTheGame), true, true)]

    [InlineData(typeof(INothing), false, false)]
    [InlineData(typeof(Almost), false, false)]
    [InlineData(typeof(NoDice), false, false)]
    [InlineData(typeof(AbsolutelyNot), false, false)]
    [InlineData(typeof(RootIfUCanBelieve), true, false)]
    [InlineData(typeof(OkNotAnymore), false, true)]
    [InlineData(typeof(Almost2), false, false)]
    [InlineData(typeof(NoDice2), false, false)]
    [InlineData(typeof(AbsolutelyNot2), false, false)]
    [InlineData(typeof(OkNotAnymore2), false, false)]
    [InlineData(typeof(Finally), true, false)]
    [InlineData(typeof(FinallySub), true, true)]
    public void HasAncestor_VaryingType_ReturnsExpected(Type type, bool expectTypeMatch, bool expectParentMatch)
    {
        var predicate = (Type t) =>
               typeof(ITestMod).IsAssignableFrom(t)
            && t.IsPublic
            && !t.IsAbstract
            && t.GetCustomAttribute<HiddenAttribute>(false) == null;

        var meetsCriteria = predicate(type);
        var hasQualParent = type.HasParent(predicate);

        meetsCriteria.Should().Be(expectTypeMatch);
        hasQualParent.Should().Be(expectParentMatch);
    }

    [Theory]
    [InlineData(typeof(ITestMod), null)]
    [InlineData(typeof(NoSkills), null)]
    [InlineData(typeof(RootMod1), null)]
    [InlineData(typeof(RootMod2), null)]
    [InlineData(typeof(RootMod3Base), null)]
    [InlineData(typeof(RootMod3), null)]
    [InlineData(typeof(RootMod1_Sub1), typeof(RootMod1))]
    [InlineData(typeof(RootMod1_Sub1_Sub), typeof(RootMod1_Sub1))]
    [InlineData(typeof(WootieTest), typeof(RootMod1_Sub1_Sub))]
    [InlineData(typeof(BackInTheGame), typeof(RootMod1_Sub1_Sub))]

    [InlineData(typeof(INothing), null)]
    [InlineData(typeof(Almost), null)]
    [InlineData(typeof(NoDice), null)]
    [InlineData(typeof(AbsolutelyNot), null)]
    [InlineData(typeof(RootIfUCanBelieve), null)]
    [InlineData(typeof(OkNotAnymore), typeof(RootIfUCanBelieve))]
    [InlineData(typeof(Almost2), null)]
    [InlineData(typeof(NoDice2), null)]
    [InlineData(typeof(AbsolutelyNot2), null)]
    [InlineData(typeof(OkNotAnymore2), null)]
    [InlineData(typeof(Finally), null)]
    [InlineData(typeof(FinallySub), typeof(Finally))]
    public void FindParent_VaryingType_ReturnsExpected(Type test, Type? expected)
    {
        var predicate = (Type t) =>
               typeof(ITestMod).IsAssignableFrom(t)
            && t.IsPublic
            && !t.IsAbstract
            && t.GetCustomAttribute<HiddenAttribute>(false) == null;

        var actual = test.FindParent(predicate);

        var match = actual == expected;
        match.Should().BeTrue();
     }
}

public interface ITestMod { }

// Series 2, bitch
public interface INothing { }
[Hidden] public class Almost : ITestMod { }
public abstract class NoDice : Almost { }
[Hidden] public abstract class AbsolutelyNot : NoDice { }
public class RootIfUCanBelieve : AbsolutelyNot { }
[Hidden] public class OkNotAnymore : RootIfUCanBelieve { }
[Hidden] public class Almost2 : ITestMod { }
public abstract class NoDice2 : Almost2 { }
[Hidden] public abstract class AbsolutelyNot2 : NoDice2 { }
[Hidden] public class OkNotAnymore2 : AbsolutelyNot2 { }
public class Finally : OkNotAnymore2 { }
public class FinallySub : Finally { }

// Series 1
public class NoSkills { }
public class RootMod1 : ITestMod { }
public class RootMod2 : ITestMod { }
public abstract class RootMod3Base : ITestMod { }
public class RootMod3 : RootMod3Base { }
public class RootMod1_Sub1 : RootMod1 { }
public class RootMod1_Sub1_Sub : RootMod1_Sub1 { }
public abstract class WootieTest : RootMod1_Sub1_Sub { }
public class BackInTheGame : WootieTest { }

public static class Exts
{
    public static bool HasParent(this Type t, Func<Type, bool> m)
        => t.BaseType != null && (m(t.BaseType) || HasParent(t.BaseType, m));

    public static Type? FindParent(this Type t, Func<Type, bool> m)
    {
        if (t.BaseType == null)
        {
            return null;
        }

        if (m(t.BaseType))
        {
            return t.BaseType;
        }

        return FindParent(t.BaseType, m);
    }
}