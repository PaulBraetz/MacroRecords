using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using RhoMicro.MacroRecords.Core;

namespace RhoMicro.MacroRecords.Tests
{
    internal static class Util
    {
        public static MacroRecordSourceBuilder CreateBuilder(String source)
        {
            var parseOptions = new CSharpParseOptions(
                LanguageVersion.Latest, 
                DocumentationMode.Parse,
                SourceCodeKind.Regular);

            var tree = CSharpSyntaxTree.ParseText(source, parseOptions);

            var compilation = CSharpCompilation.Create("TestCompilation")
                              .AddReferences(
                                 MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
                                 MetadataReference.CreateFromFile(typeof(MacroRecordAttribute).Assembly.Location))
                              .AddSyntaxTrees(tree);

            var root = tree.GetRoot();
            var declaration = root.DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .Where(n => n is StructDeclarationSyntax || n is ClassDeclarationSyntax)
                .Last();

            var semanticModel = compilation.GetSemanticModel(tree);

            //suppressed for debuggability
#pragma warning disable IDE0046 // Convert to conditional expression
            if(MacroRecordSourceBuilder.TryCreate(declaration, semanticModel, out var builder))
            {
                return builder;
            }

            throw new Exception("Unable to create source builder.");
#pragma warning restore IDE0046 // Convert to conditional expression
        }
    }
}
