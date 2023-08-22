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

			var nullPropagating = Attribute.WrappedType.IsValueType && Nullable.GetUnderlyingType(Attribute.WrappedType) == null ?
					String.Empty :
					"?";
			var toString = Declaration.Members
				.OfType<MethodDeclarationSyntax>()
				.Any(m => m.Modifiers.Any(t => t.IsKind(SyntaxKind.OverrideKeyword)) &&
						m.Identifier.ValueText == nameof(ToString)) ?
					String.Empty :
					$"public override System.String ToString()\r\n\t\t{{\r\n\t\t\treturn _value{nullPropagating}.ToString();\r\n\t\t}}";

			var source =
$@"namespace {typeIdentifier.Namespace}
{{
	{visibility} partial struct {name} : IEquatable<{name}>
	{{
		private {name}({wrappedType} value)
		{{
			_value = value;
		}}

		private readonly {wrappedType} _value;
		public static readonly {name} Empty = new();

		public static partial System.Boolean IsValid({wrappedType} value);

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
		public static {name} Create({wrappedType} value)
		{{
			if(!TryCreate(value, out var result))
			{{
				throw new System.ArgumentException(""The value provided for creating an instance of {name} was not valid.{reason}"", ""value"");
			}}

			return result;
		}}
		public static implicit operator {wrappedType}({name} instance)
		{{
			return instance._value;
		}}
		public static explicit operator {name}({wrappedType} value)
		{{
			return Create(value);
		}}
		public override System.Boolean Equals(System.Object obj)
		{{
			return obj is {name} instance && Equals(instance);
		}}
		public System.Boolean Equals({name} other)
		{{
			return System.Collections.Generic.EqualityComparer<{wrappedType}>.Default.Equals(_value, other._value);
		}}
		public override System.Int32 GetHashCode()
		{{
			return System.Collections.Generic.EqualityComparer<{wrappedType}>.Default.GetHashCode(_value);
		}}
		public static System.Boolean operator ==({name} left, {name} right)
		{{
			return left.Equals(right);
		}}
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
