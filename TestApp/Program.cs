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
            Int32 age0 = Age.Create(4);
            Age age1 = (Age)age0;
            //equivalent expression:
            var age2 = Age.Create(age0);

            if(Age.TryCreate(5, out var age3))
            {
                //...
            }

            try
            {
                var age4 = Age.Create(-6);
            } catch(ArgumentException ex)
            {
                //...
            }

            try
            {
                var age5 = (Age)(-6);
            } catch(ArgumentException ex)
            {
                //...
            }
        }
    }
}