using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

namespace RhoMicro.ValueObjectGenerator
{
	internal sealed class SyntaxContextReceiver : ISyntaxContextReceiver
	{
		private readonly HashSet<ValueObjectGenerationInfo> _results = new HashSet<ValueObjectGenerationInfo>();
		public IReadOnlyCollection<ValueObjectGenerationInfo> Results => _results;

		void ISyntaxContextReceiver.OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			var semanticModel = context.SemanticModel;
			var node = context.Node;
			OnVisitSyntaxNode(semanticModel, node);
		}

		internal void OnVisitSyntaxNode(SemanticModel semanticModel, SyntaxNode node)
		{
			var generateUnit = AttributeUnits.GeneratedValueObject;
			var fieldUnit = AttributeUnits.GeneratedValueObjectField;

			if (node is TypeDeclarationSyntax typeDeclaration &&
			   (typeDeclaration is StructDeclarationSyntax ||
				typeDeclaration is ClassDeclarationSyntax))
			{
				if (!typeDeclaration.HasAttributes(
					semanticModel,
					generateUnit.GeneratedType.Identifier,
					fieldUnit.GeneratedType.Identifier))
				{
					return;
				}

				var generateAttribute = typeDeclaration.AttributeLists
					.OfAttributeClasses(semanticModel, generateUnit.GeneratedType.Identifier)
					.Select(a => (success: generateUnit.Factory.TryBuild(a, semanticModel, out var result), result))
					.Where(t => t.success)
					.Select(t => t.result)
					.FirstOrDefault();
				var fieldAttributes = typeDeclaration.AttributeLists
					.OfAttributeClasses(semanticModel, fieldUnit.GeneratedType.Identifier)
					.Select(a => (success: fieldUnit.Factory.TryBuild(a, semanticModel, out var result), result))
					.Where(t => t.success)
					.Select(t => t.result)
					.ToArray();

				if (generateAttribute == null || fieldAttributes.Length == 0)
				{
					return;
				}

				_results.Add(new ValueObjectGenerationInfo(typeDeclaration, semanticModel, generateAttribute, fieldAttributes));
			}
		}
	}
}