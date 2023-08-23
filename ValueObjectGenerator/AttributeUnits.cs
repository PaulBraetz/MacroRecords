using RhoMicro.CodeAnalysis.Attributes;

namespace RhoMicro.ValueObjectGenerator
{
	internal static class AttributeUnits
	{
		public static readonly AttributeAnalysisUnit<GeneratedValueObjectAttribute> GeneratedValueObject =
			new AttributeAnalysisUnit<GeneratedValueObjectAttribute>(
@"using System;
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
		/// Set this to <see langword=""false""/> if the value object inherits a class that requires
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
		/// Gets or sets a value indicating whether the generator should generate the <see cref=""DebuggerDisplayAttribute""/> annotation.
		/// </summary>
		public Boolean GenerateDebugDisplay { get; set; } = true;
	}
}
");
		public static readonly AttributeAnalysisUnit<GenerateValueObjectFieldAttribute> GeneratedValueObjectField =
			new AttributeAnalysisUnit<GenerateValueObjectFieldAttribute>(
@"using System;

namespace RhoMicro.ValueObjectGenerator
{
	/// <summary>
	/// Informs the value object generator to generate a readonly field into the generated
	/// value object.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
	public sealed class GenerateValueObjectFieldAttribute : Attribute
	{
		/// <summary>
		/// Defines field visibility modifiers.
		/// </summary>
		public enum VisibilityModifier
		{
			/// <summary>
			/// The field will be <see langword=""public""/>.
			/// </summary>
			Public,
			/// <summary>
			/// The field will be <see langword=""private""/>.
			/// </summary>
			Private,
			/// <summary>
			/// The field will be <see langword=""protected""/>.
			/// </summary>
			Protected,
			/// <summary>
			/// The field will be <see langword=""internal""/>.
			/// </summary>
			Internal,
			/// <summary>
			/// The field will be <see langword=""protected""/> <see langword=""internal""/>.
			/// </summary>
			ProtectedInternal,
			/// <summary>
			/// The field will be <see langword=""private""/> <see langword=""protected""/>.
			/// </summary>
			PrivateProtected
		}

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		/// <param name=""type"">The type of the generated field.</param>
		/// <param name=""name"">The name of the generated field.</param>
		public GenerateValueObjectFieldAttribute(Type type, string name)
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
		public VisibilityModifier Visibility { get; set; }
		/// <summary>
		/// Gets or sets the documentation summary of the generated field field.
		/// </summary>
		public String Summary { get; set; }

		/// <summary>
		/// This method is not intended for use outside of the generator.
		/// </summary>
		/// <param name=""propertyName""></param>
		/// <param name=""type""></param>
		public void SetTypeProperty(String propertyName, Object type)
		{
			if (propertyName == nameof(Type))
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
			if (propertyName == nameof(Type))
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
			if (parameterName == ""type"")
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
			if (parameterName == ""type"")
			{
				return Type;
			}

			throw new InvalidOperationException();
		}
	}
}
");
	}
}