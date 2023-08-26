namespace RhoMicro.CodeAnalysis
{
    internal interface IIdentifierPart
    {
        IdentifierParts.Kind Kind { get; }
        System.String Value { get; }
    }
}