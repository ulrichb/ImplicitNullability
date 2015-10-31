using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.Highlighting.NotNullOnImplicitCanBeNull;
using NUnit.Framework;

namespace ImplicitNullability.Samples.Consumer.Highlighting.NotNullOnImplicitCanBeNull
{
    [TestFixture]
    public class NotNullOnImplicitCanBeNullSampleTests
    {
        private NotNullOnImplicitCanBeNullSample _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new NotNullOnImplicitCanBeNullSample();
        }

        [Test]
        public void MethodWithNullableInt()
        {
            Action act = () => _instance.MethodWithNullableInt(null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow("the [NotNull] won't be respected by NullGuard => warning");
        }

        [Test]
        public void MethodWithOptionalParameter()
        {
            Action act = () => _instance.MethodWithOptionalParameter(optional: null /*Expect:AssignNullToNotNullAttribute*/);

            act.ShouldNotThrow("the [NotNull] won't be respected by NullGuard => warning");
        }
    }
}