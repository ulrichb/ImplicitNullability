using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
  [TestFixture]
  public class AsyncMethodsSampleTests
  {
    private AsyncMethodsSample _instance;

    [SetUp]
    public void SetUp ()
    {
      _instance = new AsyncMethodsSample();
    }

    [Test]
    public void TestAsync ()
    {
      Action act = () => _instance.TestAsync ("a").Wait();

      act.ShouldNotThrow();
    }

    [Test]
    public void TestAsyncWithNullArgument ()
    {
      // Note that TestAsync throws immediately => no Wait() call
      Action act = () => _instance.TestAsync (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException> ("not an AggregateException because the outermost (rewritten) async method throwed")
          .And.ParamName.Should().Be ("a");
    }

    [Test]
    public void TestAsyncWithManualNullCheck ()
    {
      Action act = () => _instance.TestAsyncWithManualNullCheck (null /*Expect:AssignNullToNotNullAttribute*/).Wait();

      act.ShouldThrow<AggregateException> ("the outermost (non rewritten) method throws *within* the async state machine")
          .And.InnerException.Should().BeOfType<ArgumentNullException>()
          .Which.ParamName.Should().Be ("a");
    }

    [Test]
    public void CallTestAsyncWithNullArgument ()
    {
      Action act = () => _instance.CallTestAsyncWithNullArgument().Wait();

      act.ShouldThrow<AggregateException> ("wrapped by the outer CallTestAsync method")
          .And.InnerException.Should().BeOfType<ArgumentNullException>()
          .Which.ParamName.Should().Be ("a");
    }
  }
}