namespace RhoMicro.CodeAnalysis
{
    internal interface ITypeIdentifier
    {
        ITypeIdentifierName Name { get; }
        INamespace Namespace { get; }
    }
}