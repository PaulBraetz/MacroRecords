using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests.Parent
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
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        [Field(typeof(int), "Field1", Options = FieldOptions.None)]
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
                        [MacroRecord,
                        Field(typeof(int), "Field1", Options = FieldOptions.None),
                        Field(typeof(int), "Field2", Options = FieldOptions.None)]
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
                        [Field(typeof(int), "Field1", Options = FieldOptions.SupportsWith)]
                        partial class TVO
                        {
                        }
                        """,
						"""
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1);
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
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1);
                        """
                    },
                    new Object[]
					{
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1", Options = FieldOptions.SupportsWith),
                        Field(typeof(int), "Field2", Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
						"""
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        """
					},
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1", Options = FieldOptions.All),
                        Field(typeof(int), "Field2", Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        """
                    },
                    new Object[]
					{
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1", Options = FieldOptions.SupportsWith),
                        Field(typeof(string), "Field2", Options = FieldOptions.SupportsWith)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        public TVO WithField2(string in_Field2) =>
                            Create(this.Field1, in_Field2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1", Options = FieldOptions.All),
                        Field(typeof(string), "Field2", Options = FieldOptions.SupportsWith)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        public TVO WithField2(string in_Field2) =>
                            Create(this.Field1, in_Field2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1", Options = FieldOptions.SupportsWith),
                        Field(typeof(string), "Field2", Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        public TVO WithField2(string in_Field2) =>
                            Create(this.Field1, in_Field2);
                        """
                    },
                    new Object[]
                    {
                        """
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1", Options = FieldOptions.All),
                        Field(typeof(string), "Field2", Options = FieldOptions.All)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        public TVO WithField2(string in_Field2) =>
                            Create(this.Field1, in_Field2);
                        """
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
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1);
                        """
					},
					new Object[]
					{
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1"),
                        Field(typeof(int), "Field2", Options = FieldOptions.None)]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        """
                    },
					new Object[]
					{
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord,
                        Field(typeof(int), "Field1"),
                        Field(typeof(string), "Field2")]
                        partial class TVO
                        {
                        }
                        """,
                        """
                        public TVO WithField1(int in_Field1) =>
                            Create(in_Field1, this.Field2);
                        public TVO WithField2(string in_Field2) =>
                            Create(this.Field1, in_Field2);
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
