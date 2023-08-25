using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueObjectGenerator.Tests.Parent
{
    [TestClass]
    public class TransformationTests
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
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject,
                        Field(typeof(int), "Field1"),
                        Field(typeof(int), "Field2")]
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
                        [Field(typeof(int), "Field1", GenerateOptions = FieldAttribute.Options.SupportsWith)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(System.Int32 in_Field1) =>
                            Create(in_Field1);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject,
                        Field(typeof(int), "Field1", GenerateOptions = FieldAttribute.Options.SupportsWith),
                        Field(typeof(int), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(System.Int32 in_Field1) =>
                            Create(in_Field1, Field2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.ValueObjectGenerator;
                        [GeneratedValueObject,
                        Field(typeof(int), "Field1", GenerateOptions = FieldAttribute.Options.SupportsWith),
                        Field(typeof(string), "Field2", GenerateOptions = FieldAttribute.Options.SupportsWith)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(System.Int32 in_Field1) =>
                            Create(in_Field1, Field2);
                        public TVO WithField2(System.String in_Field2) =>
                            Create(Field1, in_Field2);
                        """
                    }
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Data))]
        public void GeneratesTransformatorsCorrectly(String consumer, String expected)
        {
            //Arrange
            var builder = Util.CreateBuilder(consumer);

            //Act
            var actual = builder.AddParentTransformation()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
