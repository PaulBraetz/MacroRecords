using System;
using System.Collections.Generic;
using System.Text;

namespace RhoMicro.ValueObjectGenerator
{
	internal static class Util
	{
		private static readonly IReadOnlyDictionary<GenerateValueObjectFieldAttribute.VisibilityModifier, String> _visibilities =
			new Dictionary<GenerateValueObjectFieldAttribute.VisibilityModifier, String>()
			{
				{ GenerateValueObjectFieldAttribute.VisibilityModifier.Public, "public"},
				{ GenerateValueObjectFieldAttribute.VisibilityModifier.Protected, "protected"},
				{ GenerateValueObjectFieldAttribute.VisibilityModifier.Private, "private"},
				{ GenerateValueObjectFieldAttribute.VisibilityModifier.Internal, "internal"},
				{ GenerateValueObjectFieldAttribute.VisibilityModifier.ProtectedInternal, "protected internal"},
				{ GenerateValueObjectFieldAttribute.VisibilityModifier.PrivateProtected, "private protected"}
			};
		public static String GetString(GenerateValueObjectFieldAttribute.VisibilityModifier visibility)
		{
			var result = _visibilities[visibility];

			return result;
		}
	}
}
