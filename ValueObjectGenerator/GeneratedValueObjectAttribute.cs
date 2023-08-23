using System;
using System.Diagnostics;

namespace RhoMicro.ValueObjectGenerator
{
	/// <summary>
	/// Informs the value object generator to generate a value type from
	/// the annotated partial struct or class declaration.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
	public sealed class GeneratedValueObjectAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets a value indicating whether the generator should generate a custom constructor.
		/// <para>
		/// Set this to <see langword="false"/> if the value object inherits a class that requires
		/// its own constructor to be called. In that case, make sure to properly assign the value objects
		/// properties inside your constructor.
		/// </para>
		/// <para>
		/// At least one constructor with the signature <c>ctor(T1, T2,..., Tn)</c>, where <c>Ti</c> is 
		/// a property type corresponding to the order of declaration, has to be available in order for the factory 
		/// methods to compile correctly.
		/// </para>
		/// </summary>
		public Boolean GenerateConstructor { get; set; } = true;
		/// <summary>
		/// Gets or sets a value indicating whether the generator should generate the <see cref="DebuggerDisplayAttribute"/> annotation.
		/// </summary>
		public Boolean GenerateDebugDisplay { get; set; } = true;
	}
}
