using RhoMicro.MacroRecords;

using System.Text.RegularExpressions;

namespace TestApp
{
    [MacroRecord]
    [Field(typeof(String), "_value", Visibility = Visibility.Private,
        Options = FieldOptions.Deconstructable | FieldOptions.Validated)]
    internal readonly partial struct Name
    {
        static partial void Validate(ValidateParameters parameters, ref ValidateResult result)
        {
            result._valueIsInvalid = Regex.IsMatch(parameters._value, @"[a-zA-Z0-9]+");
            result._valueError = "Name must be alphanumeric";
        }
    }

    record NameR(String Value1, Int32 Value2);

    internal partial class Program
    {
        static void Main(String[] _)
        {
        }
    }
}
