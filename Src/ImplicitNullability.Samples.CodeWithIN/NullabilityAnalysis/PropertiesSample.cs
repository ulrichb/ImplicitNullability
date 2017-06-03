using ImplicitNullability.Samples.CodeWithoutIN;
using JetBrains.Annotations;
using static ImplicitNullability.Samples.CodeWithIN.ReSharper;

// ReSharper disable EmptyConstructor
// ReSharper disable ArrangeAccessorOwnerBody

namespace ImplicitNullability.Samples.CodeWithIN.NullabilityAnalysis
{
    public static class PropertiesSample
    {
        public class Properties
        {
            public string AutoProperty /*Expect:NotNullMemberIsNotInitialized[Prps && !RtGo]*/ { get; set; }

            [CanBeNull]
            public string AutoPropertyWithCanBeNull { get; set; }

            //

            public string GetterOnly
            {
                get { return null /*Expect:AssignNullToNotNullAttribute[Prps]*/; }
            }

            [CanBeNull]
            public string GetterOnlyWithCanBeNull
            {
                get { return null; }
            }

            public string GetterOnlyExpression => null /*Expect:AssignNullToNotNullAttribute[Prps]*/;

            [CanBeNull]
            public string GetterOnlyExpressionWithCanBeNull => null;

            public string GetterOnlyWithInitializer { get; } = null /*Expect:AssignNullToNotNullAttribute[Prps]*/;

            [CanBeNull]
            public string GetterOnlyWithInitializerWithCanBeNull { get; } = null;

            //

            public string SetterOnly
            {
                set { TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/); }
            }

            [CanBeNull]
            public string SetterOnlyWithCanBeNull
            {
                set { TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null); }
            }

            //

            public string PropertyWithGetterAndSetterExpression
            {
                get => null /*Expect:AssignNullToNotNullAttribute[Prps && !RtGo]*/;
                set => TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps && !RtGo]*/);
            }

            [CanBeNull]
            public string PropertyWithGetterAndSetterExpressionWithCanBeNull
            {
                get => null;
                set => TestValueAnalysis(value /*Expect:AssignNullToNotNullAttribute*/, value == null);
            }
        }

        public class StaticProperties
        {
            public static string AutoProperty /*Expect:NotNullMemberIsNotInitialized[Prps && !RtGo]*/ { get; set; }

            [CanBeNull]
            public static string AutoPropertyWithCanBeNull { get; set; }

            //

            public string GetterOnly
            {
                get { return null /*Expect:AssignNullToNotNullAttribute[Prps]*/; }
            }

            [CanBeNull]
            public string GetterOnlyWithCanBeNull
            {
                get { return null; }
            }
        }

        public class GetterOrSetterAttributeAttributes
        {
            // Prove that the contract attributes at getters/setters are ignored.

            [NotNull]
            public string Property
            {
                [CanBeNull]
                get { return null /*Expect:AssignNullToNotNullAttribute*/; }
                [CanBeNull]
                set { TestValueAnalysis(value, value == null /*Expect:ConditionIsAlwaysTrueOrFalse*/); }
            }
        }

        public class MutableClass
        {
            public string Property /*Expect:NotNullMemberIsNotInitialized[Prps && !RtGo]*/ { get; set; }

            public string DelegatingProperty
            {
                get { return Property; }
                set { Property = value; }
            }

            public string DelegatingGetterOnlyProperty
            {
                get { return Property; }
            }
        }

        public class ImmutableClass
        {
            public string Property { get; }

            // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
            public string AutoPropertyWithPrivateSetter { get; private set; }

            public string DelegatingProperty
            {
                get { return Property; }
            }

            [CanBeNull]
            public string NullableProperty { get; }

            public string PropertyWithUnknownValue { get; }

            public ImmutableClass(string value, string nullableValue = null)
            {
                Property = value;
                AutoPropertyWithPrivateSetter = value;
                NullableProperty = nullableValue;
                PropertyWithUnknownValue = External.UnknownNullabilityString;
            }

            // Bad c'tor:
            public ImmutableClass /*Expect:NotNullMemberIsNotInitialized[Prps]*/()
            {
            }
        }

        public class ImmutableClassWithNullInitialization
        {
            public string Property { get; }

            public ImmutableClassWithNullInitialization()
            {
                Property = null /*Expect:AssignNullToNotNullAttribute[Prps]*/;
            }
        }

        public class ImmutableClassWithUseBeforeAssignment
        {
            public string Property { get; }

            public ImmutableClassWithUseBeforeAssignment /*Expect:NotNullMemberIsNotInitialized[Prps]*/()
            {
                // This is wrong, but R# instead emits the NotNullMemberIsNotInitialized on the c'tor, which is good enough:
                // REPORTED: https://youtrack.jetbrains.com/issue/RSRP-460405
                TestValueAnalysis(Property, Property == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);

                Property = "";
            }
        }

        public class ImmutableClassWithUseAfterAssignment
        {
            public string Property { get; }

            public ImmutableClassWithUseAfterAssignment()
            {
                Property = "";
                TestValueAnalysis(Property, Property == null /* REPORTED false negative https://youtrack.jetbrains.com/issue/RSRP-464147 */);
            }

            public void Consume()
            {
                TestValueAnalysis(Property, Property == null /*Expect:ConditionIsAlwaysTrueOrFalse[Prps]*/);
            }
        }

        public static class PropertiesInClassWithCtor
        {
            // Regression samples for issue #10 (exclusion for backing fields)

            public class AutoPropertySample
            {
                public string AutoProperty { get; set; }

                public AutoPropertySample /*Expect:NotNullMemberIsNotInitialized[Prps && !RtGo]*/ /* and only for properties, not for fields!*/()
                {
                }
            }

            public class GetterOnlySample
            {
                public string GetterOnlyProperty { get; }

                public GetterOnlySample /*Expect no warning*/()
                {
                    GetterOnlyProperty = "";
                }
            }
        }

        public struct MutableStruct
        {
            public string Property { get; set; }

            public MutableStruct(string property)
            {
                Property = property;
            }
        }

        public struct ImmutableStruct
        {
            public string Property { get; }

            public ImmutableStruct(string property)
            {
                Property = property;
            }

            public ImmutableStruct(int i)
            {
                SuppressUnusedWarning(i);

                Property = null /*Expect:AssignNullToNotNullAttribute[Prps && !RtRT]*/;
            }
        }
    }
}
