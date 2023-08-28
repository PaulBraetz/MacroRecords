using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests.Parent
{
	[TestClass]
	public class EmptyMemberTests
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
                        [MacroRecord(Options = RecordOptions.EmptyMember)]
                        partial struct TVO
                        {
                        }
                        """,
						"""
						public static readonly TVO Empty = default;
						"""
					},
					new Object[]
					{
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord]
                        partial struct TVO
                        {
                        }
                        """,
						"""
						public static readonly TVO Empty = default;
						"""
					},
					new Object[]
					{
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.None)]
                        partial struct TVO
                        {
                        }
                        """,
						String.Empty
					},
					new Object[]
					{
						"""
                        using RhoMicro.MacroRecords;
                        [MacroRecord(Options = RecordOptions.EmptyMember)]
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
					}
				};
			}
		}

		[TestMethod]
		[DynamicData(nameof(Data))]
		public void GeneratesConstructorCorrectly(String consumer, String expected)
		{
			//Arrange
			var builder = Util.CreateBuilder(consumer);

			//Act
			var actual = builder.AddParentEmptyMember()
				.BuildCore();

			//Assert
			Assertions.AreEquivalent(expected, actual);
		}
	}
}
