using RhoMicro.CodeAnalysis;
using RhoMicro.CodeAnalysis.Attributes;

using System;
using System.Collections.Generic;

namespace RhoMicro.MacroRecords.Core
{
    internal static class Util
    {
        public static readonly IAttributeFactory<FieldAttribute> FieldAttributeFactory = AttributeFactory<FieldAttribute>.Create();
        public static readonly IAttributeFactory<MacroRecordAttribute> MacroRecordAttributeFactory = AttributeFactory<MacroRecordAttribute>.Create();

        public static readonly ITypeIdentifier FieldAttributeIdentifier = TypeIdentifier.Create<FieldAttribute>();
        public static readonly ITypeIdentifier MacroRecordAttributeIdentifier = TypeIdentifier.Create<MacroRecordAttribute>();

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
        public static String GetString(Visibility visibility)
        {
            var result = _visibilities[(Int32)visibility];

            return result;
        }
    }
}
