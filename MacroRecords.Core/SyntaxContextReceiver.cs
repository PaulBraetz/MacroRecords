using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

namespace RhoMicro.MacroRecords.Core
{
    internal sealed class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        private readonly HashSet<MacroRecordSourceBuilder> _results = new HashSet<MacroRecordSourceBuilder>();
        public IReadOnlyCollection<MacroRecordSourceBuilder> Results => _results;

        void ISyntaxContextReceiver.OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var semanticModel = context.SemanticModel;
            var node = context.Node;
            OnVisitSyntaxNode(semanticModel, node);
        }

        internal void OnVisitSyntaxNode(SemanticModel semanticModel, SyntaxNode node)
        {
            if(node is TypeDeclarationSyntax typeDeclaration &&
                typeDeclaration.Modifiers.Any(m => m.ValueText == "partial") &&
               MacroRecordSourceBuilder.TryCreate(typeDeclaration, semanticModel, out var builder))
            {
                _results.Add(builder);
            }
        }
    }
}