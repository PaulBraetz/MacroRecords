using System;

namespace ValueObjects
{
	enum Visibility
	{
		Public,
		Private,
		Protected,
		Internal,
		InternalProtected
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
	sealed class GenerateValueObjectPropertyAttribute : Attribute
	{
		public GenerateValueObjectPropertyAttribute(Type type, string name)
		{
			Type = type;
			Name = name;
		}
		public Type Type { get; private set; }
		public String Name { get; private set; }
		public Boolean IsPublic { get; set; }
	}
}