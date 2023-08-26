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
        /// Gets or sets a value indicating whether the generator should generate a constructor.
        /// <para>
        /// Set this to <see langword="false"/> if the record inherits a class that requires
        /// its own constructor to be called. In that case, make sure to properly assign the records
        /// fields inside your constructor.
        /// </para>
        /// <para>
        /// At least one constructor with the signature <c>ctor(T1, T2,..., Tn)</c>, where <c>Ti</c> is 
        /// a property type corresponding to the order of declaration, has to be available in order for the factory 
        /// methods to compile correctly.
        /// </para>
        /// </summary>
        public Boolean GenerateConstructor { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether the generator should generate the <see cref="System.Diagnostics.DebuggerDisplayAttribute"/> annotation.
        /// </summary>
        public Boolean GenerateDebugDisplay { get; set; } = true;
        /// <summary>
        /// Gets or sets the visibility of the generated constructor.
        /// </summary>
        public Visibility ConstructorVisibility { get; set; } = Visibility.Private;
    }
}
