using System;
using JetBrains.Annotations;

namespace ImplicitNullability.Sample.NullabilityAnalysis
{
  public class PropertiesAndFieldsSample
  {
    public string SomeField;

    [CanBeNull]
    public string SomeCanBeNullField;

    public string SomeProperty
    {
      get { return null; }
      set { ReSharper.TestValueAnalysis (value, value == null); }
    }
    
    public string SomeAutoProperty { get; set; }

    [CanBeNull]
    public string SomeCanBeNullProperty { get; set; }
  }
}