using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using RhoMicro.CodeAnalysis;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp;
using System.ComponentModel;

namespace RhoMicro.ValueObjectGenerator
{
    internal readonly partial struct ValueObjectGenerationInfo
    {
        internal GeneratedType GeneratePartial()
        {
            var instructions = ObjectInstructions.Create(_typeDeclaration, _semanticModel, _generateAttribute, _fieldAttributes);

            var builder = new StringBuilder();
            BuildParentType(instructions, builder);
            var source = builder.ToString();

            var declarationSymbol = (ITypeSymbol)_semanticModel.GetDeclaredSymbol(_typeDeclaration);
            var identifier = TypeIdentifier.Create(declarationSymbol);
            var result = new GeneratedType(identifier, source);

            return result;
        }
        #region Parent Type
        internal static void Open(ObjectInstructions instr, StringBuilder builder)
        {
            var ns = instr.TypeSymbol.ContainingNamespace;
            if(ns != null)
            {
                builder.Append("namespace ").Append(ns).Append('{').Nl();
            }
            var parent = instr.TypeSymbol.ContainingType;
            while(parent != null)
            {
                var classOrStruct = parent.IsReferenceType ?
                    "class" :
                    "struct";

                builder.Append("partial ").Append(classOrStruct).Append(' ').Append(parent.Name).Append('{').Nl();

                parent = parent.ContainingType;
            }
        }
        internal static void Close(ObjectInstructions instr, StringBuilder builder)
        {
            var ns = instr.TypeSymbol.ContainingNamespace;
            if(ns != null)
            {
                builder.Append('}');
            }
            var parent = instr.TypeSymbol.ContainingType;
            while(parent != null)
            {
                builder.Append('}');

                parent = parent.ContainingType;
            }
        }
        internal static void BuildParentType(ObjectInstructions instr, StringBuilder builder)
        {
            var validatedFields = instr.FieldInstructions
                .Where(f => f.Attribute.IsValidated)
                .ToArray();

            Open(instr, builder);
            ParentTypeSignature(instr, builder);
            BuildValidateResultType(validatedFields, instr.FieldInstructions, builder);
            BuildValidateParametersType(validatedFields, builder);
            BuildIsValidResultType(validatedFields, builder);
            ParentConstructorAndFields(instr, builder);
            ParentValidationAndFactories(instr, builder);
            ParentDeconstructionAndTransformation(instr, builder);
            ParentEqualityAndHashing(instr, builder);
            Close(instr, builder);
        }
        #region Type Signature
        internal static void ParentTypeSignature(ObjectInstructions instr, StringBuilder builder)
        {
            DebuggerDisplayAttribute(instr, builder);
            builder.Append(instr.Visibility)
                .Append(" partial ")
                .Append(instr.StructOrClass)
                .Append(' ')
                .Append(instr.TypeSymbol.Name)
                .Append(" : IEquatable<")
                .Append(instr.TypeSymbol.Name)
                .Append(">{").Nl()
                .Append("#region Nested Types").Nl();
        }
        internal static void DebuggerDisplayAttribute(ObjectInstructions instr, StringBuilder builder)
        {
            if(!instr.Attribute.GenerateDebugDisplay)
            {
                return;
            }

            builder.Nl().Append("[System.Diagnostics.DebuggerDisplay(\"")
                .Append(instr.TypeSymbol.Name)
                .Append(" { ")
                .ForEach(instr.FieldInstructions.Where(f => !f.Attribute.ExcludedFromDebugDisplay), ", ", (b, f) =>
                    b.Append('\"').Append(f.Attribute.Name).Append("\":").Append(f.Attribute.Name))
                .Append(" }")
                .Append("\")]").Nl();
        }
        #endregion
        #region Constructor & Fields
        private static void ParentConstructorAndFields(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.FieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append("#region Constructor & Fields");
            Constructor(instr, builder);
            Fields(instr, builder);
            EmptyField(instr, builder);
            builder.Append("#endregion");
        }
        internal static void Constructor(ObjectInstructions instr, StringBuilder builder)
        {
            if(!instr.Attribute.GenerateConstructor)
            {
                return;
            }

            builder.Append("/// <summary>").Nl()
                .Append("/// Initializes a new instance.").Nl()
                .Append("/// </summary>").Nl()
                .ForEach(instr.FieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).Append("\">").Nl()
                        .Append("/// The value to assign to <see cref=\"").Append(f.Attribute.Name).Append("\"/>.").Nl()
                        .Append("/// </param>"))
                .Append("private ")
                .Append(instr.TypeSymbol.Name)
                .Append('(')
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(')').Nl()
                .Append('{').Nl()
                .ForEach(instr.FieldInstructions, (b, f) =>
                    b.Append(f.Attribute.Name).Append('=').Append(f.InParamName).Append(';').Nl())
                .Append('}').Nl();
        }
        internal static void Fields(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.FieldInstructions.Count == 0)
            {
                return;
            }

