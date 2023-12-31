﻿namespace RhoMicro.MacroRecords.Tests.Parent
{
    [TestClass]
    public class DeconstructableTests
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
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Deconstructable)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public static implicit operator int(TVO instance)=>
                            instance.Field1;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public static implicit operator int(TVO instance)=>
                            instance.Field1;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(System.IComparable), "Field1", Options = FieldOptions.Deconstructable)]
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
                        [Field(typeof(System.IComparable), "Field1", Options = FieldOptions.All)]
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
                        [Field(typeof(System.IComparable), "Field1", Options = FieldOptions.None)]
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
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Deconstructable)]
                        [Field(typeof(int), "Field2", Options = FieldOptions.Deconstructable)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public void Deconstruct(out int out_Field1, out int out_Field2)
                        {
                            out_Field1 = this.Field1;
                            out_Field2 = this.Field2;
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.All)]
                        [Field(typeof(int), "Field2", Options = FieldOptions.Deconstructable)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public void Deconstruct(out int out_Field1, out int out_Field2)
                        {
                            out_Field1 = this.Field1;
                            out_Field2 = this.Field2;
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Deconstructable)]
                        [Field(typeof(int), "Field2", Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public void Deconstruct(out int out_Field1, out int out_Field2)
                        {
                            out_Field1 = this.Field1;
                            out_Field2 = this.Field2;
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.Deconstructable)]
                        [Field(typeof(int), "Field2", Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public static implicit operator int(TVO instance)=>
                            instance.Field1;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.All)]
                        [Field(typeof(int), "Field2", Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public static implicit operator int(TVO instance)=>
                            instance.Field1;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.None)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.Deconstructable)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public static implicit operator string(TVO instance)=>
                            instance.Field2;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.None)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public static implicit operator string(TVO instance)=>
                            instance.Field2;
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.None)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.Deconstructable)]
                        [Field(typeof(object), "ObjectField", Options = FieldOptions.Deconstructable)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public void Deconstruct(out string out_Field2, out object out_ObjectField)
                        {
                            out_Field2 = this.Field2;
                            out_ObjectField = this.ObjectField;
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.None)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.All)]
                        [Field(typeof(object), "ObjectField", Options = FieldOptions.Deconstructable)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public void Deconstruct(out string out_Field2, out object out_ObjectField)
                        {
                            out_Field2 = this.Field2;
                            out_ObjectField = this.ObjectField;
                        }
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.None)]
                        [Field(typeof(string), "Field2", Options = FieldOptions.Deconstructable)]
                        [Field(typeof(object), "ObjectField", Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public void Deconstruct(out string out_Field2, out object out_ObjectField)
                        {
                            out_Field2 = this.Field2;
                            out_ObjectField = this.ObjectField;
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
            var actual = builder.AddParentDeconstruction()
                .BuildCore();

            //Assert
            Assertions.AreEquivalent(expected, actual);
        }
    }
}
