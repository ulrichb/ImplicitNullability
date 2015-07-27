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
        public void SetUp()
        {
            _instance = new IteratorsSample();
        }

        [Test]
        public void SomeIteratorWithNonNullArgument()
        {
            Func<IEnumerable<object>> act = () => _instance.SomeIterator("");

            act.Enumerating().ShouldNotThrow();
        }

        [Test]
        public void SomeIteratorWithNullArgument()
        {
            Action act = () => ReSharper.SuppressUnusedWarning(_instance.SomeIterator(null /*Expect:AssignNullToNotNullAttribute[MIn]*/));

            act.ShouldThrow<ArgumentNullException>("throws immediately => no ForceIteration needed")
                .And.ParamName.Should().Be("str");
        }

        [Test]
        public void SomeIteratorWithManualNullCheckWithNullArgument()
        {
            Action act =
                () => ReSharper.SuppressUnusedWarning(_instance.SomeIteratorWithManualNullCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/));

            act.ShouldNotThrow("no iteration");
        }

        [Test]
        public void SomeIteratorWithManualNullCheckWithNullArgumentAndEnumerating()
        {
            Func<IEnumerable<object>> act = () => _instance.SomeIteratorWithManualNullCheck(null /*Expect:AssignNullToNotNullAttribute[MIn]*/);

            act.Enumerating().ShouldThrow<ArgumentNullException>()
                .And.ParamName.Should().Be("str");
        }

        [Test]
        public void SomeIteratorReturningNullItem()
        {
            var result = _instance.SomeIteratorReturningNullItem();

            result.Should().Equal(new object[] {null});
        }
    }
}