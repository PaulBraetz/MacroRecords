using RhoMicro.MacroRecords;

using System.Text.RegularExpressions;

namespace TestApp
{
    [MacroRecord()]
    [Field(typeof(String), "Value", Options = FieldOptions.Validated)]
    internal readonly partial struct Name
    {
        static partial void Validate(ValidateParameters parameters, ref ValidateResult result)
        {
            result.ValueIsInvalid = Regex.IsMatch(parameters.Value, @"[a-zA-Z0-9]+");
            result.ValueError = "Name must be alphanumeric";
        }
    }
    record NameR(String Value1, Int32 Value2);

    internal partial class Program
    {
        static void Main(String[] _)
        {
            var nr = new NameR("", 6);
            var n = Name.Create("");
        }
    }
}
