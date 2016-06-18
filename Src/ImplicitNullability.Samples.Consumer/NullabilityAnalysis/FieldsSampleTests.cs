using FluentAssertions;
using ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis;
using ImplicitNullability.Samples.CodeWithoutIN;
using NUnit.Framework;
using SF = ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis.FieldsSample.StaticFields;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

namespace ImplicitNullability.Samples.Consumer.NullabilityAnalysis
{
    [TestFixture]
    public class FieldsSampleTests
    {
        [Test]
        public void PublicFieldsAccess()
        {
            var f = new FieldsSample.Fields();

            // Here the implicit NotNull is potentially wrong:
            TestValueAnalysis(f.Field, f.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            TestValueAnalysis(f.NullableField /*Expect:AssignNullToNotNullAttribute*/, f.NullableField == null);

            TestValueAnalysis(f.UnassignedReadonlyField, f.UnassignedReadonlyField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            TestValueAnalysis(f.ReadonlyField, f.ReadonlyField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            TestValueAnalysis(f.NullableReadonlyField /*Expect:AssignNullToNotNullAttribute*/, f.NullableReadonlyField == null);
        }

        [Test]
        public void PublicStaticFieldsAccess()
        {
            // Here the implicit NotNull is potentially wrong:
            TestValueAnalysis(SF.Field, SF.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            TestValueAnalysis(SF.NullableField /*Expect:AssignNullToNotNullAttribute*/, SF.NullableField == null);

            TestValueAnalysis(SF.UnassignedReadonlyField, SF.UnassignedReadonlyField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            TestValueAnalysis(SF.ReadonlyField, SF.ReadonlyField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            TestValueAnalysis(SF.NullableReadonlyField /*Expect:AssignNullToNotNullAttribute*/, SF.NullableReadonlyField == null);
        }

        [Test]
        public void MutableClassWithImplicitNotNull()
        {
            var mutableClass = new FieldsSample.MutableClass { Field = "" /*Expect no warning*/ };

            TestValueAnalysis(mutableClass.Field, mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            mutableClass.Field.Should().NotBeNull();
        }

        [Test]
        public void MutableClassWithImplicitNotNullAndNullAssignment()
        {
            var mutableClass = new FieldsSample.MutableClass { Field = null /*Expect:AssignNullToNotNullAttribute[Flds && !RtRo]*/ };

            TestValueAnalysis(mutableClass.Field, mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            mutableClass.Field.Should().BeNull();
        }

        [Test]
        public void MutableClassWithImplicitNotNullAndInitialState()
        {
            // Here the implicit NotNull is wrong, but we have the NotNullMemberIsNotInitialized warning at declaration site.

            var mutableClass = new FieldsSample.MutableClass();

            TestValueAnalysis(mutableClass.Field, mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            mutableClass.Field.Should().BeNull();
        }

        [Test]
        public void MutableClassWithImplicitNotNullAndUnknownNullabilityAssignment()
        {
            // Here the implicit NotNull is potentially wrong.

            var mutableClass = new FieldsSample.MutableClass { Field = External.UnknownNullabilityString };

            TestValueAnalysis(mutableClass.Field, mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            mutableClass.Field.Should().BeNull();
        }

        [Test]
        public void ImmutableClass()
        {
            var instance = new FieldsSample.ImmutableClass(value: "field value");

            TestValueAnalysis(instance.Field, instance.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            instance.Field.Should().Be("field value");

            TestValueAnalysis(instance.NullableField /*Expect:AssignNullToNotNullAttribute*/, instance.NullableField == null);
            instance.NullableField.Should().BeNull();

            // Here the implicit NotNull is wrong, because "UnknownNullabilityString" returns null:
            TestValueAnalysis(instance.FieldWithUnknownValue, instance.FieldWithUnknownValue == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
        }

        [Test]
        public void ImmutableStruct()
        {
            // For field read accesses in value types, R# doesn't use the NotNull.

            var @struct = new FieldsSample.ImmutableStruct(value: "field value");

            TestValueAnalysis(@struct.Field, @struct.Field == null);
            @struct.Field.Should().Be("field value");
        }

        [Test]
        public void ImmutableStructAndDefaultCtor()
        {
            // For field read accesses in value types, R# doesn't use the NotNull.

            var @struct = new FieldsSample.ImmutableStruct();

            TestValueAnalysis(@struct.Field, @struct.Field == null);
            @struct.Field.Should().BeNull();
        }

        [Test]
        public void MutableStruct()
        {
            var @struct = new FieldsSample.MutableStruct();

            TestValueAnalysis(@struct.Field, @struct.Field == null);

            @struct.Field = null /*Expect:AssignNullToNotNullAttribute[Flds && !(RtRo || RtRT)]*/;
        }
    }
}
