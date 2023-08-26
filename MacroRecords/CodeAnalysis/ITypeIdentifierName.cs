using System.Collections.Immutable;

namespace RhoMicro.CodeAnalysis
{
    internal interface ITypeIdentifierName
    {
        ImmutableArray<IIdentifierPart> Parts { get; }
    }
}