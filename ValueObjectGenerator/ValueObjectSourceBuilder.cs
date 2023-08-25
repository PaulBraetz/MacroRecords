using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using RhoMicro.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Xml.Linq;
using System.Text;

namespace RhoMicro.ValueObjectGenerator
{
    internal sealed class ValueObjectSourceBuilder
    {
        #region Constructor
        private ValueObjectSourceBuilder(
            String visibility,
            String structOrClass,
            String defaultOrNull,
            Boolean generateToString,
            IReadOnlyList<FieldInstructions> fieldInstructions,
            String nullPropagatingToken,
            GeneratedValueObjectAttribute attribute,
            INamedTypeSymbol typeSymbol)
        {
            _visibility = visibility;
            _structOrClass = structOrClass;
            _defaultOrNull = defaultOrNull;
            _generateToString = generateToString;
            _fieldInstructions = fieldInstructions;
            _nullPropagatingToken = nullPropagatingToken;
            _attribute = attribute;
            _typeSymbol = typeSymbol;

            _validatedFieldInstructions = _fieldInstructions.Where(f => f.Attribute.IsValidated).ToArray();

            _builder = new StringBuilder();
        }

        private readonly GeneratedValueObjectAttribute _attribute;
        private readonly String _visibility;
        private readonly String _structOrClass;
        private readonly String _defaultOrNull;
        private readonly Boolean _generateToString;
        private readonly IReadOnlyList<FieldInstructions> _fieldInstructions;
        private readonly IReadOnlyList<FieldInstructions> _validatedFieldInstructions;
        private readonly String _nullPropagatingToken;
        private readonly INamedTypeSymbol _typeSymbol;
        private readonly StringBuilder _builder;
        #endregion
        #region Factory
        public static Boolean TryCreate(
            TypeDeclarationSyntax declaration,
            SemanticModel semanticModel,
            out ValueObjectSourceBuilder builder)
        {
            builder = null;
            if(!(declaration is StructDeclarationSyntax || declaration is ClassDeclarationSyntax))
            {
                return false;
            }

            var (head, fields) = GetAttributes(declaration, semanticModel);
            if(head == null)
            {
                return false;
            }

            builder = CreateBuilder(declaration, semanticModel, head, fields);
            return true;
        }
        private static ValueObjectSourceBuilder CreateBuilder(
            TypeDeclarationSyntax declaration,
            SemanticModel semanticModel,
            GeneratedValueObjectAttribute head,
            IReadOnlyList<FieldAttribute> fields)
        {
            ValueObjectSourceBuilder builder;
            var declarationSymbol = semanticModel.GetDeclaredSymbol(declaration);
            var visibility = GetVisibility(declaration);
            var (structOrClass, defaultOrNull) = GetStructOrClass(declaration);
            var generateToString = GetGenerateToString(declaration);
            var fieldInstructions = fields
                .Select(f => (success: FieldInstructions.TryCreate(f, out var instruction), instruction))
                .Where(t => t.success)
                .Select(t => t.instruction)
                .ToArray();
            var nullPropagatingToken = structOrClass == "class" ?
                "?" :
                String.Empty;
            builder = new ValueObjectSourceBuilder(
                visibility,
                structOrClass,
                defaultOrNull,
                generateToString,
                fieldInstructions,
                nullPropagatingToken,
                head,
                declarationSymbol);
            return builder;
        }

