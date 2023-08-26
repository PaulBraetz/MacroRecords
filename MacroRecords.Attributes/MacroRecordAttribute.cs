using System;

namespace RhoMicro.MacroRecords
{
	/// <summary>
	/// Informs the macro record generator to generate a record from
	/// the annotated partial struct or class declaration.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
	public sealed class MacroRecordAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the visibility of the generated constructor.
		/// </summary>
		public Visibility ConstructorVisibility { get; set; } = Visibility.Private;
		/// <summary>
		/// Gets or sets additional options for the generated record.
		/// </summary>
		public RecordOptions Options { get; set; } =
			RecordOptions.DebuggerDisplay |
			RecordOptions.Constructor;
	}
}
