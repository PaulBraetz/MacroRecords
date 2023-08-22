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
            var attributeUnit = AttributeUnits.GeneratedValueObject;

            if(node is StructDeclarationSyntax structDeclaration &&
                structDeclaration.Identifier.ValueText != String.Empty)
            {
                if(!structDeclaration.HasAttributes(semanticModel, attributeUnit.GeneratedType.Identifier))
                {
                    return;
                }

                var attributes = structDeclaration.AttributeLists
                                    .OfAttributeClasses(semanticModel, attributeUnit.GeneratedType.Identifier);
                var attribute = attributes
                    .Select(a => (success: attributeUnit.Factory.TryBuild(a, semanticModel, out var result), result))
                    .Where(t => t.success)
                    .Select(t => t.result)
                    .FirstOrDefault();

                if(attribute == null)
                {
                    return;
                }

                _results.Add(new ValueObjectGenerationInfo(structDeclaration, attribute, semanticModel));
            }
        }
    }
}