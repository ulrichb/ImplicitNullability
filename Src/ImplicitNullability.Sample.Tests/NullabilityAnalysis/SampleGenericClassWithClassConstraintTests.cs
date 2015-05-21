using System;
using FluentAssertions;
using ImplicitNullability.Sample.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Sample.Tests.NullabilityAnalysis
{
  [TestFixture]
  public class SampleGenericClassWithClassConstraintTests
  {
    [Test]
    public void TestMethodWithNullArgument ()
    {
      var instance = new SampleGenericClassWithClassConstraint<string>();

      Action act = () => instance.TestMethod (null /*Expect:AssignNullToNotNullAttribute*/);

      act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be ("a");
    }
  }
}