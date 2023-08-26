namespace MacroRecords.Tests.Parent
{
    [TestClass]
    public class ConstructorParametersTests
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
                        [Field(typeof(int), "Field1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        private TVO(System.Int32 in_Field1)
                        {
                            Field1=in_Field1;
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1"),
                        Field(typeof(int), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        private TVO(System.Int32 in_Field1, System.Int32 in_Field2)
                        {
                            Field1=in_Field1;
                            Field2=in_Field2;
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1"),
                        Field(typeof(System.String), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        private TVO(System.Int32 in_Field1, System.String in_Field2)
                        {
                            Field1=in_Field1;
                            Field2=in_Field2;
                        }
                        """
                    },
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
                        private TVO()
                        {
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(object), "ObjectField"),
                        Field(typeof(string), "StringField")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        private TVO(System.Object in_ObjectField, System.String in_StringField)
                        {
                            ObjectField = in_ObjectField;
                            StringField = in_StringField;
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
