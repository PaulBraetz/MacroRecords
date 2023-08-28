using System;

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
        /// <param name="type">The type of the generated field.</param>
        /// <param name="name">The name of the generated field.</param>
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

        /// <summary>
        /// Gets a value indicating whether the <see cref="FieldOptions.Validated"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean IsValidated => Options.HasFlag(FieldOptions.Validated);
		/// <summary>
		/// Gets a value indicating whether the <see cref="FieldOptions.Deconstructable"/> flag is set on <see cref="Options"/>.
		/// </summary>
		public Boolean IsDeconstructable => Options.HasFlag(FieldOptions.Deconstructable);
		/// <summary>
		/// Gets a value indicating whether the <see cref="FieldOptions.SupportsWith"/> flag is set on <see cref="Options"/>.
		/// </summary>
		public Boolean SupportsWith => Options.HasFlag(FieldOptions.SupportsWith);
		/// <summary>
		/// Gets a value indicating whether the <see cref="FieldOptions.DebuggerDisplay"/> flag is set on <see cref="Options"/>.
		/// </summary>
		public Boolean IncludedInDebuggerDisplay => Options.HasFlag(FieldOptions.DebuggerDisplay);

        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="type"></param>
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
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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
        /// <param name="parameterName"></param>
        /// <param name="type"></param>
        public void SetTypeParameter(String parameterName, Object type)
        {
            if(parameterName == "type")
            {
                Type = Type.GetType(type.ToString());
            }
        }
        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Object GetTypeParameter(String parameterName)
        {
            if(parameterName == "type")
            {
                return Type;
            }

            throw new InvalidOperationException();
        }
    }
}
