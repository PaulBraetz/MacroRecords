using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests
{
    [TestClass]
    public class DebuggerDisplayTests
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
                        [System.Diagnostics.DebuggerDisplayAttribute("TVO")]
                        """
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
                        [Field(
                            typeof(int), 
                            "Field1", 
                            Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        [System.Diagnostics.DebuggerDisplayAttribute("TVO")]
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(
                            typeof(int), 
                            "Field1")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        [System.Diagnostics.DebuggerDisplayAttribute("TVO(Field1 : {Field1})")]
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(
                            typeof(int), 
                            "Field1")]
                        [Field(
                            typeof(int), 
                            "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        [System.Diagnostics.DebuggerDisplayAttribute("TVO(Field1 : {Field1}, Field2 : {Field2})")]
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(
                            typeof(int), 
                            "Field1", 
                            Options = FieldOptions.None)]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        [System.Diagnostics.DebuggerDisplayAttribute("TVO")]
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(
                            typeof(int), 
                            "Field1", 
                            Options = FieldOptions.None)]
                        [Field(
                            typeof(int), 
                            "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        [System.Diagnostics.DebuggerDisplayAttribute("TVO(Field2 : {Field2})")]
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(
                            typeof(int), 
                            "Field1")]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        [System.Diagnostics.DebuggerDisplayAttribute("TVO(Field1 : {Field1})")]
                        """
                    },
                    new Object[]
                    {
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.None)]
                        [Field(
                            typeof(int), 
                            "Field1")]
                        [Field(
                            typeof(int), 
                            "Field2")]
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
                        [Field(
                            typeof(int), 
                            "Field1", 
                            Options = FieldOptions.None)]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            Options = FieldOptions.None)]
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
                        [Field(
                            typeof(int), 
                            "Field1", 
                            Options = FieldOptions.None)]
                        [Field(
                            typeof(int), 
                            "Field2")]
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
                        [Field(
                            typeof(int), 
                            "Field1")]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            Options = FieldOptions.None)]
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
        public void GeneratesDebuggerDisplayCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentDebuggerDisplayAttribute()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
