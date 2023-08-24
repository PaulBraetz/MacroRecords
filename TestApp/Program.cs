using RhoMicro.ValueObjectGenerator;

using System.IO;

namespace TestApp
{
    [GeneratedValueObject]
    [GenerateValueObjectField(
        typeof(String),
        "_value",
        Visibility = GenerateValueObjectFieldAttribute.VisibilityModifier.Private,
        GenerateOptions = GenerateValueObjectFieldAttribute.Options.Validated)]
    internal readonly partial struct Name
    {
        static partial void Validate(ValidateParameters parameters, ref ValidateResult result)
        {
        }
    }

    record T(String Value)
    {
        public T(String value)
        {
            if(value == String.Empty)
            {
                throw new ArgumentException();
            }
        }
    }

    internal partial class Program
    {
        static void Main(String[] _)
        {
            var a = Name.Create("Value1");
            String b = a;
        }
    }
}