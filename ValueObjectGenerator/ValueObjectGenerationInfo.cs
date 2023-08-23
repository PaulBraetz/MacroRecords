using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhoMicro.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace RhoMicro.ValueObjectGenerator
{
	internal readonly partial struct ValueObjectGenerationInfo : IEquatable<ValueObjectGenerationInfo>
	{
		private readonly TypeDeclarationSyntax _typeDeclaration;
		private readonly SemanticModel _semanticModel;
		private readonly GeneratedValueObjectAttribute _generateAttribute;
		private readonly IReadOnlyList<GenerateValueObjectFieldAttribute> _fieldAttributes;

		public ValueObjectGenerationInfo(
			TypeDeclarationSyntax typeDeclaration,
			SemanticModel semanticModel,
			GeneratedValueObjectAttribute generateAttribute,
			IReadOnlyList<GenerateValueObjectFieldAttribute> fieldAttributes)
		{
			_typeDeclaration = typeDeclaration;
			_semanticModel = semanticModel;
			_generateAttribute = generateAttribute;
			_fieldAttributes = fieldAttributes;
		}

		public override bool Equals(object obj)
		{
			return obj is ValueObjectGenerationInfo info && Equals(info);
		}

		public bool Equals(ValueObjectGenerationInfo other)
		{
			return EqualityComparer<TypeDeclarationSyntax>.Default.Equals(_typeDeclaration, other._typeDeclaration);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<TypeDeclarationSyntax>.Default.GetHashCode(_typeDeclaration);
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