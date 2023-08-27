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
		Constructor = 1,
		/// <summary>
		/// The generated type definition should be annotated with the <see cref="System.Diagnostics.DebuggerDisplayAttribute"/>.
		/// </summary>
		DebuggerDisplay = 2,
		/// <summary>
		/// The generated type definition should include a static readonly field containing the type default value.
		/// This option only affects struct definitions.
		/// </summary>
		EmptyMember = 4,
		/// <summary>
		/// The generated type definition should include an explicit conversion operator. If only a single field is defined, the
		/// explicit conversion operator will expect an argument of its type. 
		/// Otherwise, the operator will expect a value tuple corresponding to the fields defined.
		/// </summary>
		ExplicitConversion = 8
	}
}
