using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Schema;

namespace RhoMicro.CodeAnalysis
{
    internal static class Extensions
    {
        public static Int32 GetParentsCount(this ITypeIdentifierName name)
        {
            if(name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var parentsCount = -1;
            for(var i = 0; i < name.Parts.Length; i++)
            {
                var part = name.Parts[i];
                if(part.Kind == IdentifierParts.Kind.Name)
                {
                    parentsCount++;
                }

                if(part.Kind == IdentifierParts.Kind.GenericOpen ||
                    part.Kind == IdentifierParts.Kind.Array)
                {
                    break;
                }
            }

            var result = parentsCount > 0 ?
                parentsCount :
                0;

            return result;
        }

        public static ITypeIdentifierName ParentOrSelf(this ITypeIdentifierName name)
        {
            if(name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var parentsCount = name.GetParentsCount();
            var result = parentsCount > 0 ?
                TypeIdentifierName.Create(name.Parts.Take(parentsCount * 2 - 1)) :
                name;

            return result;
        }

        public static ITypeIdentifierName WithoutParents(this ITypeIdentifierName name)
        {
            if(name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var parentsCount = name.GetParentsCount();
            var result = parentsCount > 0 ?
                TypeIdentifierName.Create(name.Parts.Skip(parentsCount * 2)) :
                name;

            return result;
        }

        public static String ToNonGenericString(this TypeIdentifier identifier)
        {
            var result = String.Concat(identifier.Namespace.Parts.Append(IdentifierPart.Period())
                .Concat(identifier.Name.Parts.TakeWhile(p => p.Kind == IdentifierParts.Kind.Name || p.Kind == IdentifierParts.Kind.Period)));

            return result;
        }

        public static IEnumerable<IEnumerable<T>> Subsets<T>(this IEnumerable<T> collection)
        {
            var arr = collection.ToArray();
            var subsetCount = Math.Pow(2, arr.Length);

            for(var i = 0; i < subsetCount; i++)
            {
                yield return collection.Where((element, position) => ((1 << position) & i) == 0);
            }
        }
        public static T[][] ToArrays<T>(this IEnumerable<IEnumerable<T>> collections) => collections.Select(s => s.ToArray()).ToArray();

        public static void AddSource(this GeneratorPostInitializationContext context, GeneratedSource source) => context.AddSource(source.HintName, source.Text);
        public static void AddSources(this GeneratorPostInitializationContext context, IEnumerable<GeneratedSource> sources)
        {
            foreach(var source in sources)
            {
                context.AddSource(source);
            }
        }
        public static void AddSources(this GeneratorPostInitializationContext context, params GeneratedSource[] sources)
        {
            foreach(var source in sources)
            {
                context.AddSource(source);
            }
        }

        public static void AddSource(this GeneratorExecutionContext context, GeneratedSource source) => context.AddSource(source.HintName, source.Text);
        public static void AddSources(this GeneratorExecutionContext context, IEnumerable<GeneratedSource> sources)
        {
            foreach(var source in sources)
            {
                context.AddSource(source);
            }
        }
        public static void AddSources(this GeneratorExecutionContext context, params GeneratedSource[] sources)
        {
            foreach(var source in sources)
            {
                context.AddSource(source);
            }
        }

        public static INamedTypeSymbol GetSymbol(this Compilation compilation, TypeIdentifier identifier) =>
            compilation.GetTypeByMetadataName(identifier.ToString());

        public static TypeSyntax AsSyntax(this TypeIdentifier typeIdentifier)
        {
            var syntax = SyntaxFactory.ParseTypeName(typeIdentifier);

            return syntax;
        }
        public static TypeSyntax AsSyntax(this Type type)
        {
            var syntax = TypeIdentifier.Create(type).AsSyntax();

            return syntax;
        }

        public static ITypeIdentifier GetIdentifier(this Type type)
        {
            var identifier = type != null ?
                (ITypeIdentifier)TypeIdentifier.Create(type) :
                throw new ArgumentNullException(nameof(type));

            return identifier;
        }

        #region AttributeSyntax Operations
        public static Boolean Matches(this AttributeSyntax attribute, SemanticModel semanticModel, ConstructorInfo constructor)
        {
            var arguments = (IEnumerable<AttributeArgumentSyntax>)attribute.ArgumentList?.Arguments ?? Array.Empty<AttributeArgumentSyntax>();

            var match = matchesType() && matchesParameters() && matchesProperties();

            return match;

            Boolean matchesType()
            {
                var typeMatch = attribute.IsType(semanticModel, TypeIdentifier.Create(constructor.DeclaringType));

                return typeMatch;
            }

            Boolean matchesParameters()
            {
                var unmatchedArguments = arguments.Where(a => a.NameEquals == null).ToArray();

                var position = 0;
                var positionalParameters = constructor.GetParameters().ToDictionary(p => position++, p => p);
                var namedParameters = constructor.GetParameters().ToDictionary(p => p.Name, p => p);

                for(position = 0; position < unmatchedArguments.Length; position++)
                {
                    var unmatchedArgument = unmatchedArguments[position];

                    if(unmatchedArgument.NameColon == null)
                    {
                        if(!positionalParameters.TryGetValue(position, out var positionalParameter))
                        {
                            return false;
                        }

                        _ = namedParameters.Remove(positionalParameter.Name);
                        _ = positionalParameters.Remove(position);
                    } else
                    {
                        var argumentName = unmatchedArgument.NameColon.Name.Identifier.ToString();
                        if(!namedParameters.TryGetValue(argumentName, out var namedParameter))
                        {
                            return false;
                        }

                        _ = namedParameters.Remove(argumentName);
                        _ = positionalParameters.Remove(positionalParameters.Single(kvp => kvp.Value.Name == argumentName).Key);
                    }

                    if(unmatchedArgument.Expression is TypeOfExpressionSyntax &&
                        !constructor.DeclaringType.ImplementsMethodsSemantically<IHasTypeConstructorParameter>())
                    {
                        return false;
                    }
                }

                var noneLeft = !positionalParameters.Any(kvp => !kvp.Value.IsOptional);

                return noneLeft;
            }

            Boolean matchesProperties()
            {
                var properties = constructor.DeclaringType.GetProperties()
                    .Where(p => p.CanWrite)
                    .ToDictionary(p => p.Name, p => p);

                var allValid = arguments.Where(a => a.NameEquals != null)
                    .All(a =>
                        properties.ContainsKey(a.NameEquals.Name.Identifier.ToString()) &&
                        (!(a.Expression is TypeOfExpressionSyntax) || constructor.DeclaringType.ImplementsMethodsSemantically<IHasTypePropertySetter>()));

                return allValid;
            }
        }

        public static Boolean TryGetMethodSemantically(this Type type, MethodInfo referenceMethod, out MethodInfo declaredMethod)
        {
            declaredMethod = type.GetMethod(referenceMethod.Name,
                                            referenceMethod.GetParameters().Select(p => p.ParameterType).ToArray());

            return declaredMethod != null;
        }

        public static Boolean ImplementsMethodsSemantically<T>(this Type type)
        {
            var match = typeof(T).GetMethods().All(rm => type.TryGetMethodSemantically(rm, out var _));

            return match;
        }

        public static IEnumerable<AttributeSyntax> OfAttributeClasses(this IEnumerable<AttributeSyntax> attributes, SemanticModel semanticModel, params ITypeIdentifier[] identifiers)
        {
            var requiredTypes = identifiers.SelectMany(GetVariations).ToImmutableHashSet();
            foreach(var attribute in attributes)
            {
                var attributeTypeSymbol = semanticModel.GetTypeInfo(attribute).Type;
                var identifier = TypeIdentifier.Create(attributeTypeSymbol);
                if(requiredTypes.Contains(identifier))
                {
                    yield return attribute;
                }
            }
        }
        public static IEnumerable<AttributeSyntax> OfAttributeClasses(this IEnumerable<AttributeListSyntax> attributeLists, SemanticModel semanticModel, params ITypeIdentifier[] identifiers)
        {
            var foundAttributes = attributeLists.SelectMany(al => al.Attributes).OfAttributeClasses(semanticModel, identifiers);

            return foundAttributes;
        }
        public static Boolean HasAttributes(this IEnumerable<AttributeListSyntax> attributeLists, SemanticModel semanticModel, params ITypeIdentifier[] identifiers)
        {
            var match = attributeLists.OfAttributeClasses(semanticModel, identifiers).Any();

            return match;
        }

        public static Boolean IsType(this AttributeSyntax attribute, SemanticModel semanticModel, TypeIdentifier identifier)
        {
            var variations = GetVariations(identifier).ToImmutableHashSet();
            var attributeIdentifier = TypeIdentifier.Create(semanticModel.GetTypeInfo(attribute).Type);
            var match = variations.Contains(attributeIdentifier);

            return match;
        }
        public static Boolean TryParseArgument<T>(
            this AttributeSyntax attribute,
            SemanticModel semanticModel,
            out T value,
            Int32 position = -1,
            String propertyName = null,
            String parameterName = null)
        {
            var arg = attribute.GetArgument(semanticModel, position, propertyName, parameterName);

            var result = TryParse(arg, out value);

            return result;
        }
        public static Boolean TryParseArrayArgument<T>(this AttributeSyntax attribute, SemanticModel semanticModel, out T[] value, Int32 position = -1, String propertyName = null, String parameterName = null)
        {
            var arg = attribute.GetArgument(semanticModel, position, propertyName, parameterName);

            var result = TryParseArray(arg, out value);

            return result;
        }

        public static Boolean TryParse<T>(this Optional<Object> constant, out T value)
        {
            if(constant.HasValue)
            {
                if(constant.Value is T castValue)
                {
                    value = castValue;
                    return true;
                }

                if(constant.Value == null)
                {
                    value = default;
                    return true;
                }

                try
                {
                    value = (T)constant.Value;
                    return true;
                } catch { }
            }

            value = default;
            return false;
        }
        public static Boolean TryParseArray<T>(this Optional<Object> constant, out T[] values)
        {
            if(!constant.HasValue)
            {
                values = null;
                return false;
            }

            var elements = constant.Value is Object[] objectArray ? objectArray : constant.Value is T ? new Object[] { constant.Value } : Array.Empty<Object>();
            var tempValues = new T[elements.Length];

            for(var i = 0; i < elements.Length; i++)
            {
                var element = elements[i];

                if(element is T castValue)
                {
                    tempValues[i] = castValue;
                } else if(element == null)
                {
                    tempValues[i] = default;
                } else
                {
                    values = null;
                    return false;
                }
            }

            values = tempValues;
            return true;
        }

        public static Optional<Object> GetArgument(this AttributeSyntax attribute, SemanticModel semanticModel, Int32 position = -1, String propertyName = null, String parameterName = null)
        {
            var arguments = (IEnumerable<AttributeArgumentSyntax>)attribute.ArgumentList?.Arguments ?? Array.Empty<AttributeArgumentSyntax>();
            foreach(var argument in arguments)
            {
                if(argument.NameEquals != null)
                {
                    if(argument.NameEquals.Name.Identifier.ValueText.Equals(propertyName))
                    {
                        return getConstantValue();
                    }
                } else if(argument.NameColon != null)
                {
                    if(argument.NameColon.Name.Identifier.ValueText.Equals(parameterName))
                    {
                        return getConstantValue();
                    }
                } else if(position-- == 0)
                {
                    return getConstantValue();
                }

                Optional<Object> getConstantValue()
                {
                    var result = semanticModel.GetConstantValue(argument.Expression);

                    if(argument.Expression is ArrayCreationExpressionSyntax arrayCreationExpression)
                    {
                        var elements = arrayCreationExpression.Initializer?
                            .Expressions
                            .Select(e => semanticModel.GetConstantValue(e))
                            .Where(o => o.HasValue)
                            .Select(o => o.Value)
                            .ToArray() ?? Array.Empty<Object>();

                        result = new Optional<Object>(elements);
                    } else if(argument.Expression is ObjectCreationExpressionSyntax objectCreationExpression)
                    {
                        result = new Optional<Object>(new Object());
                    } else if(argument.Expression is TypeOfExpressionSyntax typeOfExpression)
                    {
                        var symbol = semanticModel.GetTypeInfo(typeOfExpression.Type).Type;

                        result = new Optional<Object>(symbol);
                    }

                    return result;
                }
            }

            return new Optional<Object>();
        }
        #endregion

        #region AttributeData Operations
        public static IEnumerable<AttributeData> OfAttributeClasses(this IEnumerable<AttributeData> attributes, params ITypeIdentifier[] identifiers)
        {
            var variations = identifiers.SelectMany(GetVariations).ToImmutableHashSet();
            foreach(var attribute in attributes)
            {
                var attributeTypeSymbol = attribute.AttributeClass.OriginalDefinition;
                var identifier = TypeIdentifier.Create(attributeTypeSymbol);
                if(variations.Contains(identifier))
                {
                    yield return attribute;
                }
            }
        }
        public static Boolean HasAttributes(this SyntaxNode node, SemanticModel semanticModel, params ITypeIdentifier[] identifiers)
        {
            var match = semanticModel.GetDeclaredSymbol(node)?.HasAttributes(identifiers)
                ?? throw new ArgumentException($"{nameof(node)} was not declared in {nameof(semanticModel)}.");

            return match;
        }
        public static Boolean HasAttributes(this ISymbol symbol, params ITypeIdentifier[] identifiers)
        {
            var match = symbol.GetAttributes().OfAttributeClasses(identifiers).Any();

            return match;
        }

        public static Boolean IsType(this AttributeData attribute, TypeIdentifier identifier)
        {
            var match = attribute.AttributeClass.ToDisplayString() == identifier.ToString();

            return match;
        }
        public static Boolean TryParseArgument<T>(this AttributeData attribute, out T value, Int32 position = -1, String propertyName = null)
        {
            var arg = attribute.GetArgument(position, propertyName);

            var result = TryParse(arg, out value);

            return result;
        }
        public static Boolean TryParseArrayArgument<T>(this AttributeData attribute, out T[] value, Int32 position = -1, String propertyName = null)
        {
            var arg = attribute.GetArgument(position, propertyName);

            var result = TryParseArray(arg, out value);

            return result;
        }

        public static Boolean TryParse<T>(this TypedConstant constant, out T value)
        {
            if(constant.Kind != TypedConstantKind.Error && constant.Kind != TypedConstantKind.Array)
            {
                if(constant.Value is T castValue)
                {
                    value = castValue;
                    return true;
                }
            }

            value = default;
            return false;
        }
        public static Boolean TryParseArray<T>(this TypedConstant constant, out T[] values)
        {
            if(constant.Kind is TypedConstantKind.Array)
            {
                var parseResults = constant.Values
                    .Select(c => (success: TryParse(c, out T value), value))
                    .Where(r => r.success)
                    .Select(r => r.value)
                    .ToArray();

                if(parseResults.Length == constant.Values.Length)
                {
                    values = parseResults;
                    return true;
                }
            }

            values = default;
            return false;
        }

        public static TypedConstant GetArgument(this AttributeData attribute, Int32 position = -1, String propertyName = null)
        {
            var namedArgument = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == propertyName);
            if(namedArgument.Value.Kind != TypedConstantKind.Error)
            {
                return namedArgument.Value;
            }

            var positionalArgument = attribute.ConstructorArguments.Skip(position).FirstOrDefault();

            return positionalArgument;
        }
        #endregion

        private static IEnumerable<ITypeIdentifier> GetVariations(ITypeIdentifier attributeIdentifier)
        {
            yield return attributeIdentifier;

            var nameParts = attributeIdentifier.Name.Parts;
            var name = nameParts.Last();

            if(name.Kind != IdentifierParts.Kind.Name)
            {
                //throw instead?
                yield break;
            }

            if(name.Value.EndsWith("Attribute"))
            {
                var shortNameValue = name.Value.Substring(0, name.Value.Length - "Attribute".Length);
                var shortNamePart = IdentifierPart.Name(shortNameValue);
                var shortNameParts = nameParts.RemoveAt(nameParts.Length - 1).Add(shortNamePart);
                var shortName = TypeIdentifierName.Create(shortNameParts);
                var shortIdentifier = TypeIdentifier.Create(shortName, attributeIdentifier.Namespace);
                yield return shortIdentifier;
            }
        }
    }
}
