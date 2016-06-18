using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Samples.CodeWithIN.Highlighting.IncorrectNullableAttributeUsageAnalyzer
{
    public static class OtherWarningsSample
    {
        // Test that our problem analyzer, which hides IncorrectNullableAttributeUsageAnalyzer still passes the remaining warnings.

        public interface IInterface
        {
            void ExplicitCanBeNull([CanBeNull] string a);
        }

        public class Implementation : IInterface
        {
            public void ExplicitCanBeNull([NotNull] /*Expect:AnnotationConflictInHierarchy*/ string a)
            {
            }

            [NotNull] /*Expect:AnnotationRedundancyAtValueType*/
            public void AnnotationOnVoid()
            {
            }

            [NotNull] /*Expect:AnnotationRedundancyAtValueType*/
            public int AnnotationOnStruct() => 0;

            [ItemNotNull] /*Expect:ContainerAnnotationRedundancy[RS >= 20161]*/
            public object NonAsyncMethod() => "";

            [NotNull, CanBeNull]
            public string MultipleAnnotations /*Expect:MultipleNullableAttributesUsage*/() => "";
        }
    }
}