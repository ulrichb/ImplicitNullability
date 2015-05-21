using System;
using System.Collections.Generic;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
  [TestFixture]
  public class IteratorsSampleTests
  {
    private IteratorsSample _instance;

    [SetUp]
    public void SetUp ()
    {
      _instance = new IteratorsSample();
    }

    [Test]
    public void TestIterator ()
    {
      Func<IEnumerable<object>> act = () => _instance.TestIterator ("a");

      act.Enumerating().ShouldNotThrow();
    }

    [Test]
    public void TestIteratorWithNullArgument ()
    {
      // ReSharper disable once IteratorMethodResultIsIgnored
      Action act = () => _instance.TestIterator (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException> ("throws immediately => no ForceIteration needed")
          .And.ParamName.Should().Be ("str");
    }

    [Test]
    public void TestIteratorWithManualNullCheckWithNullArgument ()
    {
      // ReSharper disable once IteratorMethodResultIsIgnored
      Action act = () => _instance.TestIteratorWithManualNullCheck (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldNotThrow ("no iteration");
    }

    [Test]
    public void TestIteratorWithManualNullCheckWithNullArgumentAndEnumerating ()
    {
      Func<IEnumerable<object>> act = () => _instance.TestIteratorWithManualNullCheck (null /*Expect:AssignNullToNotNullAttribute*/);

      act.Enumerating().ShouldThrow<ArgumentNullException>()
          .And.ParamName.Should().Be ("str");
    }
  }
}