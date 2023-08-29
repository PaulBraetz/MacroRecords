using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class ExplicitConversionTests
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
                        String.Empty
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.All)]
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
                        [MacroRecord(Options = RecordOptions.ExplicitConversion)]
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
                        [MacroRecord(Options = RecordOptions.None)]
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
						[MacroRecord]
						[Field(typeof(int), "Field1")]
						partial class TVO
						{
						}
						""",
                        """
						public static explicit operator TVO(System.Int32 in_Field1) => Create(in_Field1);
						"""
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.ExplicitConversion)]
                        [Field(typeof(int), "Field1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
						public static explicit operator TVO(System.Int32 in_Field1) => Create(in_Field1);
						"""
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.All)]
                        [Field(typeof(int), "Field1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
						public static explicit operator TVO(System.Int32 in_Field1) => Create(in_Field1);
						"""
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.ExplicitConversion)]
                        [Field(typeof(System.IComparable), "Field1")]
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
                        [MacroRecord(Options = RecordOptions.All)]
                        [Field(typeof(System.IComparable), "Field1")]
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
                        [MacroRecord(Options = RecordOptions.None)]
                        [Field(typeof(int), "Field1")]
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
						[MacroRecord]
						[Field(typeof(int), "Field1")]
						[Field(typeof(string), "Field2")]
						partial class TVO
						{
						}
						""",
                        """
						public static explicit operator TVO((System.Int32, System.String) values) => Create(values.Item1, values.Item2);
						"""
                    },
                    new Object[]
                    {
                        """
						using RhoMicro.MacroRecords;
						[MacroRecord]
						[Field(typeof(int), "Field1")]
						[Field(typeof(System.IComparable), "Field2")]
						partial class TVO
						{
						}
						""",
                        """
						public static explicit operator TVO((System.Int32, System.IComparable) values) => Create(values.Item1, values.Item2);
						"""
                    },
                    new Object[]
                    {
                        """
						using RhoMicro.MacroRecords;
						[MacroRecord]
						[Field(typeof(System.IFormattable), "Field1")]
						[Field(typeof(System.IComparable), "Field2")]
						partial class TVO
						{
						}
						""",
                        """
						public static explicit operator TVO((System.IFormattable, System.IComparable) values) => Create(values.Item1, values.Item2);
						"""
                    },
                    new Object[]
                    {
                        """
						using RhoMicro.MacroRecords;
						[MacroRecord(Options = RecordOptions.ExplicitConversion)]
						[Field(typeof(int), "Field1")]
						[Field(typeof(string), "Field2")]
						partial class TVO
						{
						}
						""",
                        """
						public static explicit operator TVO((System.Int32, System.String) values) => Create(values.Item1, values.Item2);
						"""
                    },
                    new Object[]
                    {
                        """
						using RhoMicro.MacroRecords;
						[MacroRecord(Options = RecordOptions.All)]
						[Field(typeof(int), "Field1")]
						[Field(typeof(string), "Field2")]
						partial class TVO
						{
						}
						""",
                        """
						public static explicit operator TVO((System.Int32, System.String) values) => Create(values.Item1, values.Item2);
						"""
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.None)]
                        [Field(typeof(int), "Field1")]
                        [Field(typeof(string), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        String.Empty
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesSummaryCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddExplicitConversion().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
