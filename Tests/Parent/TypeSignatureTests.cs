namespace ValueObjectGenerator.Tests.Parent
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
