using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

using RhoMicro.ValueObjectGenerator;

namespace ValueObjectGenerator.Tests
{
    internal static class Util
    {
        public static ValueObjectSourceBuilder CreateBuilder(String source)
        {
            var tree = CSharpSyntaxTree.ParseText(source);

            var compilation = CSharpCompilation.Create("TestCompilation")
                              .AddReferences(
                                 MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
                                 MetadataReference.CreateFromFile(typeof(Generator).Assembly.Location))
                              .AddSyntaxTrees(tree);
            var root = tree.GetRoot();
            var declaration = root.DescendantNodes().OfType<TypeDeclarationSyntax>().Single();
            var semanticModel = compilation.GetSemanticModel(tree);

            //suppressed for debuggability
#pragma warning disable IDE0046 // Convert to conditional expression
            if(ValueObjectSourceBuilder.TryCreate(declaration, semanticModel, out var builder))
            {
                return builder;
            }

            throw new Exception("Unable to create source builder.");
#pragma warning restore IDE0046 // Convert to conditional expression
        }
    }
}
