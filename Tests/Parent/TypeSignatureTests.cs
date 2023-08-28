namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class TypeSignatureTests
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
                        partial class TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        sealed partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        abstract partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        internal partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        internal sealed partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        internal abstract partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        protected partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        protected sealed partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        protected abstract partial class TVO
                        {
                        }
                        """,
                        """
                        partial class TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        readonly partial struct TVO
                        {
                        }
                        """,
                        """
                        partial struct TVO : IEquatable<TVO>
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        partial struct TVO
                        {
                        }
                        """,
                        """
                        partial struct TVO : IEquatable<TVO>
                        """
                    }
                };
            }
        }
        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesTypeSignatureCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentTypeSignature()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
