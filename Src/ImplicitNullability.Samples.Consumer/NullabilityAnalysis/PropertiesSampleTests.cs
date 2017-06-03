using System;
using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using NUnit.Framework;
using SP = ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis.PropertiesSample.StaticProperties;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class PropertiesSampleTests
    {
        private PropertiesSample.Properties _instance;

        [SetUp]
        public void SetUp()
        {
            _instance = new PropertiesSample.Properties();
        }

        [Test]
        public void AutoProperty_WithSetterWithNullValue()
        {
            Action act = () => _instance.AutoProperty = null /*Expect:AssignNullToNotNullAttribute[Prps && !RtGo]*/;

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [Test]
        public void AutoProperty_WithSetterWithNonNullValue()
        {
            Action act = () => _instance.AutoProperty = "";

            act.ShouldNotThrow();
        }

        [Test]
        public void AutoProperty_WithGetterReturningNullValue()
        {
            Action act = () =>
            {
                var value = _instance.AutoProperty;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        [Test]
        public void AutoProperty_WithGetterReturningNonNullValue()
        {
            _instance.AutoProperty = "";

            Action act = () =>
            {
                var value = _instance.AutoProperty;
                TestValueAnalysis(value, value == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-464147 */);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void AutoPropertyWithCanBeNull_WithSetter()
        {
            Action act = () => _instance.AutoPropertyWithCanBeNull = null /*Expect no warning*/;

            act.ShouldNotThrow();
        }

        [Test]
        public void AutoPropertyWithCanBeNull_WithGetter()
        {
            Action act = () =>
            {
                var value = _instance.AutoPropertyWithCanBeNull;
                TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);
            };

            act.ShouldNotThrow();
        }

        [Test]
        public void GetterOnly()
        {
            Action act = () =>
            {
                var value = _instance.GetterOnly;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        [Test]
        public void GetterOnlyWithCanBeNull()
        {
            Action act = () =>
            {
                var value = _instance.GetterOnlyWithCanBeNull;
                TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);
            };

            act.ShouldNotThrow();
        }

        //

        [Test]
        public void StaticAutoProperty_WithSetterWithNullValue()
        {
            Action act = () => SP.AutoProperty = null /*Expect:AssignNullToNotNullAttribute[Prps && !RtGo]*/;

            act.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [Test]
        public void StaticAutoProperty_WithSetterWithNonNullValue()
        {
            Action act = () => SP.AutoProperty = "";

            act.ShouldNotThrow();
        }

        [Test]
        public void StaticAutoProperty_WithGetterReturningNullValue()
        {
            Action act = () =>
            {
                var value = SP.AutoProperty;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        [Test]
        public void StaticAutoPropertyWithCanBeNull_WithSetter()
        {
            Action act = () => SP.AutoPropertyWithCanBeNull = null /*Expect no warning*/;

            act.ShouldNotThrow();
        }

        [Test]
        public void StaticAutoPropertyWithCanBeNull_WithGetter()
        {
            Action act = () =>
            {
                var value = SP.AutoPropertyWithCanBeNull;
                TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);
            };

            act.ShouldNotThrow();
        }

        //

        [Test]
        public void MutableClass()
        {
            var mutableClass = new PropertiesSample.MutableClass { Property = "value" };

            TestValueAnalysis(mutableClass.Property, mutableClass.Property == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);
            mutableClass.Property.Should().Be("value");

            TestValueAnalysis(mutableClass.DelegatingProperty,
                mutableClass.DelegatingProperty == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);
            mutableClass.DelegatingProperty.Should().Be("value");

            TestValueAnalysis(
                mutableClass.DelegatingGetterOnlyProperty,
                mutableClass.DelegatingGetterOnlyProperty == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);
            mutableClass.DelegatingGetterOnlyProperty.Should().Be("value");
        }

        [Test]
        public void MutableClassWithNullInitialization()
        {
            Func<object> act = () => new PropertiesSample.MutableClass { Property = null /*Expect:AssignNullToNotNullAttribute[Prps && !RtGo]*/ };

            act.ToAction().ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("value");
        }

        [Test]
        public void MutableClassWithInitialState()
        {
            // Here the implicit NotNull is wrong, but we have the NotNullMemberIsNotInitialized warning at declaration site and
            // the NullGuard check at the property getter.

            var mutableClass = new PropertiesSample.MutableClass();

            Action act = () =>
            {
                var value = mutableClass.Property;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        //

        [Test]
        public void ImmutableClass()
        {
            var c = new PropertiesSample.ImmutableClass(value: "value");

            TestValueAnalysis(c.Property, c.Property == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);
            c.Property.Should().Be("value");

            TestValueAnalysis(c.AutoPropertyWithPrivateSetter, c.AutoPropertyWithPrivateSetter == null
                /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && (!RtGo || (External && RS <= 20171))]*/);
            c.AutoPropertyWithPrivateSetter.Should().Be("value");

            TestValueAnalysis(c.DelegatingProperty, c.DelegatingProperty == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);
            c.DelegatingProperty.Should().Be("value");

            TestValueAnalysis(c.NullableProperty /*Expect:AssignNullToNotNullAttribute*/, c.NullableProperty == null);
            c.NullableProperty.Should().BeNull();

            // Here the implicit NotNull is wrong, because "UnknownNullabilityString" returns null:
            c.Invoking(x => TestValueAnalysis(
                    x.PropertyWithUnknownValue, x.PropertyWithUnknownValue == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/))
                .ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        [Test]
        public void ImmutableClassWithBadCtor()
        {
            var immutableClass = new PropertiesSample.ImmutableClass();

            // Here the implicit NotNull is wrong, but we have the NotNullMemberIsNotInitialized warning at the bad c'tor and
            // the NullGuard check at the property getter.
            Action act = () =>
            {
                var value = immutableClass.Property;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        //

        [Test]
        public void ImmutableClassWithNullInitialization()
        {
            // The property initialization is not checked by NullGuard, only the property getter.

            var immutableClass = new PropertiesSample.ImmutableClassWithNullInitialization();

            Action act = () =>
            {
                var value = immutableClass.Property;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);
            };
            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        //

        [Test]
        public void MutableStructAndDefaultCtor()
        {
            var @struct = new PropertiesSample.MutableStruct();

            // Here the implicit NotNull is wrong, but we have the NullGuard check at the property getter:
            Action act = () =>
            {
                var value = @struct.Property;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !(RtGo || RtRT)]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }

        //

        [Test]
        public void ImmutableStruct()
        {
            var @struct = new PropertiesSample.ImmutableStruct(property: "value");

            TestValueAnalysis(@struct.Property, @struct.Property == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtRT]*/);
            @struct.Property.Should().Be("value");
        }

        [Test]
        public void ImmutableStructAndDefaultCtor()
        {
            var @struct = new PropertiesSample.ImmutableStruct();

            // Here the implicit NotNull is wrong, but we have the NullGuard check at the property getter:
            Action act = () =>
            {
                var value = @struct.Property;
                TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtRT]*/);
            };

            act.ShouldThrow<InvalidOperationException>().WithMessage("[NullGuard] Return value * is null.");
        }
    }
}
