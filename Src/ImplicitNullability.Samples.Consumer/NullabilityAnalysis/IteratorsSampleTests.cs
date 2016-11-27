using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
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
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public void SomeIteratorResult()
        {
            var someIterator = _instance.SomeIterator("");

            // Note that MOut also applies here (and NotNull is satisfied by definition):
            ReSharper.TestValueAnalysis(someIterator, someIterator == null /*Expect:ConditionIsAlwaysTrueOrFalse[MOut]*/);

            someIterator.Should().NotBeNull();
        }

        [Test]
        public void SomeIteratorReturningNullItem()
        {
            foreach (var item in _instance.SomeIteratorReturningNullItem())
            {
                ReSharper.TestValueAnalysis(item, item == null);

                item.Should().BeNull();
            }
        }

        [Test]
        public void SomeIteratorWithItemCanBeNull()
        {
            foreach (var item in _instance.SomeIteratorWithItemCanBeNull())
            {
                ReSharper.TestValueAnalysis(item /*Expect:AssignNullToNotNullAttribute*/, item == null);

                item.Should().BeNull();
            }
        }

        [Test]
        public void SomeIteratorWithItemNotNull()
        {
            foreach (var item in _instance.SomeIteratorWithItemNotNull())
            {
                ReSharper.TestValueAnalysis(item, item == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);

                item.Should().BeNull();
            }
        }
    }
}