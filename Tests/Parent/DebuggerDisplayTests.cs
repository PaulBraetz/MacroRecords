using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueObjectGenerator.Tests
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(GenerateDebugDisplay = false)]
                        partial class TVO
                        {
                        }
                        """,
                        String.Empty
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(
                            typeof(int), 
                            "Field1", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(
                            typeof(int), 
                            "Field1", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(
                            typeof(int), 
                            "Field1", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject]
                        [Field(
                            typeof(int), 
                            "Field1")]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(GenerateDebugDisplay = false)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(GenerateDebugDisplay = false)]
                        [Field(
                            typeof(int), 
                            "Field1", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
                        partial class TVO
                        {
                        }
                        """,
                        String.Empty
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(GenerateDebugDisplay = false)]
                        [Field(
                            typeof(int), 
                            "Field1", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject(GenerateDebugDisplay = false)]
                        [Field(
                            typeof(int), 
                            "Field1")]
                        [Field(
                            typeof(int), 
                            "Field2", 
                            GenerateOptions = FieldAttribute.Options.ExcludedFromDebuggerDisplay)]
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
