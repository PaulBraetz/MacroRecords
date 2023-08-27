using RhoMicro.CodeAnalysis.Attributes;

namespace RhoMicro.MacroRecords
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
            Type = type;
            Name = name;
        }
        /// <summary>
        /// Gets the type of the generated field.
        /// </summary>
        public Type Type { get; private set; }
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

        public Boolean IsValidated => Options.HasFlag(FieldOptions.Validated);
        public Boolean IsDeconstructable => Options.HasFlag(FieldOptions.Deconstructable);
        public Boolean SupportsWith => Options.HasFlag(FieldOptions.SupportsWith);
        public Boolean IncludedInDebuggerDisplay => Options.HasFlag(FieldOptions.DebuggerDisplay);

        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name=""propertyName""></param>
        /// <param name=""type""></param>
        public void SetTypeProperty(String propertyName, Object type)
        {
            if(propertyName == nameof(Type))
            {
                Type = Type.GetType(type.ToString());
            }
        }
        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name=""propertyName""></param>
        /// <returns></returns>
        /// <exception cref=""InvalidOperationException""></exception>
        public Object GetTypeProperty(String propertyName)
        {
            if(propertyName == nameof(Type))
            {
                return Type;
            }

            throw new InvalidOperationException();
        }
        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name=""parameterName""></param>
        /// <param name=""type""></param>
        public void SetTypeParameter(String parameterName, Object type)
        {
            if(parameterName == ""type"")
            {
                Type = Type.GetType(type.ToString());
            }
        }
        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name=""parameterName""></param>
        /// <returns></returns>
        /// <exception cref=""InvalidOperationException""></exception>
        public Object GetTypeParameter(String parameterName)
        {
            if(parameterName == ""type"")
            {
                return Type;
            }

            throw new InvalidOperationException();
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
			RecordOptions.Constructor |
			RecordOptions.EmptyMember;

		public Boolean GenerateDebuggerDisplay => Options.HasFlag(RecordOptions.DebuggerDisplay);
		public Boolean GenerateConstructor => Options.HasFlag(RecordOptions.Constructor);
		public Boolean GenerateEmptyMember => Options.HasFlag(RecordOptions.EmptyMember);
	}
}
");
    }
}