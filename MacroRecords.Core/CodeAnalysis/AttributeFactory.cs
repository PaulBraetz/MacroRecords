using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RhoMicro.CodeAnalysis.Attributes
{
    internal abstract class AttributeFactory<T> : IAttributeFactory<T>
    {
        public static IAttributeFactory<T> Create()
        {
            var collection = new AttributeFactoryCollection<T>();

            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.CanWrite).ToArray();
            var constructors = type.GetConstructors().Where(c => !c.IsStatic).ToArray();

            for(var ctorIndex = 0; ctorIndex < constructors.Length; ctorIndex++)
            {
                var constructor = constructors[ctorIndex];

                var parameters = new ParameterExpression[2]
                {
                            Expression.Parameter(typeof(AttributeSyntax), "attributeData"),
                            Expression.Parameter(typeof(SemanticModel), "semanticModel"),
                };
                var canBuildStrategy = getCanBuildStrategy();
                var buildStrategy = getBuildStrategy();
                var factory = Create(canBuildStrategy, buildStrategy);

                _ = collection.Add(factory);

                Func<AttributeSyntax, SemanticModel, T> getBuildStrategy()
                {
                    var canBuildTest = getCanBuildTest();
                    var ifTrue = getBuildExpr();
                    var ifFalse = getThrowExpr($"Cannot build {typeof(T)} using the attribute syntax and semantic model provided.");
                    var body = Expression.Condition(canBuildTest, ifTrue, ifFalse);

                    var lambda = Expression.Lambda(body, parameters);
                    var strategy = (Func<AttributeSyntax, SemanticModel, T>)lambda.Compile();

                    return strategy;

                    Expression getBuildExpr()
                    {
                        var ctorParams = constructor.GetParameters().ToArray();

                        var newInstanceExpr = Expression.Variable(type, "instance");

                        var blockVariables = new List<ParameterExpression>();
                        var blockExpressions = new List<Expression>();
                        var typeParamSetExpressions = new List<Expression>();
                        var typeParamVariables = new List<ParameterExpression>();

                        var implementsTypeParameterSetter = 
                            type.TryGetMethodSemantically(typeof(IHasTypeConstructorParameter).GetMethod(nameof(IHasTypeConstructorParameter.SetTypeParameter)), out var typeParameterSetter);

                        for(var i = 0; i < ctorParams.Length; i++)
                        {
                            var parameter = ctorParams[i];

                            var outValueType = parameter.ParameterType;
                            var tryParseMethod = getTryParseMethod(outValueType);
                            var outValue = Expression.Parameter(outValueType, $"argumentValue_{parameter.Name}");

                            blockVariables.Add(outValue);

                            Expression paramAssignmentExpr = null;

                            if(!(parameter.ParameterType == typeof(Type) && implementsTypeParameterSetter))
                            {
                                var callExpr = Expression.Call(null, tryParseMethod, parameters[0], parameters[1], outValue, Expression.Constant(i), Expression.Convert(Expression.Constant(null), typeof(String)), Expression.Constant(parameter.Name));

                                var noArgReactionExpr = parameter.HasDefaultValue
                                    ? Expression.Assign(outValue, Expression.Convert(Expression.Constant(parameter.DefaultValue), parameter.ParameterType))
                                    : getThrowExpr($"Missing argument for {parameter.Name} of type {parameter.ParameterType} encountered while attempting to construct instance of type {typeof(T)}.");
                                paramAssignmentExpr = Expression.IfThen(Expression.Not(callExpr), noArgReactionExpr);
                            } else
                            {
                                paramAssignmentExpr = Expression.Assign(outValue, Expression.Convert(Expression.Constant(null), outValueType));

                                outValueType = typeof(Object);
                                tryParseMethod = getTryParseMethod(outValueType);
                                outValue = Expression.Parameter(outValueType, $"typeArgumentValue_{parameter.Name}");
                                var callExpr = Expression.Call(null, tryParseMethod, parameters[0], parameters[1], outValue, Expression.Constant(i), Expression.Convert(Expression.Constant(null), typeof(String)), Expression.Constant(parameter.Name));
                                var typeParamSetExpr = Expression.Call(newInstanceExpr, typeParameterSetter, Expression.Constant(parameter.Name), outValue);
                                var conditionalSetExpr = Expression.IfThen(callExpr, typeParamSetExpr);

                                typeParamVariables.Add(outValue);
                                typeParamSetExpressions.Add(conditionalSetExpr);
                            }

                            blockExpressions.Add(paramAssignmentExpr);
                        }

                        var newExpr = Expression.New(constructor, blockVariables);
                        var newInstanceAssignmentExpr = Expression.Assign(newInstanceExpr, newExpr);

                        blockVariables.AddRange(typeParamVariables);
                        blockVariables.Add(newInstanceExpr);
                        blockExpressions.Add(newInstanceAssignmentExpr);
                        blockExpressions.AddRange(typeParamSetExpressions);

                        var implementsTypePropertySetter = type.TryGetMethodSemantically(typeof(IHasTypePropertySetter).GetMethod(nameof(IHasTypePropertySetter.SetTypeProperty)), out var typePropertySetter);

                        for(var i = 0; i < properties.Length; i++)
                        {
                            var property = properties[i];

                            var outValueType = property.PropertyType;
                            ParameterExpression outValue;
                            Expression setConditionExpr;
                            Expression setExpr;
                            if(property.PropertyType == typeof(Type) && implementsTypePropertySetter)
                            {
                                setExpr = Expression.Call(newInstanceExpr, property.SetMethod, Expression.Convert(Expression.Constant(null), outValueType));

                                outValueType = typeof(Object);
                                var tryParseMethod = getTryParseMethod(outValueType);
                                outValue = Expression.Parameter(outValueType, $"typePropertyValue_{property.Name}");
                                var helperCallExpr = Expression.Call(null, tryParseMethod, parameters[0], parameters[1], outValue, Expression.Constant(-1), Expression.Constant(property.Name), Expression.Convert(Expression.Constant(null), typeof(String)));
                                var helperSetExpr = Expression.Call(newInstanceExpr, typePropertySetter, Expression.Constant(property.Name), outValue);

                                var conditionalBlock = Expression.Block(setExpr, helperSetExpr);

                                setConditionExpr = Expression.IfThen(helperCallExpr, conditionalBlock);
                            } else
                            {
                                var tryParseMethod = getTryParseMethod(outValueType);
                                outValue = Expression.Parameter(outValueType, $"propertyValue_{property.Name}");
                                var callExpr = Expression.Call(null, tryParseMethod, parameters[0], parameters[1], outValue, Expression.Constant(-1), Expression.Constant(property.Name), Expression.Convert(Expression.Constant(null), typeof(String)));
                                setExpr = Expression.Call(newInstanceExpr, property.SetMethod, outValue);
                                setConditionExpr = Expression.IfThen(callExpr, setExpr);
                            }

                            blockVariables.Add(outValue);
                            blockExpressions.Add(setConditionExpr);
                        }

                        blockExpressions.Add(newInstanceExpr);

                        var block = Expression.Block(blockVariables, blockExpressions);

                        return block;

                        MethodInfo getTryParseMethod(Type forType)
                        {
                            var name = forType.IsArray ?
                                nameof(Extensions.TryParseArrayArgument) :
                                nameof(Extensions.TryParseArgument);
                            var constraint = forType.IsArray ? forType.GetElementType() : forType;

                            var method = typeof(Extensions).GetMethods()
                                .Where(m => m.IsGenericMethod)
                                .Select(m => m.MakeGenericMethod(constraint))
                                .Single(m =>
                                {
                                    var methodParams = m.GetParameters();
                                    return m.Name == name &&
                                        methodParams.Length == 6 &&
                                        methodParams[0].ParameterType == typeof(AttributeSyntax) &&
                                        methodParams[1].ParameterType == typeof(SemanticModel) &&
                                        methodParams[2].ParameterType == forType.MakeByRefType() &&
                                        methodParams[3].ParameterType == typeof(Int32) &&
                                        methodParams[4].ParameterType == typeof(String) &&
                                        methodParams[5].ParameterType == typeof(String);
                                });

                            return method;
                        }
                    }

                    Expression getThrowExpr(String message)
                    {
                        var ctorInfo = typeof(InvalidOperationException).GetConstructor(new[] { typeof(String) });
                        var ctorParam = Expression.Constant(message);
                        var throwExpr = Expression.Throw(Expression.New(ctorInfo, ctorParam));
                        var returnExpr = Expression.Convert(Expression.Constant(null), type);
                        var throwBlock = Expression.Block(throwExpr, returnExpr);

                        return throwBlock;
                    }
                }

                Func<AttributeSyntax, SemanticModel, Boolean> getCanBuildStrategy()
                {
                    var body = getCanBuildTest();

                    var lambda = Expression.Lambda(body, parameters);
                    var strategy = (Func<AttributeSyntax, SemanticModel, Boolean>)lambda.Compile();

                    return strategy;
                }

                Expression getCanBuildTest()
                {
                    var typeExpr = Expression.Constant(type, typeof(Type));
                    var getCtorsMethod = typeof(Type).GetMethod(nameof(Type.GetConstructors), Array.Empty<Type>());
                    var getCtorsCallExpr = Expression.Call(typeExpr, getCtorsMethod);

                    var whereMethod = typeof(Enumerable).GetMethods()
                        .Where(m => m.Name == nameof(Enumerable.Where))
                        .Select(m => m.MakeGenericMethod(typeof(ConstructorInfo)))
                        .Single(m =>
                        {
                            var methodParameters = m.GetParameters();
                            var match = methodParameters.Length == 2 &&
                                        methodParameters[0].ParameterType == typeof(IEnumerable<ConstructorInfo>) &&
                                        methodParameters[1].ParameterType == typeof(Func<ConstructorInfo, Boolean>);

                            return match;
                        });
                    var predicateParamExpr = Expression.Parameter(typeof(ConstructorInfo), "c");
                    var isStaticMethod = typeof(ConstructorInfo).GetProperty(nameof(ConstructorInfo.IsStatic)).GetMethod;
                    var predicateExpr = Expression.Lambda(Expression.Not(Expression.Call(predicateParamExpr, isStaticMethod)), predicateParamExpr);
                    var whereCallExpr = Expression.Call(null, whereMethod, getCtorsCallExpr, predicateExpr);

                    var toArrayMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToArray)).MakeGenericMethod(typeof(ConstructorInfo));
                    var toArrayCall = Expression.Call(null, toArrayMethod, whereCallExpr);

                    var ctorIndexExpr = Expression.Constant(ctorIndex);
                    var indexAccessExpr = Expression.ArrayIndex(toArrayCall, ctorIndexExpr);

                    var matchesMethod = typeof(Extensions).GetMethods()
                        .Single(m =>
                        {
                            var methodParams = m.GetParameters();
                            return m.Name == nameof(Extensions.Matches) &&
                                methodParams.Length == 3 &&
                                methodParams[0].ParameterType == typeof(AttributeSyntax) &&
                                methodParams[1].ParameterType == typeof(SemanticModel) &&
                                methodParams[2].ParameterType == typeof(ConstructorInfo);
                        });
                    var matchesCall = Expression.Call(null, matchesMethod, parameters[0], parameters[1], indexAccessExpr);

                    return matchesCall;
                }
            }

            return collection;
        }
        public static IAttributeFactory<T> Create(Func<AttributeSyntax, SemanticModel, Boolean> canBuildStrategy, Func<AttributeSyntax, SemanticModel, T> buildStrategy) => new AttributeFactoryStrategy<T>(canBuildStrategy, buildStrategy);
        public static IAttributeFactory<T> Create(TypeIdentifier typeIdentifier, Func<AttributeSyntax, SemanticModel, T> buildStrategy) => new AttributeFactoryStrategy<T>((d, s) => d.IsType(s, typeIdentifier), buildStrategy);

        protected abstract T Build(AttributeSyntax attributeData, SemanticModel semanticModel);
        protected abstract Boolean CanBuild(AttributeSyntax attributeData, SemanticModel semanticModel);
        public Boolean TryBuild(AttributeSyntax attributeData, SemanticModel semanticModel, out T attribute)
        {
            if(CanBuild(attributeData, semanticModel))
            {
                attribute = Build(attributeData, semanticModel);
                return true;
            }

            attribute = default;
            return false;
        }
    }
}
