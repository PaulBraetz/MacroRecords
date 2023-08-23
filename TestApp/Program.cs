using RhoMicro.ValueObjectGenerator;

using System.IO;

namespace TestApp
{
    [GeneratedValueObject]
    [GenerateValueObjectField(typeof(String), "Street")]
    [GenerateValueObjectField(typeof(String), "City")]
    internal partial class Address
    {
        static partial void Validate(ValidateParameters parameters, ref ValidateResult result)
        {
            var (city, street) = parameters;
            if(String.IsNullOrEmpty(city))
            {
                result.CityIsInvalid = true;
                result.CityError = "A city may not be null or empty.";
            }
            if(String.IsNullOrEmpty(street))
            {
                result.StreetIsInvalid = true;
                result.StreetError = "A street must be provided (may not be null or empty).";
            }
            if(street == "Baker")
            {
                result.StreetIsInvalid = true;
                result.StreetError = "Baker Street is not real.";
            }
        }
    }

    internal partial class Program
    {
        static void Main(String[] _)
        {
            var address = Address.Create("Main", "London");
        }
    }
}