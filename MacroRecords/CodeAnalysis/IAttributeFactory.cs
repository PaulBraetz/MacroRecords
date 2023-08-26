using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RhoMicro.CodeAnalysis.Attributes
{
    internal interface IAttributeFactory<T>
    {
        System.Boolean TryBuild(AttributeSyntax attributeData, SemanticModel semanticModel, out T attribute);
    }
}
