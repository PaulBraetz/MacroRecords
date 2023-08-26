using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RhoMicro.CodeAnalysis
{
    internal readonly struct TypeIdentifierName : ITypeIdentifierName, IEquatable<ITypeIdentifierName>
    {
        public ImmutableArray<IIdentifierPart> Parts { get; }

        private TypeIdentifierName(ImmutableArray<IIdentifierPart> parts)
        {
            Parts = parts;
        }

        public static TypeIdentifierName Create<T>() => Create(typeof(T));
        public static TypeIdentifierName Create(Type type) => Create().AppendNamePart(type.Name);
        public static TypeIdentifierName Create(ITypeSymbol symbol)
        {
            var result = Create();

            if(symbol.ContainingType != null)
            {
                var containingType = Create(symbol.ContainingType);
                result = result.AppendTypePart(containingType);
            }

            var flag = false;
            if(symbol is IArrayTypeSymbol arraySymbol)
            {
                flag = true;
                symbol = arraySymbol.ElementType;
            }

            result = result.AppendNamePart(symbol.Name);

            if(symbol is INamedTypeSymbol namedSymbol && namedSymbol.TypeArguments.Any())
            {
                var arguments = new ITypeIdentifier[namedSymbol.TypeArguments.Length];

                for(var i = 0; i < arguments.Length; i++)
                {
                    var typeArgument = namedSymbol.TypeArguments[i];
                    var argument = SymbolEqualityComparer.Default.Equals(typeArgument.ContainingType, namedSymbol)
                        ? TypeIdentifier.Create(TypeIdentifierName.Create().AppendNamePart(typeArgument.ToString()), Namespace.Create())
                        : TypeIdentifier.Create(typeArgument);
                    arguments[i] = argument;
                }

                result = result.AppendGenericPart(arguments);
            }

            if(flag)
            {
                result = result.AppendArrayPart();
            }

            return result;
        }
        public static TypeIdentifierName Create() => new TypeIdentifierName(ImmutableArray<IIdentifierPart>.Empty);
        public static TypeIdentifierName Create(IEnumerable<IIdentifierPart> parts)
        {
            if(parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            var resultParts = ImmutableArray.Create<IIdentifierPart>()
                .AddRange(parts);

            var result = new TypeIdentifierName(resultParts);

            return result;
        }

        public TypeIdentifierName AppendTypePart(ITypeIdentifierName type)
        {
            var parts = GetNextParts(IdentifierParts.Kind.Name)
                .AddRange(type.Parts);

            return new TypeIdentifierName(parts);
        }
        public TypeIdentifierName AppendNamePart(String name)
        {
            var parts = GetNextParts(IdentifierParts.Kind.Name)
                .Add(IdentifierPart.Name(name));

            return new TypeIdentifierName(parts);
        }
        public TypeIdentifierName AppendGenericPart(ITypeIdentifier[] arguments)
        {
            var parts = GetNextParts(IdentifierParts.Kind.GenericOpen)
                .Add(IdentifierPart.GenericOpen());

            var typesArray = arguments ?? Array.Empty<ITypeIdentifier>();

            for(var i = 0; i < typesArray.Length; i++)
            {
                var type = typesArray[i];

                if(type.Namespace.Parts.Any())
                {
                    parts = parts.AddRange(type.Namespace.Parts)
                                 .Add(IdentifierPart.Period());
                }

                parts = parts.AddRange(type.Name.Parts);

                if(i != typesArray.Length - 1)
                {
                    parts = parts.Add(IdentifierPart.Comma());
                }
            }

            parts = parts.Add(IdentifierPart.GenericClose());

            return new TypeIdentifierName(parts);
        }
        public TypeIdentifierName AppendArrayPart()
        {
            var parts = GetNextParts(IdentifierParts.Kind.Array).Add(IdentifierPart.Array());
            return new TypeIdentifierName(parts);
        }

        private ImmutableArray<IIdentifierPart> GetNextParts(IdentifierParts.Kind nextKind)
        {
            var lastKind = Parts.LastOrDefault()?.Kind ?? IdentifierParts.Kind.None;

            var prependSeparator = nextKind == IdentifierParts.Kind.Name &&
                                    (lastKind == IdentifierParts.Kind.GenericOpen ||
                                    lastKind == IdentifierParts.Kind.Name);

            return prependSeparator ? Parts.Add(IdentifierPart.Period()) : Parts;
        }

        public override String ToString() => String.Concat(Parts);

        public override Boolean Equals(Object obj) => obj is ITypeIdentifierName name && Equals(name);

        public Boolean Equals(ITypeIdentifierName other) => TypeIdentifierNameEqualityComparer.Instance.Equals(this, other);

        public override Int32 GetHashCode() => TypeIdentifierNameEqualityComparer.Instance.GetHashCode(this);

        public static Boolean operator ==(TypeIdentifierName left, TypeIdentifierName right) => left.Equals(right);

        public static Boolean operator !=(TypeIdentifierName left, TypeIdentifierName right) => !(left == right);

        public static implicit operator String(TypeIdentifierName name) => name.ToString();
    }
}
