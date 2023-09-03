namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class DefaultGetHashCodeTests
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
                        [MacroRecord(Options = RecordOptions.Default)]
                        [Field(typeof(byte[]), "Field", Options = FieldOptions.All)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.All)]
                        readonly partial struct TVO {}
                        """,
                        """
                        public override int GetHashCode() =>
                            (this.Field, this.Field2).GetHashCode();
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.Default)]
                        [Field(typeof(byte[]), "Field", Options = FieldOptions.All)]
                        readonly partial struct TVO {}
                        """,
                        """
                        public override int GetHashCode() =>
                            global::System.Collections.Generic.EqualityComparer<byte[]>.Default.GetHashCode(this.Field);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.Default)]
                        readonly partial struct TVO {}
                        """,
                        """
                        public override int GetHashCode() => 0;
                        """
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesEqualsCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentGetHashCodeMethod().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
