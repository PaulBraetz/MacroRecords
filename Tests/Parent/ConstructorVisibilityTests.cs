namespace MacroRecords.Tests.Parent
{
    [TestClass]
    public class ConstructorVisibilityTests
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
                        /// <summary>
                        /// Initializes a new instance.
                        /// </summary>
                        private TVO(){}
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(ConstructorVisibility = Visibility.Public)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        /// <summary>
                        /// Initializes a new instance.
                        /// </summary>
                        public TVO(){}
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(ConstructorVisibility = Visibility.Protected)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        /// <summary>
                        /// Initializes a new instance.
                        /// </summary>
                        protected TVO(){}
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(ConstructorVisibility = Visibility.Internal)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        /// <summary>
                        /// Initializes a new instance.
                        /// </summary>
                        internal TVO(){}
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(ConstructorVisibility = Visibility.PrivateProtected)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        /// <summary>
                        /// Initializes a new instance.
                        /// </summary>
                        private protected TVO(){}
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(ConstructorVisibility = Visibility.ProtectedInternal)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        /// <summary>
                        /// Initializes a new instance.
                        /// </summary>
                        protected internal TVO(){}
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
            Assert.AreEqual(expected, actual);
        }
    }
}
