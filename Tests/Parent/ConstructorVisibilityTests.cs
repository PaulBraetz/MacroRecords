namespace ValueObjectGenerator.Tests.Parent
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(ConstructorVisibility = GeneratedValueObjectAttribute.VisibilityModifier.Public)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(ConstructorVisibility = GeneratedValueObjectAttribute.VisibilityModifier.Protected)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(ConstructorVisibility = GeneratedValueObjectAttribute.VisibilityModifier.Internal)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(ConstructorVisibility = GeneratedValueObjectAttribute.VisibilityModifier.PrivateProtected)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(ConstructorVisibility = GeneratedValueObjectAttribute.VisibilityModifier.ProtectedInternal)]
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
