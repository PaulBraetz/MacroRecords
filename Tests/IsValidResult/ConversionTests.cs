using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests.IsValidResult
{
    [TestClass]
    public class ConversionTests
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
                        [Field(typeof(object), "ObjField", Options = FieldOptions.Validated)]
                        partial class TVO {}
                        """,
                        "public static implicit operator System.Boolean(IsValidResult result) => !result.ObjFieldIsInvalid;"
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField")]
                        partial class TVO {}
                        """,
                        "public static implicit operator System.Boolean(IsValidResult result) => true;"
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField1", Options = FieldOptions.Validated)]
                        [Field(typeof(object), "ObjField2", Options = FieldOptions.Validated)]
                        partial class TVO {}
                        """,
                        "public static implicit operator System.Boolean(IsValidResult result) => !result.ObjField1IsInvalid && !result.ObjField2IsInvalid;"
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField1", Options = FieldOptions.Validated)]
                        [Field(typeof(object), "ObjField2")]
                        partial class TVO {}
                        """,
                        "public static implicit operator System.Boolean(IsValidResult result) => !result.ObjField1IsInvalid;"
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField1")]
                        [Field(typeof(object), "ObjField2", Options = FieldOptions.Validated)]
                        partial class TVO {}
                        """,
                        "public static implicit operator System.Boolean(IsValidResult result) => !result.ObjField2IsInvalid;"
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(object), "ObjField1")]
                        [Field(typeof(object), "ObjField2")]
                        partial class TVO {}
                        """,
                        "public static implicit operator System.Boolean(IsValidResult result) => true;"
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesConversionCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddIsValidResultConversion()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
