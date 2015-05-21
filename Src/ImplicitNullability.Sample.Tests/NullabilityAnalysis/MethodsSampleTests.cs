using System;
using System.Reflection;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
  [TestFixture]
  public class MethodsSampleTests
  {
    private MethodsSample _instance;

    [SetUp]
    public void SetUp ()
    {
      _instance = new MethodsSample();
    }

    [Test]
    public void TestProcedureWithNonNullValue ()
    {
      Action act = () => _instance.TestProcedure ("a");

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNullValue ()
    {
      Action act = () => _instance.TestProcedure (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("a");
    }

    [Test]
    public void TestProcedureWithExplicitCanBeNull ()
    {
      Action act = () => _instance.TestProcedureWithExplicitCanBeNull (null);

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithImplicitCanBeNull ()
    {
      Action act = () => _instance.TestProcedureWithImplicitCanBeNull (nullableInt: null, optional: null);

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNotNullAnnotationForNullableInt ()
    {
      Action act = () => _instance.TestProcedureWithNotNullAnnotationForNullableInt (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldNotThrow ("the NotNull-annotation won't be respected by NullGuard (and it doesn't really make sense for nullable value types)");
    }

    [Test]
    public void TestProcedureWithNotNullAnnotationForNullDefaultArgument ()
    {
      Action act = () => _instance.TestProcedureWithNotNullAnnotationForNullDefaultArgument (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldNotThrow ("the NotNull-annotation won't be respected by NullGuard (and it doesn't really make sense for null default arguments)");
    }

    [Test]
    public void TestProcedureWithOutAndRefParameters ()
    {
      Action act = () =>
      {
        string outString;
        string refString = "s";
        _instance.TestProcedureWithOutAndRefParameters (out outString, ref refString);
      };

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithOutAndRefParametersWithNullValue ()
    {
      Action act = () =>
      {
        string outString;
        string refString = null;
        _instance.TestProcedureWithOutAndRefParameters (out outString, ref refString /* REPORT? constant null value to NotNull parameter */);
      };

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("refString");
    }

    [Test]
    public void TestProcedureWithExplicitNotNullRefParameter ()
    {
      Action act = () =>
      {
        string refString = "s";
        _instance.TestProcedureWithExplicitNotNullRefParameter (ref refString);
      };

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithExplicitNotNullRefParameterWithNullValue ()
    {
      Action act = () =>
      {
        string refString = null;
        _instance.TestProcedureWithExplicitNotNullRefParameter (ref refString /* REPORT? constant null value to NotNull parameter */);
      };

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("refString");
    }

    [Test]
    public void TestProcedureWithCanBeNullRefParameter ()
    {
      Action act = () =>
      {
        string refString = null;
        _instance.TestProcedureWithCanBeNullRefParameter (ref refString);
      };

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNullDefaultOfStringDefaultArgument ()
    {
      Action act = () => _instance.TestProcedureWithNullDefaultOfStringDefaultArgument (optional: null);

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNullDefaultArgumentFromConst ()
    {
      Action act = () => _instance.TestProcedureWithNullDefaultArgumentFromConst (optional: null);

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNullDefaultArgumentFromDefaultOfStringConst ()
    {
      Action act = () => _instance.TestProcedureWithNullDefaultArgumentFromDefaultOfStringConst (optional: null);

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNonNullDefaultArgument ()
    {
      Action act = () => _instance.TestProcedureWithNonNullDefaultArgument (optional: "overridden default");

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNonNullDefaultArgumentCalledWithNullValue ()
    {
      Action act = () => _instance.TestProcedureWithNonNullDefaultArgument (optional: null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("optional");
    }

    [Test]
    public void TestProcedureWithNonNullDefaultArgumentFromConst ()
    {
      Action act = () => _instance.TestProcedureWithNonNullDefaultArgumentFromConst (optional: "overridden default");

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithNonNullDefaultArgumentFromConstWithNullValue ()
    {
      Action act = () => _instance.TestProcedureWithNonNullDefaultArgumentFromConst (optional: null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("optional");
    }

    [Test]
    public void TestProcedureWithParams ()
    {
      Action act = () => _instance.TestProcedureWithParams ("a", "b");

      act.ShouldNotThrow();
    }

    [Test]
    public void TestProcedureWithParamsWithNullValue ()
    {
      Action act = () => _instance.TestProcedureWithParams (a: null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("a");
    }

    [Test]
    public void TestFunctionWithNullReturnValue ()
    {
      Action act = () => _instance.TestFunction (null);

      act.ShouldNotThrow ("at the moment, method return values should not be guarded");
    }

    [Test]
    public void TestCanBeNullFunctionWithNullReturnValue ()
    {
      Action act = () => _instance.TestCanBeNullFunction (null);

      act.ShouldNotThrow();
    }

    [Test]
    public void TestInternalMethodWithNullArgument ()
    {
      Action act = () => GetNonPublicMethod ("InternalMethod").Invoke (_instance, new object[] { null });

      act.ShouldThrow<TargetInvocationException>()
          .And.InnerException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be ("internalMethodParameter");
    }

    [Test]
    public void TestProtectedMethodWithNullArgument ()
    {
      Action act = () => GetNonPublicMethod ("ProtectedMethod").Invoke (_instance, new object[] { null });

      act.ShouldThrow<TargetInvocationException>()
          .And.InnerException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be ("protectedMethodParameter");
    }

    [Test]
    public void TestPrivateMethodWithNullArgument ()
    {
      Action act = () => GetNonPublicMethod ("PrivateMethod").Invoke (_instance, new object[] { null });

      act.ShouldThrow<TargetInvocationException>()
          .And.InnerException.Should().BeOfType<ArgumentNullException>().Which.ParamName.Should().Be ("privateMethodParameter");
    }

    [Test]
    public void StaticTestProcedureWithNonNullValue ()
    {
      Action act = () => MethodsSample.StaticTestProcedure ("a");

      act.ShouldNotThrow();
    }

    [Test]
    public void StaticTestProcedureWithNullValue ()
    {
      Action act = () => MethodsSample.StaticTestProcedure (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("a");
    }

    [Test]
    public void GenericMethodWithNullValue ()
    {
      Action act = () => _instance.GenericMethod ((string) null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("a");
    }

    private MethodInfo GetNonPublicMethod (string name)
    {
      return _instance.GetType().GetMethod (name, BindingFlags.NonPublic | BindingFlags.Instance);
    }
  }
}