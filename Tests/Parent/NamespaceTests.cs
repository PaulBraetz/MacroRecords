namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class NamespaceTests
    {
        private static Object[][] OpenData
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
                        String.Empty
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        partial record struct TVOP
                        {
                            [MacroRecord]
                            partial class TVO
                            {
                            }
                        }
                        """,
                        """"
                        partial record struct TVOP{
                        """"
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        partial class TVOP
                        {
                            [MacroRecord]
                            partial class TVO
                            {
                            }
                        }
                        """,
                        """
                        partial class TVOP{
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        namespace TestNamespace;
                        [MacroRecord]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        namespace TestNamespace{
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        namespace ParentNamespace.TestNamespace;
                        [MacroRecord]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        namespace ParentNamespace.TestNamespace{
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        namespace TestNamespace
                        {
                        [MacroRecord]
                        partial class TVO
                        {
                        }
                        }
                        """,
                        """
                        namespace TestNamespace{
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        namespace ParentNamespace.TestNamespace
                        {
                        [MacroRecord]
                        partial class TVO
                        {
                        }
                        }
                        """,
                        """
                        namespace ParentNamespace.TestNamespace{
                        """
                    }
                };
            }
        }

        private static Object[][] CloseData
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
                        String.Empty
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        namespace TestNamespace
                        {
                        [MacroRecord]
                        partial class TVO
                        {
                        }
                        }
                        """,
                        """
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        namespace TestNamespace;
                        [MacroRecord]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        }
                        """
                    }
                };
            }
        }
        [TestMethod]
        [DynamicData(nameof(OpenData))]
        public void GeneratesNamespaceOpenCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentOpen().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
        [TestMethod]
        [DynamicData(nameof(CloseData))]
        public void GeneratesNamespaceCloseCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentClose().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
