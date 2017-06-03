using ImplicitNullability.Samples.CodeWithoutIN;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

// ReSharper disable MemberCanBePrivate.Global

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public static class FieldsSample
    {
        public class Fields
        {
            public string Field /*Expect:NotNullMemberIsNotInitialized[Flds && !RtRo]*/;

            [CanBeNull]
            public string NullableField = null;

            public readonly string UnassignedReadonlyField /*Expect:UnassignedReadonlyField*/;

            public readonly string ReadonlyField = "";

            public readonly string ReadonlyFieldWithNullAssignment = null /*Expect:AssignNullToNotNullAttribute[Flds]*/;

            [CanBeNull]
            public readonly string NullableReadonlyField = null;
        }

        public class StaticFields
        {
            public static string Field /*Expect:NotNullMemberIsNotInitialized[Flds && !RtRo]*/;

            [CanBeNull]
            public static string NullableField = null;

            public static readonly string UnassignedReadonlyField /*Expect:UnassignedReadonlyField*/;

            public static readonly string ReadonlyField = "";

            public static readonly string ReadonlyFieldWithNullAssignment = null /*Expect:AssignNullToNotNullAttribute[Flds]*/;

            [CanBeNull]
            public static readonly string NullableReadonlyField = null;
        }

        public class MutableClass
        {
            public string Field /*Expect:NotNullMemberIsNotInitialized[Flds && !RtRo]*/;
        }

        public class ImmutableClass
        {
            public readonly string Field;

            [CanBeNull]
            public readonly string NullableField;

            public readonly string FieldWithUnknownValue;

            public ImmutableClass(string value, string nullableValue = null)
            {
                Field = value;
                NullableField = nullableValue;
                FieldWithUnknownValue = External.UnknownNullabilityString;
            }

            // Bad c'tor:
            public ImmutableClass /*Expect:NotNullMemberIsNotInitialized[Flds]*/()
            {
            }
        }

        public class ImmutableClassWithUseBeforeAssignment
        {
            public readonly string Field;

            public ImmutableClassWithUseBeforeAssignment /*Expect:NotNullMemberIsNotInitialized[Flds]*/()
            {
                // This is wrong, but R# instead emits the NotNullMemberIsNotInitialized on the c'tor, which is good enough:
                // REPORTED: https://youtrack.jetbrains.com/issue/RSRP-460405
                TestValueAnalysis(Field, Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);

                Field = "";
            }
        }

        public class ImmutableClassWithUseAfterAssignment
        {
            public readonly string Field;

            public ImmutableClassWithUseAfterAssignment()
            {
                Field = "";
                TestValueAnalysis(Field, Field == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-464147 */);
            }

            public void Consume()
            {
                TestValueAnalysis(Field, Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);
            }
        }

        public struct MutableStruct
        {
            public string Field;

            public MutableStruct(string field)
            {
                Field = field;
            }
        }

        public struct ImmutableStruct
        {
            public readonly string Field;

            public ImmutableStruct(string field)
            {
                Field = field;
            }

            public ImmutableStruct(int i)
            {
                SuppressUnusedWarning(i);

                Field = null /*Expect:AssignNullToNotNullAttribute[Flds && !RtRT]*/;
            }

            public void ConsumeWithinStruct()
            {
                // REPORTED false positive https://youtrack.jetbrains.com/issue/RSRP-462952
                TestValueAnalysis(Field, Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRT]*/);
            }
        }

        public class FieldsDerived : Fields
        {
            // Prove that nullability is not inherited

            [CanBeNull]
            public new string Field = null /*Expect no warning*/;

            public new string NullableField = null /*Expect:AssignNullToNotNullAttribute[Flds && !RtRo]*/;
        }
    }
}
