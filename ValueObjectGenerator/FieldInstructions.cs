using System;

namespace RhoMicro.ValueObjectGenerator
{
    internal sealed class FieldInstructions
    {
        public FieldInstructions(
            String inParamName,
            String nullPropagatingToken,
            String outParamName,
            FieldAttribute attribute)
        {
            InParamName = inParamName;
            NullPropagatingToken = nullPropagatingToken;
            OutParamName = outParamName;
            Attribute = attribute;
        }

        public readonly FieldAttribute Attribute;
        public readonly String InParamName;
        public readonly String OutParamName;
        public readonly String NullPropagatingToken;

        public static Boolean TryCreate(FieldAttribute attribute, out FieldInstructions instruction)
        {
            instruction = null;
            if(String.IsNullOrEmpty(attribute.Name) ||
                attribute.Type == null)
            {
                return false;
            }

            var fieldName = attribute.Name;
            var inParamName = $"in_{fieldName}";
            var outParamName = $"out_{fieldName}";
            var nullPropagatingToken =
                attribute.Type.IsValueType &&
                Nullable.GetUnderlyingType(attribute.Type) == null ?
                    String.Empty :
                    "?";

            instruction = new FieldInstructions(
                inParamName,
                nullPropagatingToken,
                outParamName,
                attribute);

            return true;
        }
    }
}