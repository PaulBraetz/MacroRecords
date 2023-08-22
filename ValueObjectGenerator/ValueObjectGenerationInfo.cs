using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhoMicro.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace RhoMicro.ValueObjectGenerator
{
	internal readonly partial struct ValueObjectGenerationInfo : IEquatable<ValueObjectGenerationInfo>
	{
		public ValueObjectGenerationInfo(StructDeclarationSyntax declaration, GeneratedValueObjectAttribute attribute, SemanticModel semanticModel)
		{
			Attribute = attribute;
			Declaration = declaration;
			SemanticModel = semanticModel;
		}
		public readonly GeneratedValueObjectAttribute Attribute;
		public readonly StructDeclarationSyntax Declaration;
		public readonly SemanticModel SemanticModel;

		public override bool Equals(object obj)
		{
			return obj is ValueObjectGenerationInfo info && Equals(info);
		}

		public bool Equals(ValueObjectGenerationInfo other)
		{
			return EqualityComparer<StructDeclarationSyntax>.Default.Equals(Declaration, other.Declaration);
		}

		public override int GetHashCode()
		{
			return 302405195 + EqualityComparer<StructDeclarationSyntax>.Default.GetHashCode(Declaration);
		}

		public static bool operator ==(ValueObjectGenerationInfo left, ValueObjectGenerationInfo right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ValueObjectGenerationInfo left, ValueObjectGenerationInfo right)
		{
			return !(left == right);
		}
	}
}