using Microsoft.CodeAnalysis;

using System;

namespace RhoMicro.MacroRecords.Core
{
    internal static class Extensions
    {
        public static ITypeSymbol GetTypeSymbol(this FieldAttribute attribute) =>
            attribute.TypeSymbol as ITypeSymbol ??
            throw new Exception("Field attribute was not provided with instance of ITypeSymbol.");
    }
}
