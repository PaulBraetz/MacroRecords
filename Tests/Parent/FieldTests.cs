using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                        public readonly global::System.IComparable Field1;
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
                        public readonly global::ITestInterface Field;
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
                        public readonly global::TestNamespace.ITestInterface Field;
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
                        public readonly int Field1;
                        public readonly global::System.IComparable Field2;
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
                        public readonly int Field1;
                        public readonly string Field2;
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
                        public readonly object ObjectField;
                        public readonly string StringField;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field"),
                        Field(typeof(object), "Field"),
                        Field(typeof(string), "Field")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public readonly int Field;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(object), "Field"),
                        Field(typeof(string), "Field")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public readonly object Field;
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
