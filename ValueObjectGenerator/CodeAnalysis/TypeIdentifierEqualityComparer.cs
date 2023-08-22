using System;
using System.Collections.Generic;

namespace RhoMicro.CodeAnalysis
{
    internal sealed class TypeIdentifierEqualityComparer : IEqualityComparer<ITypeIdentifier>
    {
        public static readonly TypeIdentifierEqualityComparer Instance = new TypeIdentifierEqualityComparer();
        public Boolean Equals(ITypeIdentifier x, ITypeIdentifier y)
        {
            var result = x == y ||
                         x != null &&
                         y != null &&
                         NamespaceEqualityComparer.Instance.Equals(x.Namespace, y.Namespace) &&
                         TypeIdentifierNameEqualityComparer.Instance.Equals(x.Name, y.Name);

            return result;
        }

        public Int32 GetHashCode(ITypeIdentifier obj)
        {
            if(obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var hashCode = -179327946;
            hashCode = hashCode * -1521134295 + TypeIdentifierNameEqualityComparer.Instance.GetHashCode(obj.Name);
            hashCode = hashCode * -1521134295 + NamespaceEqualityComparer.Instance.GetHashCode(obj.Namespace);
            return hashCode;
        }
    }
}
