using RhoMicro.CodeAnalysis.Attributes;

namespace RhoMicro.ValueObjectGenerator
{
    internal static class AttributeUnits
    {
        public static readonly AttributeAnalysisUnit<GeneratedValueObjectAttribute> GeneratedValueObject =
            new AttributeAnalysisUnit<GeneratedValueObjectAttribute>(
@"using System;

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

		public void SetTypeParameter(String parameterName, Object type)
		{
			if (parameterName == ""wrappedType"")
			{
				WrappedType = Type.GetType(type.ToString());
			}
		}
		public Object GetTypeParameter(String parameterName)
		{
			if (parameterName == ""wrappedType"")
			{
				return WrappedType;
			}

			throw new InvalidOperationException();
		}
	}
}
");
    }
}