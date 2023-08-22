using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using RhoMicro.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace RhoMicro.ValueObjectGenerator
{
    internal readonly partial struct ValueObjectGenerationInfo
    {
        internal GeneratedType GeneratePartial()
        {
            var typeSymbol = SemanticModel.GetDeclaredSymbol(Declaration) as ITypeSymbol;
            var typeIdentifier = TypeIdentifier.Create(typeSymbol);
            var name = typeIdentifier.Name.ToString();
            var wrappedType = Attribute.WrappedType.FullName;
            var spec = Attribute.ValueSpecification;
            var reason = spec == null ?
                String.Empty :
                 $" Reason: {spec}";
            var visibilityModifiers = Declaration.Modifiers
                .Select(m => (m, k: m.Kind()))
                .Where(t => t.k == SyntaxKind.PublicKeyword ||
                          t.k == SyntaxKind.InternalKeyword ||
                          t.k == SyntaxKind.PrivateKeyword ||
                          t.k == SyntaxKind.ProtectedKeyword)
                .Select(t => t.m.ValueText);
            var visibility = String.Join(" ", visibilityModifiers);

            var isValueType = Attribute.WrappedType.IsValueType && Nullable.GetUnderlyingType(Attribute.WrappedType) == null;
            var nullPropagating = isValueType ?
                    String.Empty :
                    "?";
            var defaultLangword = isValueType ?
                "default" :
                "null";

            var toString = Declaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Any(m => m.Modifiers.Any(t => t.IsKind(SyntaxKind.OverrideKeyword)) &&
                        m.Identifier.ValueText == nameof(ToString)) ?
                    String.Empty :
$@"		/// <inheritdoc/>
		public override System.String ToString()
		{{
			return _value{nullPropagating}.ToString();
		}}";

            var source =
$@"namespace {typeIdentifier.Namespace}
{{
	{visibility} partial struct {name} : IEquatable<{name}>
	{{
		/// <summary>
		/// Initializes a new instance.
		/// <para><paramref name=""value""/> will not be validated.</para>
		/// </summary>
		/// <param name=""value"">The value to wrap inside the new instance.</param>
		private {name}({wrappedType} value)
		{{
			_value = value;
		}}

		private readonly {wrappedType} _value;
		/// <summary>
		/// The empty (<see langword=""default""/>) instance of <see cref=""{name}""/>.
		/// </summary>
		public static readonly {name} Empty = new();
		/// <summary>
		/// Indicates whether an instance of <see cref=""{wrappedType}""/> is a valid representation of 
		/// <see cref=""{name}""/>.
		/// </summary>
		/// <param name=""value"">The value to check.</param>
		/// <returns>
		/// <see langword=""true""/> if <paramref name=""value""/> is a valid representation of <see cref=""{name}""/>;
		/// otherwise, <see	langword=""false""/>.
		/// </returns>
		public static partial System.Boolean IsValid({wrappedType} value);
		/// <summary>
		/// Attempts to create a new instance of <see cref=""{name}""/> using the primitive value provided.
		/// </summary>
		/// <param name=""value"">The primitive value to create an instance of <see cref=""{name}""/> from.</param>
		/// <param name=""result"">
		/// Contains the new instance if <paramref name=""value""/> is a valid representation of a 
		/// <see cref=""{name}""/>; otherwise, <see langword=""{defaultLangword}""/>.
		/// </param>
		/// <returns>
		/// <see langword=""true""/> if the value could successfully be wrapped; otherwise, <see langword=""false""/>.
		/// </returns>
		public static System.Boolean TryCreate({wrappedType} value, out {name} result)
		{{
			System.Boolean isValid = IsValid(value);
			if(isValid)
			{{
				result = new {name}(value);
			}} else {{
				result = Empty;
			}}

			return isValid;
		}}
		/// <summary>
		/// Creates a new instance of <see cref=""{name}""/> using the primitive value provided.
		/// </summary>
		/// <param name=""value"">The primitive value to create an instance of <see cref=""{name}""/> from.</param>
		/// <returns>A new instance of <see cref=""{name}""/> wrapping <paramref name=""value""/>.</returns>
		public static {name} Create({wrappedType} value)
		{{
			if(!TryCreate(value, out var result))
			{{
				throw new System.ArgumentException(""The value provided for creating an instance of {name} was not valid.{reason}"", ""value"");
			}}

			return result;
		}}
		/// <summary>
		/// </summary>
		public static implicit operator {wrappedType}({name} instance)
		{{
			return instance._value;
		}}
		/// <summary>
		/// </summary>
		public static explicit operator {name}({wrappedType} value)
		{{
			return Create(value);
		}}
		/// <inheritdoc/>
		public override System.Boolean Equals(System.Object obj)
		{{
			return obj is {name} instance && Equals(instance);
		}}
		/// <inheritdoc/>
		public System.Boolean Equals({name} other)
		{{
			return System.Collections.Generic.EqualityComparer<{wrappedType}>.Default.Equals(_value, other._value);
		}}
		/// <inheritdoc/>
		public override System.Int32 GetHashCode()
		{{
			return System.Collections.Generic.EqualityComparer<{wrappedType}>.Default.GetHashCode(_value);
		}}
		/// <summary>
		/// </summary>
		public static System.Boolean operator ==({name} left, {name} right)
		{{
			return left.Equals(right);
		}}
		/// <summary>
		/// </summary>
		public static System.Boolean operator !=({name} left, {name} right)
		{{
			return !(left == right);
		}}
{toString}
	}}
}}";

            var declarationSymbol = (ITypeSymbol)SemanticModel.GetDeclaredSymbol(Declaration);
            var identifier = TypeIdentifier.Create(declarationSymbol);
            var result = new GeneratedType(identifier, source);

            return result;
        }
    }
}
