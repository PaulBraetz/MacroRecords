using System;

namespace ValueObjects
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	sealed class GeneratedValueObjectAttribute : Attribute
	{
		public Boolean GenerateConstructor { get; set; } = true;
	}
}