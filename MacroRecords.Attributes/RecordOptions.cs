using System;

namespace RhoMicro.MacroRecords
{
	/// <summary>
	/// Defines generation options for a record type definition.
	/// </summary>
	[Flags]
	public enum RecordOptions
	{
		/// <summary>
		/// No special generation options should be applied to this record.
		/// </summary>
		None = 0,
		/// <summary>
		/// The generated type definition should have a default constructor that assigns all fields generated.
		/// <para>
		/// Unset this if the record inherits a class that requires
		/// its own constructor to be called. In that case, make sure to properly assign the records
		/// fields inside your constructor.
		/// </para>
		/// <para>
		/// At least one constructor with the signature <c>ctor(T1, T2,..., Tn)</c>, where <c>Ti</c> is 
		/// a property type corresponding to the order of declaration, has to be available in order for the factory 
		/// methods to compile correctly.
		/// </para>
		/// </summary>
		GenerateConstructor = 1,
		/// <summary>
		/// The generated type definition should be annotated with the <see cref="System.Diagnostics.DebuggerDisplayAttribute"/>.
		/// </summary>
		GenerateDebugDisplay = 2,
		/// <summary>
		/// The generated type definition should include a static readonly field containing the type default value.
		/// This option only affects struct definitions.
		/// </summary>
		GenerateEmptyMember = 4
	}
}
