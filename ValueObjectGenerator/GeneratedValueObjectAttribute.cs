using System;

namespace RhoMicro.ValueObjectGenerator
{
    [AttributeUsage(AttributeTargets.Struct)]
    internal sealed class GeneratedValueObjectAttribute : Attribute
    {
        public GeneratedValueObjectAttribute(Type wrappedType)
        {
            WrappedType = wrappedType;
        }
        public Type WrappedType { get; private set; }
        public String ValueSpecification { get; set; }

        public void SetTypeProperty(String propertyName, Object type)
        {
            if(propertyName == nameof(WrappedType))
            {
                WrappedType = Type.GetType(type.ToString());
            }
        }
        public Object GetTypeProperty(String propertyName)
        {
            if(propertyName == nameof(WrappedType))
            {
                return WrappedType;
            }

            throw new InvalidOperationException();
        }
        public void SetTypeParameter(String parameterName, Object type)
        {
            if(parameterName == "wrappedType")
            {
                WrappedType = Type.GetType(type.ToString());
            }
        }
        public Object GetTypeParameter(String parameterName)
        {
            if(parameterName == "wrappedType")
            {
                return WrappedType;
            }

            throw new InvalidOperationException();
        }
    }
}
