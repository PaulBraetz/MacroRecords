using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RhoMicro.CodeAnalysis
{
    internal readonly struct Namespace : INamespace, IEquatable<INamespace>
    {
        private Namespace(ImmutableArray<IIdentifierPart> parts)
        {
            Parts = parts;
        }

        public ImmutableArray<IIdentifierPart> Parts { get; }

        public static Namespace Create<T>() => Create(typeof(T));
        public static Namespace Create(Type type)
        {
            var namespaceParts = type.Namespace.Split('.');
            return Create().AppendRange(namespaceParts);
        }
        public static Namespace Create(ISymbol symbol)
        {
            var result = Create();

            while(symbol != null && symbol.Name != String.Empty)
            {
                if(symbol is INamespaceSymbol)
                {
                    result = result.Prepend(symbol.Name);
                }

                symbol = symbol.ContainingNamespace;
            }

            return result;
        }
        public static Namespace Create() => new Namespace(ImmutableArray.Create<IIdentifierPart>());

        public Namespace Append(String name)
        {
            if(String.IsNullOrWhiteSpace(name))
            {
                return this;
            }

            var parts = GetNextParts().Add(IdentifierPart.Name(name));

            return new Namespace(parts);
        }
        public Namespace Prepend(String name)
        {
            if(String.IsNullOrWhiteSpace(name))
            {
                return this;
            }

            var parts = GetPreviousParts().Insert(0, IdentifierPart.Name(name));

            return new Namespace(parts);
        }
        public Namespace PrependRange(IEnumerable<String> names)
        {
            var @namespace = this;
            foreach(var name in names)
            {
                @namespace = @namespace.Prepend(name);
            }

            return @namespace;
        }
        public Namespace AppendRange(IEnumerable<String> names)
        {
            var @namespace = this;
            foreach(var name in names)
            {
                @namespace = @namespace.Append(name);
            }

            return @namespace;
        }

        private ImmutableArray<IIdentifierPart> GetNextParts()
        {
            var lastKind = Parts.LastOrDefault()?.Kind ?? IdentifierParts.Kind.None;

            var prependSeparator = lastKind == IdentifierParts.Kind.Name;

            return prependSeparator ?
                Parts.Add(IdentifierPart.Period()) :
                Parts;
        }
        private ImmutableArray<IIdentifierPart> GetPreviousParts()
        {
            var firstKind = Parts.FirstOrDefault()?.Kind ?? IdentifierParts.Kind.None;

            var appendSeparator = firstKind == IdentifierParts.Kind.Name;

            return appendSeparator ?
                Parts.Insert(0, IdentifierPart.Period()) :
                Parts;
        }

        public override String ToString() => String.Concat(Parts);

        public override Boolean Equals(Object obj) => obj is INamespace @namespace && Equals(@namespace);

        public Boolean Equals(INamespace other) => NamespaceEqualityComparer.Instance.Equals(this, other);

        public override Int32 GetHashCode() => NamespaceEqualityComparer.Instance.GetHashCode(this);

        public static Boolean operator ==(Namespace left, Namespace right) => left.Equals(right);

        public static Boolean operator !=(Namespace left, Namespace right) => !(left == right);

        public static implicit operator String(Namespace @namespace) => @namespace.ToString();
    }
}
