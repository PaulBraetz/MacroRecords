using System.Collections.Immutable;

namespace RhoMicro.CodeAnalysis
{
    internal interface INamespace
    {
        ImmutableArray<IIdentifierPart> Parts { get; }
    }
}