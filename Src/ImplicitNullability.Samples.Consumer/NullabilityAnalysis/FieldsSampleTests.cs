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

        //

        [Test]
        public void MutableClass()
        {
            var mutableClass = new FieldsSample.MutableClass { Field = "value" };

            TestValueAnalysis(mutableClass.Field, mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            mutableClass.Field.Should().Be("value");
        }

        [Test]
        public void MutableClassWithNullInitialization()
        {
            var mutableClass = new FieldsSample.MutableClass { Field = null /*Expect:AssignNullToNotNullAttribute[Flds && !RtRo]*/ };

            // The null assignment above overwrites the implicit NotNull with 'NULL' (since R# 2018.2):
            TestValueAnalysis(
                mutableClass.Field /*Expect:AssignNullToNotNullAttribute[RS > 20181]*/,
                mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            mutableClass.Field.Should().BeNull();
        }

        [Test]
        public void MutableClassWithInitialState()
        {
            // Here the implicit NotNull is wrong, but we have the NotNullMemberIsNotInitialized warning at declaration site.

            var mutableClass = new FieldsSample.MutableClass();

            TestValueAnalysis(mutableClass.Field, mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            mutableClass.Field.Should().BeNull();
        }

        [Test]
        public void MutableClassWithUnknownNullabilityInitialization()
        {
            // Here the implicit NotNull is wrong, but we have the NotNullMemberIsNotInitialized warning at declaration site.

            var mutableClass = new FieldsSample.MutableClass { Field = External.UnknownNullabilityString };

            // The assignment above overwrites the implicit NotNull with 'UNKNOWN' (since R# 2018.2):
            TestValueAnalysis(mutableClass.Field, mutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo && RS <= 20181]*/);
            mutableClass.Field.Should().BeNull();
        }

        //

        [Test]
        public void ImmutableClass()
        {
            var c = new FieldsSample.ImmutableClass(value: "value");

            TestValueAnalysis(c.Field, c.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            c.Field.Should().Be("value");

            TestValueAnalysis(c.NullableField /*Expect:AssignNullToNotNullAttribute*/, c.NullableField == null);
            c.NullableField.Should().BeNull();

            // Here the implicit NotNull is wrong, because "UnknownNullabilityString" returned null:
            TestValueAnalysis(c.FieldWithUnknownValue, c.FieldWithUnknownValue == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            c.FieldWithUnknownValue.Should().BeNull();
        }

        [Test]
        public void ImmutableClassWithBadCtor()
        {
            var immutableClass = new FieldsSample.ImmutableClass();

            // Here the implicit NotNull is wrong, but we have the NotNullMemberIsNotInitialized warning at the bad c'tor.
            TestValueAnalysis(immutableClass.Field, immutableClass.Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            immutableClass.Field.Should().BeNull();
        }

        //

        [Test]
        public void MutableStructAndDefaultCtor()
        {
            var @struct = new FieldsSample.MutableStruct();

            // For field read accesses in value types, R# doesn't use the NotNull:
            TestValueAnalysis(@struct.Field, @struct.Field == null);

            @struct.Field = null /*Expect:AssignNullToNotNullAttribute[Flds && !(RtRo || RtRT)]*/;
        }

        //

        [Test]
        public void ImmutableStruct()
        {
            var @struct = new FieldsSample.ImmutableStruct(field: "value");

            // For field read accesses in value types, R# doesn't use the NotNull:
            TestValueAnalysis(@struct.Field, @struct.Field == null);
            @struct.Field.Should().Be("value");
        }

        [Test]
        public void ImmutableStructAndDefaultCtor()
        {
            var @struct = new FieldsSample.ImmutableStruct();

            // For field read accesses in value types, R# doesn't use the NotNull:
            TestValueAnalysis(@struct.Field, @struct.Field == null);
            @struct.Field.Should().BeNull();
        }
    }
}
