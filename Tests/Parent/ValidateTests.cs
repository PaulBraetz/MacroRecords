namespace RhoMicro.MacroRecords.Tests.Parent
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
                        [MacroRecord(Options = RecordOptions.All)]
                        [Field(typeof(int), "Field1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(in ValidateParameters parameters, ref ValidateResult result);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.Validated)]
                        [Field(typeof(int), "Field1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(in ValidateParameters parameters, ref ValidateResult result);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.Validated)]
                        [Field(typeof(int), "Field1")]
                        [Field(typeof(int), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        static partial void Validate(in ValidateParameters parameters, ref ValidateResult result);
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
