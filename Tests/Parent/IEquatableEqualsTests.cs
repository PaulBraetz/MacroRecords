using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class IEquatableEqualsTests
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
                        [Field(typeof(object), "ObjField", 
                            Options = FieldOptions.CustomEquality)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjFieldEquals(other.ObjField);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField", 
                            Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjFieldEquals(other.ObjField);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField, other.ObjField);
                        """
                    },

                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField", 
                            Options = FieldOptions.CustomEquality)]
                        [Field(typeof(object), "ObjField1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjFieldEquals(other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField", 
                            Options = FieldOptions.All)]
                        [Field(typeof(object), "ObjField1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjFieldEquals(other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        [Field(typeof(object), "ObjField1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField, other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1);
                        """
                    },

                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField", 
                            Options = FieldOptions.CustomEquality)]
                        [Field(typeof(object), "ObjField1")]
                        [Field(typeof(object), "ObjField2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjFieldEquals(other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField2, other.ObjField2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        [Field(typeof(object), "ObjField1", 
                            Options = FieldOptions.CustomEquality)]
                        [Field(typeof(object), "ObjField2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjField1Equals(other.ObjField1) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField, other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField2, other.ObjField2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        [Field(typeof(object), "ObjField1")]
                        [Field(typeof(object), "ObjField2", 
                            Options = FieldOptions.CustomEquality)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjField1Equals(other.ObjField2) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField2, other.ObjField2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField", 
                            Options = FieldOptions.All)]
                        [Field(typeof(object), "ObjField1")]
                        [Field(typeof(object), "ObjField2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjFieldEquals(other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField2, other.ObjField2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        [Field(typeof(object), "ObjField1", 
                            Options = FieldOptions.All)]
                        [Field(typeof(object), "ObjField2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjField1Equals(other.ObjField1) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField, other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField2, other.ObjField2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        [Field(typeof(object), "ObjField1")]
                        [Field(typeof(object), "ObjField2", 
                            Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            ObjField1Equals(other.ObjField2) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField2, other.ObjField2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        [Field(typeof(object), "ObjField1")]
                        [Field(typeof(object), "ObjField2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public System.Boolean Equals(TVO other) =>
                            other != null &&
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField, other.ObjField) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField1, other.ObjField1) && 
                            System.Collections.Generic.EqualityComparer<System.Object>.Default.Equals(ObjField2, other.ObjField2);
                        """
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesEqualsCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentEqualsMethods().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