        private static Boolean GetGenerateToString(TypeDeclarationSyntax declaration)
        {
            var result = declaration.Members
                .OfType<MethodDeclarationSyntax>()
                .Any(m => m.Modifiers.Any(t => t.IsKind(SyntaxKind.OverrideKeyword)) &&
                        m.Identifier.ValueText == nameof(ToString));

            return result;
        }
        private static (String structOrClass, String defaultOrNull) GetStructOrClass(TypeDeclarationSyntax declaration) =>
            declaration.Keyword.ValueText == "struct" ?
                ("struct", "default") :
                ("class", "null");
        private static String GetVisibility(TypeDeclarationSyntax declaration)
        {
            var modifiers = declaration.Modifiers.Where(IsVisibility).Select(m => m.ValueText);
            var result = String.Join(" ", modifiers);

            return result;
        }
        private static Boolean IsVisibility(SyntaxToken token) =>
            token.IsKind(SyntaxKind.PublicKeyword) ||
            token.IsKind(SyntaxKind.PrivateKeyword) ||
            token.IsKind(SyntaxKind.InternalKeyword) ||
            token.IsKind(SyntaxKind.ProtectedKeyword);
        private static (GeneratedValueObjectAttribute head, IReadOnlyList<FieldAttribute> fields) GetAttributes(TypeDeclarationSyntax declaration, SemanticModel semanticModel)
        {
            var attributes = declaration.AttributeLists
                .OfAttributeClasses(
                semanticModel,
                AttributeUnits.GeneratedValueObject.GeneratedType.Identifier,
                AttributeUnits.GeneratedValueObjectField.GeneratedType.Identifier);

            var head = attributes
                .Select(a => (success: AttributeUnits.GeneratedValueObject.Factory.TryBuild(a, semanticModel, out var result), result))
                .Where(t => t.success)
                .Select(t => t.result)
                .FirstOrDefault();

            var fields = attributes
                .Select(a => (success: AttributeUnits.GeneratedValueObjectField.Factory.TryBuild(a, semanticModel, out var result), result))
                .Where(t => t.success)
                .Select(t => t.result)
                .ToArray();

            return (head, fields);
        }
        #endregion
        #region Build Method
        public GeneratedType Build()
        {
            var identifier = TypeIdentifier.Create(_typeSymbol);
            var source = BuildCore();
            var result = new GeneratedType(identifier, source);
            return result;
        }
        public String BuildCore() => _builder.ToString();
        #endregion
        #region Parent Type
        public ValueObjectSourceBuilder AddParentType()
        {
            AddParentOpen();
            AddParentTypeSignatureAndAttributes();
            _builder.Append('{');
            AddNestedTypes();
            AddParentConstructorAndFields();
            AddParentValidationAndFactories();
            AddParentDeconstructionAndTransformation();
            AddParentEqualityAndHashing();
            _builder.Append('}');
            AddParentClose();

            return this;
        }
        public ValueObjectSourceBuilder AddNestedTypes()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("#region Nested Types");
            AddValidateResultType();
            AddValidateParametersType();
            AddIsValidResultType();
            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddParentOpen()
        {
            var ns = _typeSymbol.ContainingNamespace;
            if(ns != null)
            {
                _builder.Append("namespace ").Append(ns).Append('{');
            }

            var parent = _typeSymbol.ContainingType;
            while(parent != null)
            {
                var classOrStruct = parent.IsReferenceType ?
                    "class" :
                    "struct";

                _builder.Append("partial ").Append(classOrStruct).Append(' ').Append(parent.Name).Append('{');

                parent = parent.ContainingType;
            }

            return this;
        }
        public ValueObjectSourceBuilder AddParentClose()
        {
            var ns = _typeSymbol.ContainingNamespace;
            if(ns != null)
            {
                _builder.Append('}');
            }

            var parent = _typeSymbol.ContainingType;
            while(parent != null)
            {
                _builder.Append('}');

                parent = parent.ContainingType;
            }

            return this;
        }
        #region Type Signature & Attributes
        public ValueObjectSourceBuilder AddParentTypeSignatureAndAttributes()
        {
            return AddParentDebuggerDisplayAttribute()
                .AddParentTypeSignature();
        }
        public ValueObjectSourceBuilder AddParentTypeSignature()
        {
            _builder.Append("partial ")
                .Append(_structOrClass)
                .Append(' ')
                .Append(_typeSymbol.Name)
                .Append(" : IEquatable<")
                .Append(_typeSymbol.Name)
                .Append('>');

            return this;
        }
        public ValueObjectSourceBuilder AddParentDebuggerDisplayAttribute()
        {
            if(!_attribute.GenerateDebugDisplay)
            {
                return this;
            }

            _builder.Append("[System.Diagnostics.DebuggerDisplayAttribute(\"")
                .Append(_typeSymbol.Name);

            var includedFields = _fieldInstructions
                .Where(f => !f.Attribute.ExcludedFromDebugDisplay)
                .ToArray();

            if(includedFields.Length > 0)
            {
                _builder.Append('(')
                    .ForEach(includedFields, ", ", (b, f) =>
                        b.Append(f.Attribute.Name).Append(" : {").Append(f.Attribute.Name).Append('}'))
                    .Append(')');
            }

            _builder.Append("\")]");

            return this;
        }
        #endregion
        #region Constructor & Fields
        public ValueObjectSourceBuilder AddParentConstructorAndFields()
        {
            if(_fieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("#region Constructor & Fields");
            AddParentConstructor();
            AddParentFields();
            AddParentEmptyField();
            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddParentConstructor()
        {
            if(!_attribute.GenerateConstructor)
            {
                return this;
            }

            _builder.AppendLine("/// <summary>")
                .AppendLine("/// Initializes a new instance.")
                .AppendLine("/// </summary>")
                .ForEach(_fieldInstructions, String.Empty, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).AppendLine("\">")
                        .Append("/// The value to assign to <see cref=\"").Append(f.Attribute.Name).AppendLine("\"/>.")
                        .AppendLine("/// </param>"))
                .Append(Util.GetString(_attribute.ConstructorVisibility))
                .Append(' ')
                .Append(_typeSymbol.Name)
                .Append('(')
                .ForEach(_fieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(')')
                .Append('{')
                .ForEach(_fieldInstructions, (b, f) =>
                    b.Append(f.Attribute.Name).Append('=').Append(f.InParamName).Append(';'))
                .Append('}');

            return this;
        }
        public ValueObjectSourceBuilder AddParentFields()
        {
            if(_fieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.ForEach(_fieldInstructions, (b, f) =>
                b.AppendLine("/// <summary>")
                .Append("/// ").AppendLine(f.Attribute.Summary)
                .AppendLine("/// <summary>")
                .Append(Util.GetString(f.Attribute.Visibility))
                .Append(" readonly ")
                .Append(f.Attribute.Type.FullName)
                .Append(' ')
                .Append(f.Attribute.Name)
                .Append(';'));

            return this;
        }
        public ValueObjectSourceBuilder AddParentEmptyField()
        {
            if(_structOrClass != "struct")
            {
                return this;
            }

            _builder.AppendLine("/// <summary>")
                .AppendLine("///Gets the default empty instance.")
                .AppendLine("/// </summary>")
                .Append("public static readonly ")
                .Append(_typeSymbol.Name)
                .Append(" Empty = default;");

            return this;
        }
        #endregion
        #region Validation & Factories
        public ValueObjectSourceBuilder AddParentValidationAndFactories()
        {
            _builder.AppendLine("#region Validation & Factories");
            AddParentValidateMethod();
            AddParentIsValidMethod();
            AddParentTryCreateMethod();
            AddParentCreateMethod();
            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddParentValidateMethod()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("/// <summary>")
                .AppendLine("/// Validates a set of construction parameters.")
                .AppendLine("/// </summary>")
                .AppendLine("/// <param name=\"parameters\">The parameters to validate.</param>")
                .AppendLine("/// <param name=\"result\">The validation result to communicate validation with.</param>")
                .Append("static partial void Validate(ValidateParameters parameters, ref ValidateResult result);");

            return this;
        }
        public ValueObjectSourceBuilder AddParentIsValidMethod()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("/// <summary>")
                .AppendLine("/// Gets a value indicating the validity of the parameters passed, were they to")
                .Append("/// be used for constructing a new instance of <see cref=\"").Append(_typeSymbol.Name).AppendLine("\"/>.")
                .AppendLine("/// </summary>")
                .ForEach(_validatedFieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).AppendLine("\"/>")
                    .Append("/// The potential <see cref=\"").Append(f.Attribute.Name).AppendLine("\"/> value whose validity to assert.")
                    .AppendLine("/// </param>"))
                .AppendLine("/// <returns>")
                .AppendLine("/// A value indicating the validity of the parameters passed.")
                .AppendLine("/// </returns>")
                .Append("public static IsValidResult IsValid(")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(
@")
{
var result = ValidateResult.Valid;
var validateParameters = new ValidateParameters(")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) => b.Append(f.InParamName))
                .Append(
@");
Validate(validateParameters, ref result);
return result;
}");

            return this;
        }
        public ValueObjectSourceBuilder AddParentTryCreateMethod()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("/// <summary>")
                .Append("/// Attempts to create a new instance of <see cref=\"").Append(_typeSymbol.Name).AppendLine("\"/>.")
                .AppendLine("/// </summary>")
                .ForEach(_fieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).AppendLine("\">")
                    .Append("/// The value to assign to the new instances <see cref=\"").Append(f.Attribute.Name).AppendLine("/>.")
                    .AppendLine("/// </param>"))
                .AppendLine("/// <param name=\"result\">")
                .Append("/// Upon returning, will contain a new instance of <see cref=\"").Append(_typeSymbol.Name).AppendLine("/> if")
                .Append("/// one could be constructed using the parameters passed; otherwise, <see langword=\"")
                .Append(_defaultOrNull)
                .AppendLine("\"/>.")
                .AppendLine("/// </param>")
                .AppendLine("/// <returns>")
                .AppendLine("/// A value indicating the validity of the parameters passed.")
                .AppendLine("/// </returns>")
                .Append("public static IsValidResult TryCreate(")
                .ForEach(_fieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(", out ").Append(_typeSymbol.Name).Append(_nullPropagatingToken)
                .Append(
@" result)
{
var validateResult = ValidateResult.Valid;
var validateParameters = new ValidateParameters(")
                .ForEach(_fieldInstructions, ", ", (b, f) =>
                    b.Append(f.InParamName))
                .Append(
@");
Validate(validateParameters, ref validateResult);
var isValid = validateResult.IsValid;
if(isValid)
{
result = new ").Append(_typeSymbol.Name).Append('(')
                .ForEach(_fieldInstructions, ", ", (b, f) =>
                    b.Append(f.InParamName))
                .Append(
@");
} else {
result = ").Append(_defaultOrNull).Append(
@";
}
return validateResult;
}");

            return this;
        }
        public ValueObjectSourceBuilder AddParentCreateMethod()
        {
            _builder.AppendLine("/// <summary>")
                .Append("/// Creates a new instance of <see cref = \"").Append(_typeSymbol.Name).AppendLine("\"/>.")
                .AppendLine("/// </summary>")
                .ForEach(_fieldInstructions, (b, f) =>
                    b.Append("/// <param name = \"").Append(f.InParamName).AppendLine("\">")
                    .Append("/// The value to assign to the new instances <see cref = \"").Append(f.Attribute.Name).AppendLine("\"/>.")
                    .Append("/// </param>"))
                .AppendLine()
                .AppendLine("/// <returns>")
                .Append("/// A new instance of <see cref=\"").Append(_typeSymbol.Name).AppendLine("\"/> if one could be constructed")
                .AppendLine("/// using the parameters passed; otherwise, an <see cref = \"System.ArgumentException\"/> will be thrown.")
                .AppendLine("/// </returns>")
                .AppendLine("/// <exception cref=\"System.ArgumentException\">")
                .AppendLine("/// Thrown if the parameters passed are not valid for construction.")
                .AppendLine("/// </exception>")
                .Append("public static ").Append(_typeSymbol.Name).Append(" Create(")
                .ForEach(_fieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(
@")
{");
            if(_validatedFieldInstructions.Count > 0)
            {
                _builder.Append(
@"var validateResult = ValidateResult.Valid;
var validateParameters = new ValidateParameters(")
                .ForEach(_fieldInstructions, ", ", (b, f) => b.Append(f.InParamName))
                .Append(
@");
Validate(validateParameters, ref validateResult);
if (!validateResult.IsValid)
{
string reasonMessage = null;
string paramName = null;")
                .ForEach(_fieldInstructions, $"{Environment.NewLine} else ", (b, f) =>
                    b.Append("if (validateResult.").Append(f.Attribute.Name).Append("IsInvalid)")
                    .Append('{')
                    .Append("reasonMessage = validateResult.").Append(f.Attribute.Name).Append("Error;")
                    .Append("paramName = \"").Append(f.InParamName).Append("\";")
                    .Append('}'))
                .Append("string reason = null;")
                .Append("if (reasonMessage != null)")
                .Append('{')
                .Append("reason = $\" Reason: {reasonMessage}\";")
                .Append('}')
                .Append("throw new ArgumentException($\"The {paramName} provided for creating an instance of ")
                .Append(_typeSymbol.Name).Append(" was not valid.{reason}\", paramName);")
                .Append('}');
            }

            _builder.Append("return new ").Append(_typeSymbol.Name).Append('(')
            .ForEach(_fieldInstructions, ", ", (b, f) =>
                b.Append(f.InParamName)).Append(");")
            .Append('}');

            return this;
        }
        #endregion
        #region Deconstruction & Transformation
        public ValueObjectSourceBuilder AddParentDeconstructionAndTransformation()
        {
            var anyEligible = _fieldInstructions
                 .Where(f => f.Attribute.IsDeconstructable || f.Attribute.SupportsWith)
                 .Any();

            if(!anyEligible)
            {
                return this;
            }

            _builder.AppendLine("#region Deconstruction & Transformation");
            AddParentDeconstruction();
            AddParentTransformation();
            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddParentDeconstruction()
        {
            var deconstructableFields = _fieldInstructions
                 .Where(f => f.Attribute.IsDeconstructable)
                 .ToArray();

            if(deconstructableFields.Length == 0)
            {
                return this;
            }

            if(deconstructableFields.Length == 1)
            {
                var field = deconstructableFields[0];
                _builder.AppendLine("/// <summary>")
                    .Append("/// Converts an instance of <see cref=\"").Append(_typeSymbol.Name)
                    .Append("\"/> to its single constituent, <see cref=\"")
                    .Append(_typeSymbol.Name).Append('.').Append(field.Attribute.Name).AppendLine("\"/>.")
                    .AppendLine("/// </summary>")
                    .Append("public static implicit operator ").Append(field.Attribute.Type.FullName)
                    .Append('(')
                    .Append(_typeSymbol.Name).Append(" instance) =>")
                    .Append("instance.").Append(field.Attribute.Name).Append(';');
            } else
            {
                _builder.AppendLine("/// <summary>")
                    .AppendLine("/// Deconstructs this instance into its constituent values.")
                    .AppendLine("/// </summary>")
                    .ForEach(deconstructableFields, String.Empty, (b, f) =>
                        b.Append("/// <param name=\"").Append(f.OutParamName).AppendLine("\">")
                        .Append("/// The value contained in <see cref=\"").Append(f.Attribute.Name).AppendLine("\"/>.")
                        .AppendLine("/// </param>"))
                    .Append("public void Deconstruct(")
                    .ForEach(deconstructableFields, ", ", (b, f) =>
                        b.Append("out ").Append(f.Attribute.Type.FullName).Append(' ').Append(f.OutParamName))
                    .Append(')')
                    .Append('{')
                    .ForEach(deconstructableFields, (b, f) =>
                        b.Append(f.OutParamName).Append(" = ").Append(f.Attribute.Name).Append(';'))
                    .Append('}');
            }

            return this;
        }
        public ValueObjectSourceBuilder AddParentTransformation()
        {
            var transformableFields = _fieldInstructions
                .Where(f => f.Attribute.SupportsWith)
                .ToArray();

            if(transformableFields.Length == 0)
            {
                return this;
            }

            _builder.ForEach(transformableFields, (b, f) =>
                b.AppendLine("/// <summary>")
                .Append("/// Constructs a shallow copy of this instance with the <see cref=\"")
                .Append(f.Attribute.Name).AppendLine("\"/> value replaced.")
                .AppendLine("/// </summary>")
                .Append("/// <param name=\"").Append("f.InParamName").AppendLine("\">")
                .Append("/// The value to replace <see cref=\"").Append(f.Attribute.Name).AppendLine("\"/> with.")
                .AppendLine("/// </param>")
                .AppendLine("/// <returns>")
                .Append("/// A shallow copy of this instance with the <see cref=\"")
                .Append(f.Attribute.Name).Append("\"/> value replaced by <paramref name=\"")
                .AppendLine("\"/>.")
                .AppendLine("/// </returns>")
                .Append(Util.GetString(f.Attribute.Visibility)).Append(' ')
                .Append(_typeSymbol.Name).Append(" With").Append(f.Attribute.Name).Append('(')
                .Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName).Append(") =>")
                .Append("Create(")
                .ForEach(_fieldInstructions, ", ", (b_1, f_1) =>
                    b_1.Append(f_1.Attribute.Name == f.Attribute.Name ? f_1.InParamName : f_1.Attribute.Name))
                .Append(");"));

            return this;
        }
        #endregion
        #region Equality & Hashing
        public ValueObjectSourceBuilder AddParentEqualityAndHashing()
        {
            var result = AddParentEqualsMethods()
                .AddParentGetHashCodeMethod()
                .AddParentEqualityOperator()
                .AddParentInequalityOperator();

            return result;
        }
        public ValueObjectSourceBuilder AddParentInequalityOperator()
        {
            _builder.AppendLine("/// <summary>")
                .Append("/// Indicates whether two instances of <see cref=\"")
                .Append(_typeSymbol.Name).AppendLine("\"/> are <em>not</em> equal.")
                .Append(
@"/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// <em>not</em> equal; otherwise, <see langword=""false""/>.
/// </returns>
public static bool operator !=(")
                .Append(_typeSymbol.Name).Append(" left, ")
                .Append(_typeSymbol.Name).AppendLine(" right) => !(left == right);")
                .AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddParentGetHashCodeMethod()
        {
            _builder.AppendLine("/// <inheritdoc/>")
                .Append("public override int GetHashCode() =>");

            if(_fieldInstructions.Count == 0)
            {
                _builder.Append("0;");
                return this;
            }

            if(_fieldInstructions.Count == 1)
            {
                var field = _fieldInstructions[0];
                _builder.Append("System.Collections.Generic.EqualityComparer<")
                    .Append(field.Attribute.Type.FullName)
                    .Append(">.Default.GetHashCode(")
                    .Append(field.Attribute.Name)
                    .Append(");");
            } else
            {
                _builder.Append('(')
                    .ForEach(_fieldInstructions, ", ", (b, f) =>
                        b.Append(f.Attribute.Name))
                    .Append(").GetHashCode();");
            }

            return this;
        }
        public ValueObjectSourceBuilder AddParentEqualsMethods()
        {

            _builder.Append(
@"#region Equality & Hashíng
/// <inheritdoc/>
public override bool Equals(System.Object obj) => obj is ")
                .Append(_typeSymbol.Name).Append(" instance && Equals(instance);")
                .AppendLine("/// <inheritdoc/>")
                .Append("public bool Equals(").Append(_typeSymbol.Name).Append(" other) =>");

            if(_fieldInstructions.Count == 0)
            {
                _builder.Append("true;");
                return this;
            }

            if(_structOrClass == "class")
            {
                _builder.Append("other != null && ");
            }

            if(_fieldInstructions.Count == 1)
            {
                var field = _fieldInstructions[0];
                _builder.Append("System.Collections.Generic.EqualityComparer<")
                    .Append(field.Attribute.Type.FullName)
                    .Append(">.Default.Equals(")
                    .Append(field.Attribute.Name)
                    .Append(", other.").Append(field.Attribute.Name)
                    .Append(");");
            } else
            {
                _builder.Append('(')
                    .ForEach(_fieldInstructions, ", ", (b, f) =>
                        b.Append(f.Attribute.Name))
                    .Append(").Equals((")
                    .ForEach(_fieldInstructions, ", ", (b, f) =>
                        b.Append("other.").Append(f.Attribute.Name))
                    .Append("));");
            }

            return this;
        }
        public ValueObjectSourceBuilder AddParentEqualityOperator()
        {
            _builder.AppendLine("/// <summary>")
                .Append("/// Indicates whether two instances of <see cref=\"")
                .Append(_typeSymbol.Name).AppendLine("\"/> are equal.");

            var isStruct = _structOrClass == "struct";
            if(isStruct)
            {
                _builder.Append(
@"/// <para>
/// <paramref name=""left""/> and <paramref name=""right"" /> are considered equal if 
/// <c><paramref name=""left""/>.Equals(<paramref name=""right"" />)</c> evaluates to <see langword=""true"" />.
/// </para>");
            } else
            {
                _builder.Append(
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
/// </para>");
            }

            _builder.Append(
@"/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// equal; otherwise, <see langword=""false""/>.
/// </returns>
public static bool operator ==(")
                .Append(_typeSymbol.Name).Append(" left, ")
                .Append(_typeSymbol.Name).Append(" right) =>");

            if(isStruct)
            {
                _builder.Append("left.Equals(right);");
            } else
            {
                _builder.Append("left == null ? right == null : right == null ? left == null : left.Equals(right);");
            }

            return this;
        }
        #endregion
        #endregion
        #region ValidateParameters Type
        public ValueObjectSourceBuilder AddValidateParametersType()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine(
@"/// <summary>
/// Wrapper type around possible construction parameters; used for validation.
/// </summary>
private readonly struct ValidateParameters : IEquatable<ValidateParameters>
{");

            AddValidateParametersConstructorsAndFields();
            AddValidateParametersDeconstruction();
            AddValidateParametersEqualityAndHashing();

            _builder.AppendLine("}");

            return this;
        }
        public ValueObjectSourceBuilder AddValidateParametersConstructorsAndFields()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine(
@"#region Constructors & Fields
/// <summary>
/// Initializes a new instance.
/// </summary>")
                .ForEach(_validatedFieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).AppendLine("\">")
                    .Append("/// The value for <see cref=\"").Append(f.Attribute.Name).AppendLine("\"/> to validate.")
                    .AppendLine("/// </param>"))
                .Append("public ValidateParameters(")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Type.FullName).Append(' ').Append(f.InParamName))
                .Append(')')
                .Append('{')
                .ForEach(_validatedFieldInstructions, (b, f) =>
                    b.Append(f.Attribute.Name).Append(" = ").Append(f.InParamName).Append(';'))
                .Append('}')
                .ForEach(_validatedFieldInstructions, (b, f) =>
                    b.AppendLine("/// <summary>")
                    .Append("/// The value for <see cref=\"").Append(f.Attribute.Name).AppendLine("\"/> to validate.")
                    .AppendLine("/// </summary>")
                    .Append("public readonly ").Append(f.Attribute.Type.FullName).Append(' ')
                    .Append(f.Attribute.Name).Append(';'))
                .AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddValidateParametersDeconstruction()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("#region Deconstruction");

            if(_validatedFieldInstructions.Count == 1)
            {
                var field = _validatedFieldInstructions[0];
                _builder.AppendLine("/// <summary>")
                    .Append("/// Converts an instance of <see cref=\"\"ValidateParameters\"\"/> to its single constituent, <see cref=\"ValidateParameters.")
                    .Append(field.Attribute.Name).AppendLine("\"/>.")
                    .AppendLine("/// </summary>")
                    .Append("public static implicit operator ")
                    .Append(field.Attribute.Type.FullName).Append("(ValidateParameters instance) =>")
                    .Append("instance.").Append(field.Attribute.Name).AppendLine(";");
            } else
            {
                _builder.AppendLine(
@"/// <summary>
/// Deconstructs this instance into its constituent values.
/// </summary>")
                    .ForEach(_validatedFieldInstructions, (b, f) =>
                        b.Append("/// <param name=\"").Append(f.OutParamName)
                        .AppendLine("\">")
                        .Append("/// The value contained in <see cref=\"")
                        .Append(f.Attribute.Name).AppendLine("\"/>.")
                        .AppendLine("/// </param>"))
                    .Append("public ValueObjectSourceBuilder Deconstruct(")
                    .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                        b.Append("out ").Append(f.Attribute.Type.FullName).Append(' ').Append(f.OutParamName))
                    .Append(')')
                    .Append('{')
                    .ForEach(_validatedFieldInstructions, (b, f) =>
                        b.Append(f.OutParamName).Append(" = ").Append(f.Attribute.Name).Append(';'))
                    .AppendLine("}");
            }

            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddValidateParametersEqualityAndHashing()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"#region Equality & Hashing
/// <inheritdoc/>
public override bool Equals(System.Object obj)
{
	return obj is ValidateParameters address && Equals(address);
}
/// <inheritdoc/>
public bool Equals(ValidateParameters other)
{")
                .Append("return (")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name))
                .Append(").Equals((")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append("other.").Append(f.Attribute.Name))
                .Append("));")
                .Append('}')
                .Append(
@"/// <inheritdoc/>
public override int GetHashCode()
{")
                .Append("return (")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name))
                .Append(").GetHashCode();")
                .Append('}')
                .AppendLine(
@"/// <summary>
/// Indicates whether two instances of <see cref=""ValidateParameters""/> are equal.
/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// equal; otherwise, <see langword=""false""/>.
/// </returns>
public static bool operator ==(ValidateParameters left, ValidateParameters right)
{
	return left.Equals(right);
}
/// <summary>
/// Indicates whether two instances of <see cref=""ValidateParameters""/> are <em>not</em> equal.
/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// <em>not</em> equal; otherwise, <see langword=""false""/>.
/// </returns>
public static bool operator !=(ValidateParameters left, ValidateParameters right)
{
	return !(left == right);
}
#endregion");

            return this;
        }
        #endregion
        #region ValidateResult Type
        public ValueObjectSourceBuilder AddValidateResultType()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine(
@"/// <summary>
/// Communicates detailed validation results.
/// </summary>
private ref struct ValidateResult
{");
            AddValidateResultFieldsAndProperties();
            AddValidateResultConversionOperators();
            AddValidateResultEqualityAndHashing();

            _builder.AppendLine("}");

            return this;
        }
        public ValueObjectSourceBuilder AddValidateResultFieldsAndProperties()
        {
            _builder.AppendLine("#region Fields & Properties")
                .ForEach(_validatedFieldInstructions, (b, f) =>
                    b.AppendLine("/// <summary>")
                    .Append("/// Indicates whether the value provided by <see cref=\"ValidateParameters.")
                    .Append(f.Attribute.Name).AppendLine("\"/> is invalid.")
                    .AppendLine("/// </summary>")
                    .Append("public bool ").Append(f.Attribute.Name).AppendLine("IsInvalid;")
                    .Append(
@"/// <summary>
/// Contains the error message to include in instances of <see cref=""System.ArgumentException""/> thrown 
/// by <see cref=""Create(")
                    .ForEach(_validatedFieldInstructions, ", ", (b_1, f_1) =>
                        b_1.Append(f_1.Attribute.Type.FullName))
                    .Append(")\"/> if <see cref=\"")
                    .Append(f.Attribute.Name)
                    .AppendLine("IsInvalid\"/> is set to <see langword=\"true\"/>.")
                    .AppendLine("/// </summary>")
                    .Append("public string ").Append(f.Attribute.Name).AppendLine("Error;"))
                .Append(
@"/// <summary>
/// Gets a default (valid) instance.
/// </summary>
public static ValidateResult Valid => default;

/// <summary>
/// Gets a value indicating whether none of the validation fields have been set to <see langword=""true""/>.
/// </summary>
public bool IsValid =>")
                .ForEach(_validatedFieldInstructions, "&& ", (b, f) =>
                    b.Append('!').Append(f.Attribute.Name).Append("IsInvalid"))
                .AppendLine(";")
                .AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddValidateResultConversionOperators()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"#region Conversion Operators
/// <summary>
/// Implicitly converts a mutable instance of <see cref=""ValidateResult""/> to 
/// an immutable instance of <see cref=""IsValidResult""/>.
/// </summary>
/// <param name=""result"">The instance to implicitly convert.</param>
public static implicit operator IsValidResult(ValidateResult result) =>
	new IsValidResult(")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append("result.").Append(f.Attribute.Name).Append("IsInvalid")
                    .Append(", result.").Append(f.Attribute.Name).Append("Error"))
                .Append(
@");
#endregion
");

            return this;
        }
        public ValueObjectSourceBuilder AddValidateResultEqualityAndHashing()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"#region Equals & Hashing
/// <summary>
/// <see cref=""ValidateResult""/> may not be boxed due to being a <see langword=""ref""/> <see langword=""struct""/>. 
/// Therefore, calling <see cref=""Equals(System.Object)""/> is not supported.
/// </summary>
/// <exception cref=""NotSupportedException""></exception>
public override bool Equals(System.Object obj)
{
	throw new NotSupportedException(""").Append(_typeSymbol.Name).Append(@".ValidateResult may not be boxed due to being a ref struct. Therefore, calling Equals(System.Object) is not supported."");
}
/// <summary>
/// <see cref=""ValidateResult""/> does not support calling <see cref=""GetHashCode""/>.
/// </summary>
/// <exception cref=""NotSupportedException""></exception>
public override int GetHashCode()
{
	throw new NotSupportedException();
}
#endregion
");

            return this;
        }
        #endregion
        #region IsValidResult Type
        public ValueObjectSourceBuilder AddIsValidResultType()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"/// <summary>
/// Contains validation results for construction parameters.
/// </summary>
public readonly struct IsValidResult : IEquatable<IsValidResult>
{");
            AddIsValidResultConstructorAndFields();
            AddIsValidResultConversionAndDeconstruction();
            AddIsValidResultEqualityAndHashing();
            _builder.Append('}');

            return this;
        }
        #region Constructor & Fields
        public ValueObjectSourceBuilder AddIsValidResultConstructorAndFields()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("#region Constructor & Fields");
            AddIsValidResultConstructor();
            AddIsValidResultFields();
            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddIsValidResultConstructor()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"/// <summary>
/// Initializes a new instance.
/// </summary>
")
                .ForEach(_validatedFieldInstructions, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.InParamName).AppendLine("IsInvalid\">")
                    .Append("/// Indicates whether the parameter provided for <see cref=\"")
                    .Append(f.Attribute.Name).AppendLine("\"/> was invalid.")
                    .AppendLine("/// </param>"))
                .Append("public IsValidResult(")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append("bool ").Append(f.InParamName)
                    .Append("IsInvalid, string ")
                    .Append(f.InParamName).Append("Error"))
                .Append("){")
                .ForEach(_validatedFieldInstructions, (b, f) =>
                    b.Append(f.Attribute.Name).Append("IsInvalid = ").Append(f.InParamName).Append("IsInvalid;")
                    .Append(f.Attribute.Name).Append("Error = ").Append(f.InParamName).Append("Error;"))
                .Append('}');

            return this;
        }
        public ValueObjectSourceBuilder AddIsValidResultFields()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.ForEach(_validatedFieldInstructions, (b, f) =>
                b.Append(
@"/// <summary>
/// Indicates whether the parameter provided for <see cref=""")
                .Append(f.Attribute.Name).Append(
@"""/> was invalid.
/// </summary>
public readonly bool ")
                .Append(f.Attribute.Name).AppendLine("IsInvalid;")
                .Append(
@"/// <summary>
/// Contains a validation description if <see cref=""").Append(f.Attribute.Name).Append(
@"IsInvalid""/> is set to <see langword=""true""/>.
/// </summary>
public readonly string ").Append(f.Attribute.Name).AppendLine("Error;"));

            return this;
        }
        #endregion
        #region Conversion & Deconstruction
        public ValueObjectSourceBuilder AddIsValidResultConversionAndDeconstruction()
        {
            var eligibleFields = _fieldInstructions
                .Where(f => f.Attribute.IsDeconstructable || f.Attribute.IsValidated)
                .Any();

            if(!eligibleFields)
            {
                return this;
            }

            _builder.AppendLine("#region Conversion & Deconstruction");
            AddIsValidResultConversion();
            AddIsValidResultDeconstruction();
            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddIsValidResultConversion()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"/// <summary>
/// Implicitly converts an instance of <see cref=""""IsValidResult""""/> to <see cref=""""bool""""/>.
/// ");

            if(_validatedFieldInstructions.Count == 1)
            {
                _builder.Append("As no fields are being validated, the result will always be <see langword=\"true\"/>");
            } else
            {
                _builder.Append("The result will be true if all validity fields evaluate to <see langword=\"false\"/>.");
            }

            _builder.Append(@"
/// </summary>
/// <param name=""result"" > The instance to implicitly convert.</ param >
public static implicit operator bool(IsValidResult result) =>
");

            if(_validatedFieldInstructions.Count == 1)
            {
                _builder.Append("true");
            } else
            {
                _builder.ForEach(_validatedFieldInstructions, " &&", (b, f) =>
                    b.Append("!result.").Append(f.Attribute.Name).Append("IsInvalid"));
            }

            _builder.Append(';');

            return this;
        }
        public ValueObjectSourceBuilder AddIsValidResultDeconstruction()
        {
            var deconstructableFields = _fieldInstructions
                .Where(f => f.Attribute.IsDeconstructable)
                .ToArray();

            if(deconstructableFields.Length == 0)
            {
                return this;
            }

            _builder.Append(
@"/// <summary>
/// Deconstructs this instance into its constituent values.
/// </summary>")
                .ForEach(deconstructableFields, (b, f) =>
                    b.Append("/// <param name=\"").Append(f.OutParamName).Append("IsInvalid\">")
                    .Append("/// The value contained in <see cref=\"").Append(f.Attribute.Name).Append("IsInvalid\"/>.")
                    .Append("/// </param>")
                    .Append("/// <param name=\"").Append(f.OutParamName).Append("Error\">")
                    .Append("/// The value contained in <see cref=\"").Append(f.Attribute.Name).Append("Error\"/>.")
                    .Append("/// </param>"))
                .Append("public ValueObjectSourceBuilder Deconstruct(")
                .ForEach(deconstructableFields, (b, f) =>
                    b.Append("bool ").Append(f.OutParamName).Append("IsInvalid, string ").Append(f.OutParamName).Append("Error"))
                .Append(')')
                .Append('{')
                .ForEach(deconstructableFields, (b, f) =>
                    b.Append(f.OutParamName).Append("IsInvalid = ")
                    .Append(f.Attribute.Name).Append("IsInvalid;"))
                .Append('}');

            return this;
        }
        #endregion
        #region Equality & Hashing
        public ValueObjectSourceBuilder AddIsValidResultEqualityAndHashing()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.AppendLine("#region Equality & Hashing");
            AddIsValidResultEquals();
            AddIsValidResultGetHashcode();
            _builder.AppendLine("#endregion");

            return this;
        }
        public ValueObjectSourceBuilder AddIsValidResultEqualityOperators()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"/// <summary>
/// Indicates whether two instances of <see cref=""IsValidResult""/> are equal.
/// </summary>
/// <param name=""left"">The left operand.</param>
/// <param name=""left"">The right operand.</param>
/// <returns>
/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
/// equal; otherwise, <see langword=""false""/>.
/// </returns>
public static bool operator ==(IsValidResult left, IsValidResult right)
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
public static bool operator !=(IsValidResult left, IsValidResult right)
{{
	return !(left == right);
}}");

            return this;
        }
        public ValueObjectSourceBuilder AddIsValidResultEquals()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"/// <inheritdoc/>
public override bool Equals(System.Object obj)
{
	return obj is IsValidResult result && Equals(result);
}
/// <inheritdoc/>
public bool Equals(IsValidResult other) =>
(")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name).Append("IsInvalid, ")
                    .Append(f.Attribute.Name).Append("Error"))
                .Append(").Equals((")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append("other.").Append(f.Attribute.Name).Append("IsInvalid, ")
                    .Append("other.").Append(f.Attribute.Name).Append("Error"))
                .Append("));");

            return this;
        }
        public ValueObjectSourceBuilder AddIsValidResultGetHashcode()
        {
            if(_validatedFieldInstructions.Count == 0)
            {
                return this;
            }

            _builder.Append(
@"/// <inheritdoc/>
public override int GetHashCode() =>
(")
                .ForEach(_validatedFieldInstructions, ", ", (b, f) =>
                    b.Append(f.Attribute.Name).Append("IsInvalid, ")
                    .Append(f.Attribute.Name).Append("Error"))
                .Append(").GetHashCode();");

            return this;
        }
        #endregion
        #endregion
    }
}