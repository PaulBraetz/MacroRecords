using System;

namespace RhoMicro.ValueObjectGenerator
{
    /// <summary>
    /// Informs the value object generator to generate a value type using
    /// the annotated partial struct definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    internal sealed class GeneratedValueObjectAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="wrappedType">The primitive type wrapped by instances of this value object.</param>
        public GeneratedValueObjectAttribute(Type wrappedType)
        {
            WrappedType = wrappedType;
        }
        /// <summary>
        /// Gets the primitive type wrapped by instances of this value object.
        /// </summary>
        public Type WrappedType { get; private set; }
        /// <summary>
        /// Gets or sets the explanation specifiying the conditions that must apply to 
        /// a primitive value in order to be eligible for wrapping by an instance of 
        /// the value object.
        /// </summary>
        public String ValueSpecification { get; set; }

        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="type"></param>
        public void SetTypeProperty(String propertyName, Object type)
        {
            if(propertyName == nameof(WrappedType))
            {
                WrappedType = Type.GetType(type.ToString());
            }
        }
        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Object GetTypeProperty(String propertyName)
        {
            if(propertyName == nameof(WrappedType))
            {
                return WrappedType;
            }

            throw new InvalidOperationException();
        }
        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="type"></param>
        public void SetTypeParameter(String parameterName, Object type)
        {
            if(parameterName == "wrappedType")
            {
                WrappedType = Type.GetType(type.ToString());
            }
        }
        /// <summary>
        /// This method is not intended for use outside of the generator.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
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
