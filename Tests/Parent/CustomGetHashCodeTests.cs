namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class CustomGetHashCodeTests
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
                        [Field(typeof(byte[]), "Field", Options = FieldOptions.All)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.All)]
                        readonly partial struct TVO {}
                        """,
                        """
                        partial void GetCustomHashCodes(ref CustomHashCodes hashCodes);
                        public override int GetHashCode()
                        {
                            var hashCodes = new CustomHashCodes();
                            GetCustomHashCodes(ref hashCodes);
                            var result = (
                            hashCodes.FieldHashCode ?? 
                            global::System.Collections.Generic.EqualityComparer<byte[]>.Default.GetHashCode(this.Field),
                            hashCodes.Field2HashCode ?? 
                            global::System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(this.Field2))
                            .GetHashCode(); 

                            return result;
                        }
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
