using System;

namespace RhoMicro.MacroRecords.Core
{
    internal sealed class FieldInstructions
    {
        public FieldInstructions(
            String inParamName,
            String outParamName,
            FieldAttribute attribute)
        {
            InParamName = inParamName;
            OutParamName = outParamName;
            Attribute = attribute;
        }

        public readonly FieldAttribute Attribute;
        public readonly String InParamName;
        public readonly String OutParamName;

        public static Boolean TryCreate(FieldAttribute attribute, out FieldInstructions instruction)
        {
            instruction = null;
            if(String.IsNullOrEmpty(attribute.Name) ||
                attribute.TypeSymbol == null)
            {
                return false;
            }

            var fieldName = attribute.Name;
            var inParamName = $"in_{fieldName}";
            var outParamName = $"out_{fieldName}";

            instruction = new FieldInstructions(
                inParamName,
                outParamName,
                attribute);

            return true;
        }
    }
}