using System;
using System.Collections.Generic;

namespace RhoMicro.ValueObjectGenerator
{
    internal static class Util
    {
        private static readonly IReadOnlyDictionary<Int32, String> _visibilities =
            new Dictionary<Int32, String>()
            {
                { 0, "public"},
                { 1, "private"},
                { 2, "protected"},
                { 3, "internal"},
                { 4, "protected internal"},
                { 5, "private protected"}
            };
        public static String GetString(FieldAttribute.VisibilityModifier visibility)
        {
            var result = _visibilities[(Int32)visibility];

            return result;
        }
        public static String GetString(GeneratedValueObjectAttribute.VisibilityModifier visibility)
        {
            var result = _visibilities[(Int32)visibility];

            return result;
        }
    }
}
