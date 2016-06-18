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

            internal string InternalField = "";
            protected string ProtectedField = "";
            private string _privateField = "";

            public void ConsumeNonPublicMembers()
            {
                TestValueAnalysis(InternalField, InternalField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
                TestValueAnalysis(ProtectedField, ProtectedField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
                TestValueAnalysis(_privateField, _privateField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            }
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

            internal static string InternalField = "";
            protected static string ProtectedField = "";
            private static string _privateField = "";

            public static void ConsumeNonPublicMembers()
            {
                TestValueAnalysis(InternalField, InternalField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
                TestValueAnalysis(ProtectedField, ProtectedField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
                TestValueAnalysis(_privateField, _privateField == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRo]*/);
            }
        }

        public class MutableClass
        {
            public string Field /*Expect:NotNullMemberIsNotInitialized[Flds && !RtRo]*/;
        }

        public class ImmutableClass
        {
            public readonly string Field;
            public readonly string Field2;

            [CanBeNull]
            public readonly string NullableField;

            public readonly string FieldWithUnknownValue;

            public ImmutableClass(string value, string nullableValue = null)
            {
                Field = value;
                Field2 = nullableValue /*Expect:AssignNullToNotNullAttribute[Flds]*/;
                NullableField = nullableValue;
                FieldWithUnknownValue = External.UnknownNullabilityString;
            }

            public ImmutableClass /*Expect:NotNullMemberIsNotInitialized[Flds]*/()
            {
                // REPORTED (https://youtrack.jetbrains.com/issue/RSRP-460405): The NotNull is used before the first assignment.
                TestValueAnalysis(Field, Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds]*/);

                Field2 = "";
            }
        }

        public struct ImmutableStruct
        {
            public readonly string Field;
            public readonly string Field2;

            public ImmutableStruct(string value, string nullableValue = null)
            {
                Field = value;
                Field2 = nullableValue /*Expect:AssignNullToNotNullAttribute[Flds && !RtRT]*/;
            }

            public void ConsumeWithinStruct()
            {
                // REPORTED false positive https://youtrack.jetbrains.com/issue/RSRP-462952
                TestValueAnalysis(Field, Field == null /*Expect:ConditionIsAlwaysTrueOrFalse[Flds && !RtRT]*/);
            }
        }

        public struct MutableStruct
        {
            public string Field;

            public MutableStruct(string nullableValue = null)
            {
                Field = nullableValue /*Expect:AssignNullToNotNullAttribute[Flds && !(RtRo || RtRT)]*/;
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
