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
                        [MacroRecord(Options = RecordOptions.All)]
                        [Field(typeof(byte[]), "Field", Options = FieldOptions.All)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.All)]
                        readonly partial struct TVO {}
                        """,
                        """
                        partial void GetCustomEqualities(in TVO other, ref CustomEqualities equalities);
                        public bool Equals(TVO other)
                        {
                            var equalities = new CustomEqualities();
                            GetCustomEqualities(in other, ref equalities);
                            var result = (
                                equalities.FieldIsEqual ?? 
                                global::System.Collections.Generic.EqualityComparer<byte[]>.Default.Equals(this.Field, other.Field)) && 
                                (equalities.Field2IsEqual ?? 
                                global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(this.Field2, other.Field2));
                            return result;
                        }
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
            var actual = builder.AddParentIEquatableEqualsMethod().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
