namespace RhoMicro.CodeAnalysis
{
    internal static class IdentifierParts
    {
        public enum Kind : System.Byte
        {
            None,
            Array,
            GenericOpen,
            GenericClose,
            Comma,
            Period,
            Name
        }
    }
}