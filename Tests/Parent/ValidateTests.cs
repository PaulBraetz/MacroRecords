namespace ValueObjectGenerator.Tests.Parent
{
    [TestClass]
    public class ValidateMethodTests
    {

        private static Object[][] Data
        {
            get
            {
                return new Object[][]
                {
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(typeof(int), "Field1", GenerateOptions = FieldAttribute.Options.Validated)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(ValidateParameters parameters, ref ValidateResult result);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(typeof(int), "Field1", GenerateOptions = FieldAttribute.Options.Validated)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(ValidateParameters parameters, ref ValidateResult result);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(typeof(int), "Field1", GenerateOptions = FieldAttribute.Options.Validated)]
                        [Field(typeof(int), "Field2", GenerateOptions = FieldAttribute.Options.Validated)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(ValidateParameters parameters, ref ValidateResult result);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(typeof(int), "Field1", GenerateOptions = FieldAttribute.Options.Validated)]
                        [Field(typeof(int), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(ValidateParameters parameters, ref ValidateResult result);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(typeof(int), "Field1")]
                        [Field(typeof(int), "Field2", GenerateOptions = FieldAttribute.Options.Validated)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(ValidateParameters parameters, ref ValidateResult result);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(typeof(int), "Field1")]
                        [Field(typeof(int), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        String.Empty
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesValidateCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentValidateMethod()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
