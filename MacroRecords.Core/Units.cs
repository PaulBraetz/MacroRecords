using RhoMicro.CodeAnalysis.Attributes;

namespace RhoMicro.MacroRecords.Core
{
    internal static partial class Units
    {
        public static readonly AttributeAnalysisUnit<FieldAttribute> FieldAttribute =
            new AttributeAnalysisUnit<FieldAttribute>(
@"using System;

namespace RhoMicro.MacroRecords
{
    /// <summary>
    /// Instructs the macro record generator to generate a readonly field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public sealed class FieldAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name=""type"">The type of the generated field.</param>
        /// <param name=""name"">The name of the generated field.</param>
        public FieldAttribute(Type type, String name)
        {
            Name = name;
        }
        /// <summary>
        /// Gets the name of the generated field.
        /// </summary>
        public String Name { get; private set; }
        /// <summary>
        /// Gets or sets the visibility of the generated field.
        /// </summary>
        public Visibility Visibility { get; set; }
        /// <summary>
        /// Gets or sets the documentation summary of the generated field.
        /// </summary>
        public String Summary { get; set; }
        /// <summary>
        /// Gets or sets additional options for the generated field.
        /// </summary>
        public FieldOptions Options { get; set; } = FieldOptions.DebuggerDisplay
            | FieldOptions.Deconstructable
            | FieldOptions.SupportsWith;

        /// <summary>
        /// Gets a value indicating whether the <see cref=""FieldOptions.Validated""/> flag is set on <see cref=""Options""/>.
        /// </summary>
        public Boolean IsValidated => Options.HasFlag(FieldOptions.Validated);
        /// <summary>
        /// Gets a value indicating whether the <see cref=""FieldOptions.Deconstructable""/> flag is set on <see cref=""Options""/>.
        /// </summary>
        public Boolean IsDeconstructable => Options.HasFlag(FieldOptions.Deconstructable);
        /// <summary>
        /// Gets a value indicating whether the <see cref=""FieldOptions.SupportsWith""/> flag is set on <see cref=""Options""/>.
        /// </summary>
        public Boolean SupportsWith => Options.HasFlag(FieldOptions.SupportsWith);
        /// <summary>
        /// Gets a value indicating whether the <see cref=""FieldOptions.DebuggerDisplay""/> flag is set on <see cref=""Options""/>.
        /// </summary>
        public Boolean IncludedInDebuggerDisplay => Options.HasFlag(FieldOptions.DebuggerDisplay);

        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <returns></returns>
        public Object TypeSymbol { get; private set; } = null;

        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name=""parameterName""></param>
        /// <param name=""type""></param>
        public void SetTypeParameter(String parameterName, Object type)
        {
            if(parameterName == ""type"")
            {
                TypeSymbol = type;
            }
        }
    }
}
");

        public static readonly AttributeAnalysisUnit<MacroRecordAttribute> MacroAttribute =
            new AttributeAnalysisUnit<MacroRecordAttribute>(
@"using System;

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
		/// The generated type definition should be annotated with the <see cref=""System.Diagnostics.DebuggerDisplayAttribute""/>.
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
");
    }
}