using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace MacroRecords.Tests.Parent
{
    [TestClass]
    public class ConstructorGenerateTests
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
                        partial class TVO
                        {
                        }
                        """,
                        """
                        private TVO(){}
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(GenerateConstructor = false)]
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
                        [MacroRecord(GenerateConstructor = true)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        private TVO()
                        {
                        }
                        """
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesConstructorCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentConstructor()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}