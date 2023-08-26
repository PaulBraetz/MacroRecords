namespace MacroRecords.Tests.Parent
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
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Validated)]
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
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Validated)]
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
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Validated)]
                        [Field(typeof(int), "Field2", Options = FieldOptions.Validated)]
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
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Validated)]
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
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1")]
                        [Field(typeof(int), "Field2", Options = FieldOptions.Validated)]
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
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1")]
                        [Field(typeof(int), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        String.Empty
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(ConstructorVisibility = Visibility.Protected),
                        Field(typeof(String), "Value1", Options = FieldOptions.Validated),
                        Field(typeof(Int32), "Value2")]
                        internal partial class Name
                        {
                        }
                        """,
                        """

                        """
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
