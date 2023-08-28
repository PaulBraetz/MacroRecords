using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class FieldTests
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
                        [Field(typeof(System.IComparable), "Field1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public readonly System.IComparable Field1;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(ITestInterface), "Field")]
                        partial struct TVO { }

                        interface ITestInterface
                        {
                            public String Value { get; set; }
                        }
                        """,
                        """
                        public readonly ITestInterface Field;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(TestNamespace.ITestInterface), "Field")]
                        partial struct TVO { }

                        namespace TestNamespace
                        {                        
                            interface ITestInterface
                            {
                                public String Value { get; set; }
                            }
                        }
                        """,
                        """
                        public readonly TestNamespace.ITestInterface Field;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1"),
                        Field(typeof(System.IComparable), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public readonly System.Int32 Field1;
                        public readonly System.IComparable Field2;
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
                        public readonly System.Int32 Field1;
                        public readonly System.String Field2;
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
                        String.Empty
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
                        public readonly System.Object ObjectField;
                        public readonly System.String StringField;
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
            var actual = builder.AddParentFields().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
