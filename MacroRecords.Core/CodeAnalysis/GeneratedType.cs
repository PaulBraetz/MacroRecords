﻿using Microsoft.CodeAnalysis;

using System;

namespace RhoMicro.CodeAnalysis
{
    internal readonly struct GeneratedType : IEquatable<GeneratedType>
    {
        public GeneratedType(TypeIdentifier identifier, SyntaxNode source) 
            : this(identifier, new GeneratedSource(source, identifier.ToNonGenericString()))
        {
        }
        public GeneratedType(TypeIdentifier identifier, String source) 
            : this(identifier, new GeneratedSource(source, identifier.ToNonGenericString()))
        {
        }
        public GeneratedType(TypeIdentifier identifier, GeneratedSource source) 
            : this()
        {
            Identifier = identifier;
            Source = source;
        }

        public GeneratedSource Source { get; }
        public TypeIdentifier Identifier { get; }

        public INamedTypeSymbol ExtractSymbol(Compilation compilation)
        {
            var symbol = compilation.GetTypeByMetadataName(Identifier.ToString());

            return symbol;
        }

        public override String ToString() => $"Generated Type: {Identifier}";

        public override Boolean Equals(Object obj) => obj is GeneratedType type && Equals(type);

        public Boolean Equals(GeneratedType other)
        {
            return Source.Equals(other.Source) &&
                   Identifier.Equals(other.Identifier);
        }

        public override Int32 GetHashCode()
        {
            var hashCode = 966028054;
            hashCode = hashCode * -1521134295 + Source.GetHashCode();
            hashCode = hashCode * -1521134295 + Identifier.GetHashCode();
            return hashCode;
        }

        public static Boolean operator ==(GeneratedType left, GeneratedType right) => left.Equals(right);

        public static Boolean operator !=(GeneratedType left, GeneratedType right) => !(left == right);
    }
}