            builder.ForEach(instr.FieldInstructions, (b, f) =>
                b.Append("/// <summary>").Nl()
                .Append("/// ").Append(f.Attribute.Summary).Nl()
                .Append("/// <summary>").Nl()
                .Append(Util.GetString(f.Attribute.Visibility))
                .Append(" readonly ")
                .Append(f.Attribute.Type.FullName)
                .Append(' ')
                .Append(f.Attribute.Name)
                .Append(';'));
        }
        internal static void EmptyField(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.StructOrClass != "struct")
            {
                return;
            }

            builder.Append("/// <summary>").Nl()
                .Append("///Gets the default empty instance.").Nl()
                .Append("/// </summary>").Nl()
                .Append("public static readonly ")
                .Append(instr.TypeSymbol.Name)
                .Append(" Empty = default;");
        }
        #endregion
        #region Validation & Factories
        private static void ParentValidationAndFactories(ObjectInstructions instr, StringBuilder builder)
        {
            builder.Append("#region Validation & Factories");
            ValidateMethod(instr, builder);
            IsValidMethod(instr, builder);
            TryCreateMethod(instr, builder);
            CreateMethod(instr, builder);
            builder.Append("#endregion");
        }
        internal static void ValidateMethod(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.ValidatedFieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append("/// <summary>").Nl()
                .Append("/// Validates a set of construction parameters.").Nl()
                .Append("/// </summary>").Nl()
                .Append("/// <param name=\"parameters\">The parameters to validate.</param>").Nl()
                .Append("/// <param name=\"result\">The validation result to communicate validation with.</param>").Nl()
                .Append("static partial void Validate(ValidateParameters parameters, ref ValidateResult result);");
        }
        internal static void IsValidMethod(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.ValidatedFieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append("/// <summary>").Nl()
                .Append("/// Gets a value indicating the validity of the parameters passed, were they to").Nl()
                .Append("/// be used for constructing a new instance of <see cref=\"").Append(instr.TypeSymbol.Name).Append("\"/>.").Nl()
                .Append("/// </summary>")
                .ForEach(instr.ValidatedFieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).Append("\"/>").Nl()
                    .Append("/// The potential <see cref=\"").Append(f.Attribute.Name).Append("\"/> value whose validity to assert.")
                    .Append("/// </param>"))
                .Append("/// <returns>").Nl()
                .Append("/// A value indicating the validity of the parameters passed.")
                .Append("/// </returns>")
                .Append("public static IsValidResult IsValid(")
                .ForEach(instr.ValidatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append('{')
                .Append("var result = ValidateResult.Valid;")
                .Append("var validateParameters = new ValidateParameters(")
                .ForEach(instr.ValidatedFieldInstructions, ", ", (b, f) => b.Append(f.InParamName))
                .Append("Validate(validateParameters, ref result);")
                .Append("return result;").Nl().Append('}');
        }
        internal static void TryCreateMethod(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.ValidatedFieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append("/// <summary>").Nl()
                .Append("/// Attempts to create a new instance of <see cref=\"").Append(instr.TypeSymbol.Name).Append("\"/>.")
                .Append("/// </summary>")
                .ForEach(instr.FieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).Append("\">").Nl()
                    .Append("/// The value to assign to the new instances <see cref=\"").Append(f.Attribute.Name).Append("/>.")
                    .Append("/// </param>"))
                .Append("/// <param name=\"result\">")
                .Append("/// Upon returning, will contain a new instance of <see cref=\"").Append(instr.TypeSymbol.Name).Append("/> if").Nl()
                .Append("/// one could be constructed using the parameters passed; otherwise, <see langword=\"")
                .Append(instr.DefaultOrNull)
                .Append("\"/>.").Nl()
                .Append("/// </param>").Nl()
                .Append("/// <returns>").Nl()
                .Append("/// A value indicating the validity of the parameters passed.").Nl()
                .Append("/// </returns>").Nl()
                .Append("public static IsValidResult TryCreate(")
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(", out ").Append(instr.TypeSymbol.Name).Append(instr.NullPropagatingToken).Append(" result)").Nl()
                .Append('{').Nl()
                .Append("var validateResult = ValidateResult.Valid;").Nl()
                .Append("var validateParameters = new ValidateParameters(")
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.InParamName))
                .Append(");").Nl()
                .Append("Validate(validateParameters, ref validateResult);").Nl()
                .Append("var isValid = validateResult.IsValid;")
                .Append("if(isValid)")
                .Append('{').Nl()
                .Append("result = new ").Append(instr.TypeSymbol.Name).Append('(')
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.InParamName))
                .Append(");").Nl()
                .Append("} else {").Nl()
                .Append("result = ").Append(instr.DefaultOrNull).Append(';').Nl()
                .Append('}').Nl()
                .Append("return validateResult;").Nl()
                .Append('}').Nl();
        }
        internal static void CreateMethod(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.ValidatedFieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append("/// <summary>").Nl()
                .Append("/// Creates a new instance of <see cref = \"").Append(instr.TypeSymbol.Name).Append("\"/>.").Nl()
                .Append("/// </summary>")
                .ForEach(instr.FieldInstructions, (b, f) =>
                    b.Append("/// <param name = \"").Append(f.InParamName).Append("\">").Nl()
                    .Append("/// The value to assign to the new instances <see cref = \"").Append(f.Attribute.Name).Append("\"/>.").Nl()
                    .Append("/// </param>")).Nl()
                .Append("/// <returns>").Nl()
                .Append("/// A new instance of <see cref=\"").Append(instr.TypeSymbol.Name).Append("\"/> if one could be constructed").Nl()
                .Append("/// using the parameters passed; otherwise, an <see cref = \"System.ArgumentException\"/> will be thrown.").Nl()
                .Append("/// </returns>").Nl()
                .Append("/// <exception cref=\"System.ArgumentException\">").Nl()
                .Append("/// Thrown if the parameters passed are not valid for construction.").Nl()
                .Append("/// </exception>").Nl()
                .Append("public static ").Append(instr.TypeSymbol.Name).Append(" Create(")
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(')').Nl()
                .Append('{').Nl()
                .Append("var validateResult = ValidateResult.Valid;").Nl()
                .Append("var validateParameters = new ValidateParameters(")
                .ForEach(instr.FieldInstructions, ", ", (b, f) => b.Append(f.InParamName)).Append(");").Nl()
                .Append("Validate(validateParameters, ref validateResult);").Nl()
                .Append("if (!validateResult.IsValid)").Nl()
                .Append('{').Nl()
                .Append("String reasonMessage = null;").Nl()
                .Append("String paramName = null;").Nl()
                .ForEach(instr.FieldInstructions, $"{Environment.NewLine} else ", (b, f) =>
                    b.Append("if (validateResult.").Append(f.Attribute.Name).Append("IsInvalid)").Nl()
                    .Append('{').Nl()
                    .Append("reasonMessage = validateResult.").Append(f.Attribute.Name).Append("Error;").Nl()
                    .Append("paramName = \"").Append(f.InParamName).Append("\";").Nl()
                    .Append('}')).Nl()
                .Append("String reason = null;").Nl()
                .Append("if (reasonMessage != null)").Nl()
                .Append('{').Nl()
                .Append("reason = $\" Reason: {reasonMessage}\";").Nl()
                .Append('}').Nl()
                .Append("throw new ArgumentException($\"The {paramName} provided for creating an instance of ")
                .Append(instr.TypeSymbol.Name).Append(" was not valid.{reason}\", paramName);").Nl()
                .Append('}').Nl()
                .Append("return new ").Append(instr.TypeSymbol.Name).Append('(')
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.InParamName)).Append(");").Nl()
                .Append('}');
        }
        #endregion
        #region Deconstruction & Transformation
        internal static void ParentDeconstructionAndTransformation(ObjectInstructions instr, StringBuilder builder)
        {
            var anyEligible = instr.FieldInstructions
                 .Where(f => f.Attribute.IsDeconstructable || f.Attribute.SupportsWith)
                 .Any();

            if(!anyEligible)
            {
                return;
            }

            builder.Append("#region Deconstruction & Transformation").Nl();
            Deconstruction(instr, builder);
            Transformation(instr, builder);
            builder.Append("#endregion").Nl();
        }
        internal static void Deconstruction(ObjectInstructions instr, StringBuilder builder)
        {
            var deconstructableFields = instr.FieldInstructions
                 .Where(f => f.Attribute.IsDeconstructable)
                 .ToArray();

            if(deconstructableFields.Length == 0)
            {
                return;
            }

            if(deconstructableFields.Length == 1)
            {
                var field = deconstructableFields[0];
                builder.Append("/// <summary>").Nl()
                    .Append("/// Converts an instance of <see cref=\"").Append(instr.TypeSymbol.Name)
                    .Append("\"/> to its single constituent, <see cref=\"")
                    .Append(instr.TypeSymbol.Name).Append('.').Append(field.Attribute.Name).Append("\"/>.").Nl()
                    .Append("/// </summary>").Nl()
                    .Append("public static implicit operator ").Append(field.Attribute.Type.FullName)
                    .Append('(')
                    .Append(instr.TypeSymbol.Name).Append(" instance) =>").Nl()
                    .Append("instance.").Append(field.Attribute.Name).Append(';').Nl();
            } else
            {
                builder.Append("/// <summary>").Nl()
                    .Append("/// Deconstructs this instance into its constituent values.").Nl()
                    .Append("/// </summary>").Nl()
                    .ForEach(deconstructableFields, (b, f) =>
                        b.Append("/// <param name=\"").Append(f.OutParamName).Append("\">").Nl()
                        .Append("/// The value contained in <see cref=\"").Append(f.Attribute.Name).Append("\"/>.").Nl()
                        .Append("/// </param>"))
                    .Append("public void Deconstruct(")
                    .ForEach(deconstructableFields, ", ", (b, f) =>
                        b.Append("out ").Append(f.Attribute.Type.FullName).Append(' ').Append(f.OutParamName))
                    .Append(')').Nl()
                    .Append('{').Nl()
                    .ForEach(deconstructableFields, (b, f) =>
                        b.Append(f.OutParamName).Append(" = ").Append(f.Attribute.Name))
                    .Append('}').Nl();
            }
        }
        internal static void Transformation(ObjectInstructions instr, StringBuilder builder)
        {
            var transformableFields = instr.FieldInstructions
                .Where(f => f.Attribute.SupportsWith)
                .ToArray();

            if(transformableFields.Length == 0)
            {
                return;
            }

            builder.ForEach(transformableFields, (b, f) =>
                b.Append("/// <summary>").Nl()
                .Append("/// Constructs a shallow copy of this instance with the <see cref=\"")
                .Append(f.Attribute.Name).Append("\"/> value replaced.").Nl()
                .Append("/// </summary>").Nl()
                .Append("/// <param name=\"").Append("f.InParamName").Append("\">").Nl()
                .Append("/// The value to replace <see cref=\"").Append(f.Attribute.Name).Append("\"/> with.")
                .Append("/// </param>").Nl()
                .Append("/// <returns>").Nl()
                .Append("/// A shallow copy of this instance with the <see cref=\"")
                .Append(f.Attribute.Name).Append("\"/> value replaced by <paramref name=\"")
                .Append("\"/>.").Nl()
                .Append("/// </returns>").Nl()
                .Append(Util.GetString(f.Attribute.Visibility)).Append(' ')
                .Append(instr.TypeSymbol.Name).Append(" With").Append(f.Attribute.Name).Append('(')
                .Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName).Append(')').Nl()
                .Append('{').Nl()
                .Append("var result = Create(")
                .ForEach(instr.FieldInstructions, ", ", (b_1, f_1) =>
                    b_1.Append(f_1.Attribute.Name == f.Attribute.Name ? f_1.InParamName : f_1.Attribute.Name))
                .Append(");").Nl()
                .Append("return result;").Nl()
                .Append('}').Nl());
        }
        #endregion
        #region Equality & Hashing
        private static void ParentEqualityAndHashing(ObjectInstructions instr, StringBuilder builder)
        {
            builder.Append("#region Equality & Hashíng").Nl()
                .Append("/// <inheritdoc/>").Nl()
                .Append("public override System.Boolean Equals(System.Object obj)").Nl()
                .Append('{').Nl()
                .Append("return obj is ").Append(instr.TypeSymbol.Name).Append(" instance && Equals(instance);").Nl()
                .Append('}').Nl();

            ParentEqualsMethod(instr, builder);

            builder.Append("/// <inheritdoc/>").Nl()
                .Append("public override System.Int32 GetHashCode()").Nl()
                .Append('{').Nl()
                .Append("return (")
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name))
                .Append(").GetHashCode();").Nl()
                .Append('}').Nl();

            ParentEqualsOperator(instr, builder);

            builder.Append("/// <summary>").Nl()
                .Append("/// Indicates whether two instances of <see cref=\"")
                .Append(instr.TypeSymbol.Name).Append("\"/> are <em>not</em> equal.").Nl()
                .Append(
@"/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// <em>not</em> equal; otherwise, <see langword=""false""/>.
/// </returns>
public static System.Boolean operator !=(")
                .Append(instr.TypeSymbol.Name).Append("left, ")
                .Append(instr.TypeSymbol.Name).Append(" right)").Nl()
                .Append('{').Nl()
                .Append("return !(left == right);")
                .Append('}').Nl()
                .Append("#endregion");
        }
        internal static void ParentEqualsMethod(ObjectInstructions instr, StringBuilder builder)
        {
            builder.Append("/// <inheritdoc/>").Nl()
                .Append("public System.Boolean Equals(").Append(instr.TypeSymbol.Name).Append(" other)").Nl()
                .Append('{').Nl()
                .Append("return ");

            if(instr.StructOrClass == "class")
            {
                builder.Append("other != null && ");
            }

            builder.ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name))
                .Append(").Equals(").Nl()
                .Append('(')
                .ForEach(instr.FieldInstructions, ", ", (b, f) =>
                    b.Append("other.").Append(f.Attribute.Name))
                .Append("));").Nl()
                .Append('}').Nl();
        }
        internal static void ParentEqualsOperator(ObjectInstructions instr, StringBuilder builder)
        {
            builder.Append("/// <summary>").Nl()
                .Append("/// Indicates whether two instances of <see cref=\"")
                .Append(instr.TypeSymbol.Name).Append("\"/> are equal.").Nl();

            var isStruct = instr.StructOrClass == "struct";
            if(isStruct)
            {
                builder.Append(
@"/// <para>
/// <paramref name=""left""/> and <paramref name=""right"" /> are considered equal if 
/// <c><paramref name=""left""/>.Equals(<paramref name=""right"" />)</c> evaluates to <see langword=""true"" />.
/// </para>").Nl();
            } else
            {
                builder.Append(
@"/// <para>
/// <paramref name=""left""/> and <paramref name=""right"" /> are considered equal if 
/// </para>
/// <para>
/// both are <see langword=""null""/>
/// </para>
/// <para>
/// or
/// </para>
/// <para>
/// neither is <see langword=""null""/> and 
/// <c><paramref name=""left""/>.Equals(<paramref name=""right"" />)</c> evaluates to <see langword=""true"" />.
/// </para>").Nl();
            }

            builder.Append(
@"/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// equal; otherwise, <see langword=""false""/>.
/// </returns>
public static System.Boolean operator ==(")
                .Append(instr.TypeSymbol.Name).Append(" left, ")
                .Append(instr.TypeSymbol.Name).Append(" right)").Nl()
                .Append('{').Nl();

            if(isStruct)
            {
                builder.Append("left.Equals(right)");
            } else
            {
                builder.Append("left == null ? right == null : right == null ? left == null : left.Equals(right)");
            }

            builder.Append(';').Nl()
                .Append('}').Nl();
        }
        #endregion
        #endregion
        #region ValidateParameters Type
        internal static void ValidateParametersType(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.ValidatedFieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append(
@"/// <summary>
/// Wrapper type around possible construction parameters; used for validation.
/// </summary>
private readonly struct ValidateParameters : IEquatable<ValidateParameters>
{{").Nl();

            ValidateParametersConstructorsAndFields(instr, builder);
            ValidateParametersDeconstruction(instr, builder);
            ValidateParametersEqualityAndHashing(instr, builder);

            builder.Append('}').Nl();
        }
        internal static void ValidateParametersConstructorsAndFields(ObjectInstructions instr, StringBuilder builder)
        {
            builder.Append(
@"#region Constructors & Fields
/// <summary>
/// Initializes a new instance.
/// </summary>")
                .ForEach(instr.ValidatedFieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).Append("\">").Nl()
                    .Append("/// The value for <see cref=\"").Append(f.Attribute.Name).Append("\"/> to validate.").Nl()
                    .Append("/// </param>")).Nl()
                .Append("public ValidateParameters(")
                .ForEach(instr.ValidatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(')').Nl()
                .Append('{').Nl()
                .ForEach(instr.ValidatedFieldInstructions, (b, f) =>
                    b.Append(f.Attribute.Name).Append(" = ").Append(f.InParamName).Append(';')).Nl()
                .Append('}').Nl()
                .ForEach(instr.ValidatedFieldInstructions, (b, f) =>
                    b.Append("/// <summary>").Nl()
                    .Append("/// The value for <see cref=\"").Append(f.Attribute.Name).Append("\"/> to validate.").Nl()
                    .Append("/// </summary>").Nl()
                    .Append("public readonly ").Append(f.Attribute.Type.FullName).Append(' ')
                    .Append(f.Attribute.Name)).Nl()
                .Append("#endregion");
        }
        internal static void ValidateParametersDeconstruction(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.ValidatedFieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append("#region Deconstruction");

            if(instr.ValidatedFieldInstructions.Count == 1)
            {
                var field = instr.ValidatedFieldInstructions[0];
                builder.Append("/// <summary>").Nl()
                    .Append("/// Converts an instance of <see cref=\"\"ValidateParameters\"\"/> to its single constituent, <see cref=\"ValidateParameters.")
                    .Append(field.Attribute.Name).Append("\"/>.").Nl()
                    .Append("/// </summary>").Nl()
                    .Append("public static implicit operator ")
                    .Append(field.Attribute.Type.FullName).Append("(ValidateParameters instance) =>").Nl()
                    .Append("instance.").Append(field.Attribute.Name).Append(';');
            } else
            {
                builder.Append(
@"/// <summary>
/// Deconstructs this instance into its constituent values.
/// </summary>").Nl()
                    .ForEach(instr.ValidatedFieldInstructions, (b, f) =>
                        b.Append("/// <param name=\"").Append(f.OutParamName)
                        .Append("\">").Nl()
                        .Append("/// The value contained in <see cref=\"")
                        .Append(f.Attribute.Name).Append("\"/>.").Nl()
                        .Append("/// </param>")).Nl()
                    .Append("public void Deconstruct(")
                    .ForEach(instr.ValidatedFieldInstructions, ", ", (b, f) =>
                        b.Append("out ").Append(f.Attribute.Type.FullName).Append(' ').Append(f.OutParamName))
                    .Append(')').Nl()
                    .Append('{').Nl()
                    .ForEach(instr.ValidatedFieldInstructions, (b, f) =>
                        b.Append(f.OutParamName).Append(" = ").Append(f.Attribute.Name).Append(';')).Nl()
                    .Append('}').Nl();
            }

            builder.Append("#endregion");
        }
        internal static void ValidateParametersEqualityAndHashing(ObjectInstructions instr, StringBuilder builder)
        {
            builder.Append(
@"#region Equality & Hashing
/// <inheritdoc/>
public override System.Boolean Equals(System.Object obj)
{{
	return obj is ValidateParameters address && Equals(address);
}}
/// <inheritdoc/>
public System.Boolean Equals(ValidateParameters other)
{{").Nl()
                .Append("return (")
                .ForEach(instr.ValidatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name))
                .Append(").Equals((")
                .ForEach(instr.ValidatedFieldInstructions, ", ", (b, f) =>
                    b.Append("other.").Append(f.Attribute.Name))
                .Append("));").Nl()
                .Append('}').Nl()
                .Append(
@"/// <inheritdoc/>
public override System.Int32 GetHashCode()
{{").Nl()
                .Append("return (")
                .ForEach(instr.ValidatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name))
                .Append(").GetHashCode();").Nl()
                .Append('}').Nl()
                .Append(
@"/// <summary>
/// Indicates whether two instances of <see cref=""ValidateParameters""/> are equal.
/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// equal; otherwise, <see langword=""false""/>.
/// </returns>
public static System.Boolean operator ==(ValidateParameters left, ValidateParameters right)
{{
	return left.Equals(right);
}}
/// <summary>
/// Indicates whether two instances of <see cref=""ValidateParameters""/> are <em>not</em> equal.
/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// <em>not</em> equal; otherwise, <see langword=""false""/>.
/// </returns>
public static System.Boolean operator !=(ValidateParameters left, ValidateParameters right)
{{
	return !(left == right);
}}
#endregion");
        }
        #endregion
        #region ValidateResult Type
        internal static void ValidateResultFieldsAndProperties(ObjectInstructions instr, StringBuilder builder)
        {
            builder.Append("#region Fields & Properties").Nl()
                .ForEach(instr.ValidatedFieldInstructions, (b, f) =>
                    b.Append("/// <summary>").Nl()
                    .Append("/// Indicates whether the value provided by <see cref=\"ValidateParameters.")
                    .Append(f.Attribute.Name).Append("\"/> is invalid.").Nl()
                    .Append("/// </summary>").Nl()
                    .Append("public System.Boolean ").Append(f.Attribute.Name).Append("IsInvalid;").Nl()
                    .Append(
@"/// <summary>
/// Contains the error message to include in instances of <see cref=""System.ArgumentException""/> thrown 
/// by <see cref=""Create(")
                    .ForEach(instr.ValidatedFieldInstructions, ", ", (b_1, f_1) =>
                        b_1.Append(f_1.Attribute.Type.FullName))
                    .Append(")\"/> if <see cref=\"")
                    .Append(f.Attribute.Name)
                    .Append("IsInvalid\"/> is set to <see langword=\"true\"/>.").Nl()
                    .Append("/// </summary>").Nl()
                    .Append("public System.String ").Append(f.Attribute.Name).Append("Error;").Nl())
                .Append(
@"/// <summary>
/// Gets a default (valid) instance.
/// </summary>
public static ValidateResult Valid => default;

/// <summary>
/// Gets a value indicating whether none of the validation fields have been set to <see langword=""true""/>.
/// </summary>
public System.Boolean IsValid =>")
                .ForEach(instr.ValidatedFieldInstructions, "&& ", (b, f) =>
                    b.Append('!').Append(f.Attribute.Name).Append("IsInvalid"))
                .Append(';').Nl();
        }
        internal static void BuildValidateResultType(ObjectInstructions instr, StringBuilder builder)
        {
            if(instr.ValidatedFieldInstructions.Count == 0)
            {
                return;
            }

            builder.Append(
@"/// <summary>
/// Communicates detailed validation results.
/// </summary>
/// private ref struct ValidateResult
{").Nl();
            ValidateResultFieldsAndProperties(instr, builder);
            /*
            

            var result =
$@"
			
			#region Conversion Operators
			/// <summary>
			/// Implicitly converts a mutable instance of <see cref=""ValidateResult""/> to 
			/// an immutable instance of <see cref=""IsValidResult""/>.
			/// </summary>
			/// <param name=""result"">The instance to implicitly convert.</param>
			public static implicit operator IsValidResult(ValidateResult result)=>
				new IsValidResult({PFI(validatedFields, ", ", f => $"result.{f.Attribute.Name}IsInvalid, result.{f.Attribute.Name}Error")});
			#endregion
			#region Equals & Hashing
			/// <summary>
			/// <see cref=""ValidateResult""/> may not be boxed due to being a <see langword=""ref""/> <see langword=""struct""/>. 
			/// Therefore, calling <see cref=""Equals(System.Object)""/> is not supported.
			/// </summary>
			/// <exception cref=""NotSupportedException""></exception>
			public override System.Boolean Equals(System.Object obj)
			{{
				throw new NotSupportedException(""Address.ValidateResult may not be boxed due to being a ref struct. Therefore, calling Equals(System.Object) is not supported."");
			}}
			/// <summary>
			/// <see cref=""ValidateResult""/> does not support calling <see cref=""GetHashCode""/>.
			/// </summary>
			/// <exception cref=""NotSupportedException""></exception>
			public override System.Int32 GetHashCode()
			{{
				throw new NotSupportedException();
			}}
			#endregion
		}}";
            */
        }
        #endregion
        #region IsValidResult Type
        internal static void GetIsValidResultCtor(IReadOnlyList<FieldInstructions> validatedFields)
        {
            if(validatedFields.Count == 0)
            {
                return String.Empty;
            }

            var result =
        $@"			/// <summary>
			/// Initializes a new instance.
			/// </summary>
{PFI(validatedFields, "\r\n", f =>
        $@"			/// <param name=""{f.InParamName}IsInvalid"">
			/// Indicates whether the parameter provided for <see cref=""{f.Attribute.Name}""/> was invalid.
			/// </param>")}
			public IsValidResult({PFI(validatedFields, ", ", f => $"System.Boolean {f.InParamName}IsInvalid, System.String {f.InParamName}Error")})
			{{
{PFI(validatedFields, "\r\n\r\n", f =>
        $@"				{f.Attribute.Name}IsInvalid = {f.InParamName}IsInvalid;
				{f.Attribute.Name}Error = {f.InParamName}Error;")}
			}}";

            return result;
        }
        internal static void GetIsValidResultDeconstruction(IReadOnlyList<FieldInstructions> validatedFields)
        {
            if(validatedFields.Count == 0)
            {
                return String.Empty;
            }

            var result =
        $@"			/// <summary>
			/// Deconstructs this instance into its constituent values.
			/// </summary>
{PFI(validatedFields, "\r\n", f =>
        $@"			/// <param name=""{f.OutParamName}IsInvalid"">
			/// The value contained in <see cref=""{f.Attribute.Name}IsInvalid""/>.
			/// </param>")}
			public void Deconstruct({PFI(validatedFields, ", ", f => $"System.Boolean {f.OutParamName}IsInvalid, System.String {f.OutParamName}Error")})
			{{
{PFI(validatedFields, "\r\n", f =>
        $@"				{f.OutParamName}IsInvalid = {f.Attribute.Name}IsInvalid;
				{f.OutParamName}Error = {f.Attribute.Name}Error;")}
			}}";

            return result;
        }
        internal static void GetIsValidResultConversion(IReadOnlyList<FieldInstructions> validatedFields)
        {
            var (body, comment) = validatedFields.Count > 0 ?
                (PFI(validatedFields, " &&\r\n", f =>
        $@"				!result.{f.Attribute.Name}IsInvalid"), "The result will be true if all validity fields evaluate to <see langword=\"false\"/>.") :
                ("true", "As no fields are being validated, the result will always be <see langword=\"true\"/>");

            var result =
        $@"			/// <summary>
			/// Implicitly converts an instance of <see cref=""IsValidResult""/> to <see cref=""System.Boolean""/>.
			/// {comment}
			/// </summary>
			/// <param name=""result"">The instance to implicitly convert.</param>
			public static implicit operator System.Boolean(IsValidResult result)=>
{body};";

            return result;
        }
        internal static void GetIsValidResultEquals(IReadOnlyList<FieldInstructions> validatedFields)
        {
            var body = validatedFields.Count == 1 ?
                $"System.Collections.Generic.EqualityComparer<{validatedFields[0].Attribute.Type.FullName}>.Default.Equals({validatedFields[0].Attribute.Name}.IsInvalid, other.{validatedFields[0].Attribute.Name}.IsInvalid)" :
                $@"({PFI(validatedFields, ", ", f => $"{f.Attribute.Name}IsInvalid")}).Equals(
					({PFI(validatedFields, ", ", f => $"other.{f.Attribute.Name}IsInvalid")}))";

            var result =
        $@"			/// <inheritdoc/>
			public System.Boolean Equals(IsValidResult other) =>
				{body};";

            return result;
        }
        internal static void GetIsValidResultGetHashcode(IReadOnlyList<FieldInstructions> validatedFields)
        {
            var body = validatedFields.Count == 1 ?
                $"System.Collections.Generic.EqualityComparer<{validatedFields[0].Attribute.Type.FullName}>.Default.GetHashCode({validatedFields[0].Attribute.Name}.IsInvalid)" :
                $@"({PFI(validatedFields, ", ", f => $"{f.Attribute.Name}IsInvalid")}).GetHashCode()";

            var result =
        $@"			/// <inheritdoc/>
			public System.Int32 GetHashCode() =>
				{body};";

            return result;
        }
        internal static void BuildIsValidResultType(IReadOnlyList<FieldInstructions> validatedFields, StringBuilder builder)
        {
            if(validatedFields.Count == 0)
            {
                return String.Empty;
            }

            var result =
        $@"		/// <summary>
		/// Contains validation results for construction parameters.
		/// </summary>
		public readonly struct IsValidResult : IEquatable<IsValidResult>
		{{
			#region Constructor & Fields
{GetIsValidResultCtor(validatedFields)}
{PFI(validatedFields, "\r\n", f =>
        $@"			/// <summary>
			/// Indicates whether the parameter provided for <see cref=""{f.Attribute.Name}""/> was invalid.
			/// </summary>
			public readonly System.Boolean {f.Attribute.Name}IsInvalid;
			/// <summary>
			/// Contains a validation description if <see cref=""{f.Attribute.Name}IsInvalid""/> is set to <see langword=""true""/>.
			/// </summary>
			public readonly System.String {f.Attribute.Name}Error;")}
			#endregion
			#region Conversion & Deconstruction
{GetIsValidResultConversion(validatedFields)}
{GetIsValidResultDeconstruction(validatedFields)}
			#endregion
			#region Equality & Hashing
			/// <inheritdoc/>
			public override System.Boolean Equals(System.Object obj)
			{{
				return obj is IsValidResult result && Equals(result);
			}}
{GetIsValidResultEquals(validatedFields)}
{GetIsValidResultGetHashcode(validatedFields)}
			/// <summary>
			/// Indicates whether two instances of <see cref=""IsValidResult""/> are equal.
			/// </summary>
			/// <param name=""left"">The left operand.</param>
			/// <param name=""left"">The right operand.</param>
			/// <returns>
			/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
			/// equal; otherwise, <see langword=""false""/>.
			/// </returns>
			public static System.Boolean operator ==(IsValidResult left, IsValidResult right)
			{{
				return left.Equals(right);
			}}
			/// <summary>
			/// Indicates whether two instances of <see cref=""IsValidResult""/> are <em>not</em> equal.
			/// </summary>
			/// <param name=""left"">The left operand.</param>
			/// <param name=""left"">The right operand.</param>
			/// <returns>
			/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
			/// <em>not</em> equal; otherwise, <see langword=""false""/>.
			/// </returns>
			public static System.Boolean operator !=(IsValidResult left, IsValidResult right)
			{{
				return !(left == right);
			}}
			#endregion
		}}";

            return result;
        }
        #endregion
    }
}
