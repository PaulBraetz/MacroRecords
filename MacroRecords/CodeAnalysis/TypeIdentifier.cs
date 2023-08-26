using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Linq;

namespace RhoMicro.CodeAnalysis
{
    internal readonly struct TypeIdentifier : ITypeIdentifier, IEquatable<ITypeIdentifier>
    {
        public ITypeIdentifierName Name { get; }
        public INamespace Namespace { get; }

        private TypeIdentifier(ITypeIdentifierName name, INamespace @namespace)
        {
            Name = name;
            Namespace = @namespace;
        }

        public static TypeIdentifier Create<T>() => Create(typeof(T));
        public static TypeIdentifier Create(Type type)
        {
            var name = TypeIdentifierName.Create();
            INamespace @namespace = null;

            if(type.IsNested)
            {
                var parentType = type.DeclaringType;
                var parentTypeIdentifier = Create(parentType);
                name = name.AppendTypePart(parentTypeIdentifier.Name);
                @namespace = parentTypeIdentifier.Namespace;
            }

            var typeName = type.Name;
            if(type.IsGenericType)
            {
                var iBacktick = typeName.IndexOf('`');
                if(iBacktick > 0)
                {
                    typeName = typeName.Remove(iBacktick);
                }
            }

            name = name.AppendNamePart(typeName);

            if(type.IsConstructedGenericType)
            {
                var genericArguments = type.GenericTypeArguments.Select(Create).OfType<ITypeIdentifier>().ToArray();
                name = name.AppendGenericPart(genericArguments);
            }

            if(type.IsArray)
            {
                name = name.AppendArrayPart();
            }

            if(@namespace == null)
            {
                @namespace = CodeAnalysis.Namespace.Create(type);
            }

            return Create(name, @namespace);
        }
        public static TypeIdentifier Create(TypeSyntax typeSyntax, SemanticModel semanticModel)
        {
            var symbol = semanticModel.GetDeclaredSymbol(typeSyntax) as ITypeSymbol ??
                         semanticModel.GetTypeInfo(typeSyntax).Type;

            var identifier = TypeIdentifier.Create(symbol);

            return identifier;
        }
        public static TypeIdentifier Create(ITypeSymbol symbol)
        {
            var identifier = symbol is ITypeParameterSymbol parameter ?
                 TypeIdentifierName.Create().AppendNamePart(parameter.Name) :
                 TypeIdentifierName.Create(symbol);
            var @namespace = symbol is ITypeParameterSymbol ?
                CodeAnalysis.Namespace.Create() :
                CodeAnalysis.Namespace.Create(symbol);

            return Create(identifier, @namespace);
        }
        public static TypeIdentifier Create(ITypeIdentifierName name, INamespace @namespace) => new TypeIdentifier(name, @namespace);

        public override Boolean Equals(Object obj) => obj is ITypeIdentifier identifier && Equals(identifier);

        public Boolean Equals(ITypeIdentifier other) => TypeIdentifierEqualityComparer.Instance.Equals(this, other);

        public override Int32 GetHashCode() => TypeIdentifierEqualityComparer.Instance.GetHashCode(this);

        public override String ToString()
        {
            var namespaceString = Namespace.ToString();
            var nameString = Name.ToString();
            return String.IsNullOrEmpty(namespaceString) ? String.IsNullOrEmpty(nameString) ? null : nameString.ToString() : $"{namespaceString}.{nameString}";
        }

        public static Boolean operator ==(TypeIdentifier left, TypeIdentifier right) => left.Equals(right);

        public static Boolean operator !=(TypeIdentifier left, TypeIdentifier right) => !(left == right);

        public static implicit operator String(TypeIdentifier identifier) => identifier.ToString();
    }
}
