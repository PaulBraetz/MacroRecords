using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RhoMicro.CodeAnalysis
{
    internal sealed class ImmutableArrayEqualityComparer<T> : IEqualityComparer<ImmutableArray<T>>
    {
        public ImmutableArrayEqualityComparer(IEqualityComparer<T> elementComparer)
        {
            _elementComparer = elementComparer ?? throw new ArgumentNullException(nameof(elementComparer));
        }

        public static readonly ImmutableArrayEqualityComparer<T> Instance = new  ImmutableArrayEqualityComparer<T>(EqualityComparer<T>.Default);
        private readonly IEqualityComparer<T> _elementComparer;

        public Boolean Equals(ImmutableArray<T> x, ImmutableArray<T> y)
        {
            var result = x == y ||
                         x != null &&
                         y != null &&
                         x.SequenceEqual(y, _elementComparer);

            return result;
        }

        public Int32 GetHashCode(ImmutableArray<T> obj)
        {
            if(obj.IsDefault)
            {
                return 0;
            }

            unchecked
            {
                var hash = -179327946;

                foreach(var element in obj)
                {
                    hash = hash * -1521134295 + _elementComparer.GetHashCode(element);
                }

                return hash;
            }
        }
    }
}
