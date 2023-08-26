using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacroRecords.Tests.IsValidResult
{
    [TestClass]
    public class DeconstructionTests
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
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public void Deconstruct(out bool out_ObjFieldIsInvalid, out string out_ObjFieldError)
                        {
                            out_ObjFieldIsInvalid = ObjFieldIsInvalid;
                            out_ObjFieldError = ObjFieldError;
                        }
                        """
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesDeconstructionCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddIsValidResultDeconstruction()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
