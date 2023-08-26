using System;

namespace RhoMicro.MacroRecords
{
	/// <summary>
	/// Defines generation options for a field.
	/// </summary>
	[Flags]
	public enum FieldOptions
	{
		/// <summary>
		/// No special generation options should be applied to this field.
		/// </summary>
		None = 0,
		/// <summary>
		/// The field will be included in the generated deconstruction mechanism.
		/// <para>
		/// If only a single field contained in the record is marked as 
		/// deconstructable, the mechanism chosen will be the implicit type
		/// conversion to that fields type using the field value.
		/// </para>
		/// <para>
		/// If multiple fields are marked as deconstructable, 
		/// the mechanism chosen will be the <c>Deconstruct</c>
		/// method, making available all deconstructible field values as
		/// <see langword="out"/> parameters.
		/// </para>
		/// <para>
		/// If no fields are marked as deconstructable, no deconstruction mechanism will be generated.
		/// </para>
		/// </summary>
		Deconstructable = 1,
		/// <summary>
		/// The field will support the <c>WithX(T x)</c> syntax for 
		/// transforming instances of the generated value object.
		/// </summary>
		SupportsWith = 2,
		/// <summary>
		/// The field will be included in the generated validation mechanisms.
		/// </summary>
		Validated = 4,
		/// <summary>
		/// The field name and value will be included in the <see cref="System.Diagnostics.DebuggerDisplayAttribute"/>.
		/// </summary>
		DebuggerDisplay = 8
	}
}
