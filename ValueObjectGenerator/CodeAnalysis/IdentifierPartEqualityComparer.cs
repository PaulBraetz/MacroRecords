using System.Collections.Generic;

namespace RhoMicro.CodeAnalysis
{
    internal sealed class IdentifierPartEqualityComparer : IEqualityComparer<IIdentifierPart>
    {
        public static readonly IdentifierPartEqualityComparer Instance = new IdentifierPartEqualityComparer();
        public System.Boolean Equals(IIdentifierPart x, IIdentifierPart y)
        {
            var result = x == y ||
                         x != null &&
                         y != null &&
                         x.Value == y.Value;

            return result;
        }

        public System.Int32 GetHashCode(IIdentifierPart obj)
        {
            var hashcode = obj == null
                ? 0
                : -1937169414 + EqualityComparer<System.String>.Default.GetHashCode(obj.Value);

            return hashcode;
        }
    }
}
