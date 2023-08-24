using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using RhoMicro.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Xml.Linq;

namespace RhoMicro.ValueObjectGenerator
{
	internal sealed class ObjectInstructions
	{
		public ObjectInstructions(
			string visibility,
			string structOrClass,
			string defaultOrNull,
			bool generateToString,
			IReadOnlyList<FieldInstructions> fieldInstructions,
			string nullPropagatingToken,
			GeneratedValueObjectAttribute attribute,
			INamedTypeSymbol typeSymbol)
		{
			Visibility = visibility;
			StructOrClass = structOrClass;
			DefaultOrNull = defaultOrNull;
			GenerateToString = generateToString;
			FieldInstructions = fieldInstructions;
			NullPropagatingToken = nullPropagatingToken;
			Attribute = attribute;
			TypeSymbol = typeSymbol;

			ValidatedFieldInstructions = FieldInstructions.Where(f => f.Attribute.IsValidated).ToArray();
		}

		public readonly GeneratedValueObjectAttribute Attribute;
		public readonly String Visibility;
		public readonly String StructOrClass;
		public readonly String DefaultOrNull;
		public readonly Boolean GenerateToString;
		public readonly IReadOnlyList<FieldInstructions> FieldInstructions;
		public readonly IReadOnlyList<FieldInstructions> ValidatedFieldInstructions;
		public readonly String NullPropagatingToken;
		public readonly INamedTypeSymbol TypeSymbol;

		public static ObjectInstructions Create(
			TypeDeclarationSyntax declaration,
			SemanticModel semanticModel,
			GeneratedValueObjectAttribute head,
			IReadOnlyList<GenerateValueObjectFieldAttribute> fields)
		{
			var declarationSymbol = semanticModel.GetDeclaredSymbol(declaration);
			var visibility = GetVisibility(declaration);
			var (structOrClass, defaultOrNull) = GetStructOrClass(declaration);
			var generateToString = GetGenerateToString(declaration);
			var fieldInstructions = fields
				.Select(f => (success: ValueObjectGenerator.FieldInstructions.TryCreate(f, out var instruction), instruction))
				.Where(t => t.success)
				.Select(t => t.instruction)
				.ToArray();
			var nullPropagatingToken = structOrClass == "class" ?
				"?" :
				String.Empty;
			var result = new ObjectInstructions(
				visibility,
				structOrClass,
				defaultOrNull,
				generateToString,
				fieldInstructions,
				nullPropagatingToken,
				head,
				declarationSymbol);

			return result;
		}
		private static Boolean GetGenerateToString(TypeDeclarationSyntax declaration)
		{
			var result = declaration.Members
				.OfType<MethodDeclarationSyntax>()
				.Any(m => m.Modifiers.Any(t => t.IsKind(SyntaxKind.OverrideKeyword)) &&
						m.Identifier.ValueText == nameof(ToString));

			return result;
		}
		private static (String structOrClass, String defaultOrNull) GetStructOrClass(TypeDeclarationSyntax declaration) =>
			declaration.Keyword.ValueText == "struct" ?
				("struct", "default") :
				("class", "null");

		private static String GetVisibility(TypeDeclarationSyntax declaration)
		{
			var modifiers = declaration.Modifiers.Where(IsVisibility).Select(m => m.ValueText);
			var result = String.Join(" ", modifiers);

			return result;
		}
		private static Boolean IsVisibility(SyntaxToken token) =>
			token.IsKind(SyntaxKind.PublicKeyword) ||
			token.IsKind(SyntaxKind.PrivateKeyword) ||
			token.IsKind(SyntaxKind.InternalKeyword) ||
			token.IsKind(SyntaxKind.ProtectedKeyword);

		private static (GeneratedValueObjectAttribute head, IEnumerable<GenerateValueObjectFieldAttribute> fields) GetAttributes(TypeDeclarationSyntax declaration, SemanticModel semanticModel)
		{
			var attributes = declaration.AttributeLists
				.OfAttributeClasses(
				semanticModel,
				AttributeUnits.GeneratedValueObject.GeneratedType.Identifier,
				AttributeUnits.GeneratedValueObjectField.GeneratedType.Identifier);

			var head = attributes
				.Select(a => (success: AttributeUnits.GeneratedValueObject.Factory.TryBuild(a, semanticModel, out var result), result))
				.Where(t => t.success)
				.Select(t => t.result)
				.FirstOrDefault();

			var fields = attributes
				.Select(a => (success: AttributeUnits.GeneratedValueObjectField.Factory.TryBuild(a, semanticModel, out var result), result))
				.Where(t => t.success)
				.Select(t => t.result);

			return (head, fields);
		}
	}
	internal sealed class FieldInstructions
	{
		public FieldInstructions(
			string inParamName,
			string nullPropagatingToken,
			string outParamName,
			GenerateValueObjectFieldAttribute attribute)
		{
			InParamName = inParamName;
			NullPropagatingToken = nullPropagatingToken;
			OutParamName = outParamName;
			Attribute = attribute;
		}

		public readonly GenerateValueObjectFieldAttribute Attribute;
		public readonly string InParamName;
		public readonly string OutParamName;
		public readonly string NullPropagatingToken;

		public static Boolean TryCreate(GenerateValueObjectFieldAttribute attribute, out FieldInstructions instruction)
		{
			instruction = null;
			if (String.IsNullOrEmpty(attribute.Name) ||
				attribute.Type == null)
			{
				return false;
			}

			var fieldName = attribute.Name;
			var inParamName = $"in_{fieldName}";
			var outParamName = $"out_{fieldName}";
			var fullTypeName = attribute.Type.FullName;
			var nullPropagatingToken =
				attribute.Type.IsValueType &&
				Nullable.GetUnderlyingType(attribute.Type) == null ?
					String.Empty :
					"?";
			var visibility = Util.GetString(attribute.Visibility);
			var summary = attribute.Summary;

			instruction = new FieldInstructions(
				inParamName,
				nullPropagatingToken,
				outParamName,
				attribute);

			return true;
		}
	}
}