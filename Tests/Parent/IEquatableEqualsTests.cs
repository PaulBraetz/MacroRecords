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
                        static partial void GetCustomEqualities(in TVO a, in TVO b, ref CustomEqualities equalities);
                        public bool Equals(TVO other)
                        {
                            var self = this;
                            var equalities = new CustomEqualities();
                            GetCustomEqualities(in self, in other, ref equalities);
                            var result = (
                                equalities.FieldIsEqual ?? 
                                global::System.Collections.Generic.EqualityComparer<byte[]>.Default.Equals(self.Field, other.Field)) && 
                                (equalities.Field2IsEqual ?? 
                                global::System.Collections.Generic.EqualityComparer<string>.Default.Equals(self.Field2, other.Field2));
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

    [TestClass]
    public class GetHashCodeTests
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
                        static partial void GetCustomHashCodes(in TVO obj, ref CustomHashCodes hashCodes);
                        public override int GetHashCode()
                        {
                            var instance = this;
                            var hashCodes = new CustomHashCodes();
                            GetCustomHashCodes(in instance, ref hashCodes);
                            var result = (
                            hashCodes.FieldHashCode ?? 
                            global::System.Collections.Generic.EqualityComparer<byte[]>.Default.GetHashCode(instance.Field),
                            hashCodes.Field2HashCode ?? 
                            global::System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(instance.Field2))
                            .GetHashCode(); 

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
            var actual = builder.AddParentGetHashCodeMethod().BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
