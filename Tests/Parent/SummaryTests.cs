namespace MacroRecords.Tests.Parent
{
    [TestClass]
    public class SummaryTests
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
                        [Field(typeof(object), "ObjField", Summary = "Testsummary")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        /// <summary>
                        /// Testsummary
                        /// </summary>
                        public readonly System.Object ObjField;
                        """
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesSummaryCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentFields().BuildCore();

            //Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
