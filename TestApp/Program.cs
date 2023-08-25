using RhoMicro.ValueObjectGenerator;

using System.IO;

namespace TestApp
{
    [GeneratedValueObject(ConstructorVisibility = GeneratedValueObjectAttribute.VisibilityModifier.Protected),
    Field(typeof(String), "Value1"),
    Field(typeof(Int32), "Value2")]
    internal partial class Name
    {
    }

    internal partial class Program
    {
        static void Main(String[] _)
        {
        }
    }
}

