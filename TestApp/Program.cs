namespace TestApp
{
    [RhoMicro.ValueObjectGenerator.GeneratedValueObject(
        typeof(Int32), 
        ValueSpecification = "Ages must be nonzero positive integers.")]
    readonly partial struct Age
    {
        public static partial Boolean IsValid(Int32 value) => value > 0;
    }
    internal partial class Program
    {
        static void Main(String[] _)
        {
            var age1 = Age.Create(4);
            try
            {
                var age2 = Age.Create(0);
            } catch(Exception ex)
            {

            }
            try
            {
                var age3 = Age.Create(-4);
            } catch(Exception ex)
            {

            }
        }
    }
}