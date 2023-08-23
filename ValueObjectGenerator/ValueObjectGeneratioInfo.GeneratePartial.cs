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

namespace RhoMicro.ValueObjectGenerator
{
    internal readonly partial struct ValueObjectGenerationInfo
    {
        internal GeneratedType GeneratePartial()
        {
            var instructions = ObjectInstructions.Create(_typeDeclaration, _semanticModel, _generateAttribute, _fieldAttributes);

            var source = BuildPartialType(instructions);

            var declarationSymbol = (ITypeSymbol)_semanticModel.GetDeclaredSymbol(_typeDeclaration);
            var identifier = TypeIdentifier.Create(declarationSymbol);
            var result = new GeneratedType(identifier, source);

            return result;
        }
        /// <summary>
        /// abbr.: Project Field Instructions
        /// </summary>
        private static String PFI(ObjectInstructions instructions, String separator, Func<FieldInstructions, String> projection)
        {
            var projected = instructions.FieldInstructions.Select(projection);
            var result = String.Join(separator, projected);

            return result;
        }
        /// <summary>
        /// abbr.: Project Field Instructions
        /// </summary>
        private static String PFI(ObjectInstructions instructions, String separator, Func<FieldInstructions, Int32, String> projection)
        {
            var projected = instructions.FieldInstructions.Select(projection);
            var result = String.Join(separator, projected);

            return result;
        }
        private static String BuildPartialType(ObjectInstructions instr)
        {
            var nsOpen = instr.HasNamespace ?
$@"namespace {instr.Namespace}
{{
" :
                String.Empty;
            var nsClose = instr.HasNamespace ?
                "\r\n}" :
                String.Empty;
            var emptyInstance = instr.StructOrClass == "struct" ?
$@"/// <summary>
///	Gets the default empty instance.
/// </summary>
public static readonly {instr.Name} Empty = default;" :
                String.Empty;
            var ctor = instr.Attribute.GenerateConstructor ?
$@"/// <summary>
/// Initializes a new instance.
/// </summary>
{PFI(instr, "\r\n", f =>
$@"/// <param name=""{f.InParamName}"">
/// The value to assign to <see cref=""{f.Attribute.Name}""/>.
/// </param>")}
private {instr.Name}({PFI(instr, ", ", f => $"{f.Attribute.Type.FullName} {f.InParamName}")})
{{
	{PFI(instr, "\r\n", f => $"{f.Attribute.Name} = {f.InParamName};")}
}}" :
                String.Empty;
            var debuggerDisplayAttr = instr.Attribute.GenerateDebugDisplay ?
                "\r\n[System.Diagnostics.DebuggerDisplay(\"{GetDebugDisplayString()}\")]\r\n" :
                String.Empty;
            var debuggerDisplayImpl = GetDebuggerDisplayImpl(instr);

            var result =
$@"{nsOpen}{debuggerDisplayAttr}{instr.Visibility} partial {instr.StructOrClass} {instr.Name} : IEquatable<{instr.Name}>
	{{
		#region Nested Types
{BuildValidateResultType(instr)}
{BuildValidateParametersType(instr)}
{BuildIsValidResultType(instr)}
		#endregion
		#region Constructor & Fields
{ctor}
{PFI(instr, "\r\n", f =>
$@"		/// <summary>
		/// {f.Attribute.Summary}
		/// </summary>
		{Util.GetString(f.Attribute.Visibility)} readonly {f.Attribute.Type.FullName} {f.Attribute.Name};")}{emptyInstance}		
		#endregion
{debuggerDisplayImpl}
		#region Validation & Factories
		/// <summary>
		/// Validates a set of construction parameters.
		/// </summary>
		/// <param name=""parameters"">The parameters to validate.</param>
		/// <param name=""result"">The validation result to communicate validation with.</param>
		static partial void Validate(ValidateParameters parameters, ref ValidateResult result);
		/// <summary>
		/// Gets a value indicating the validity of the parameters passed, were they to be used for constructing a new instance of
		/// <see cref=""{instr.Name}""/>.
		/// </summary>
{PFI(instr, "\r\n", f =>
$@"		/// <param name=""{f.InParamName}"">
		/// The potential <see cref=""{f.Attribute.Name}""/> value whose validity to assert.
		/// </param>")}
		/// <returns>
		/// A value indicating the validity of the parameters passed.
		/// </returns>
		public static IsValidResult IsValid({PFI(instr, ", ", f => $"{f.Attribute.Type.FullName} {f.InParamName}")})
		{{
			var result = ValidateResult.Valid;
			var validateParameters = new ValidateParameters({PFI(instr, ", ", f => f.InParamName)});
			Validate(validateParameters, ref result);

			return result;
		}}
		/// <summary>
		/// Attempts to create a new instance of <see cref=""{instr.Name}""/>.
		/// </summary>
{PFI(instr, "\r\n", f =>
$@"		/// <param name=""{f.InParamName}"">
		/// The value to assign to the new instances <see cref=""{f.Attribute.Name}""/>.
		/// </param>")}
		/// <param name=""result"">
		/// Upon returning, will contain a new instance of <see cref=""{instr.Name}""/> if
		/// one could be constructed using the parameters passed; otherwise, <see langword=""{instr.DefaultOrNull}""/>.
		/// </param>
		/// <returns>
		/// A value indicating the validity of the parameters passed.
		/// </returns>
		public static IsValidResult TryCreate({PFI(instr, ", ", f => $"{f.Attribute.Type.FullName} {f.InParamName}")}, out {instr.Name}{instr.NullPropagatingToken} result)
		{{
			var validateResult = ValidateResult.Valid;
			var validateParameters = new ValidateParameters({PFI(instr, ", ", f => f.InParamName)});
			Validate(validateParameters, ref validateResult);
			var isValid = validateResult.IsValid;

			if (isValid)
			{{
				result = new {instr.Name}({PFI(instr, ", ", f => f.InParamName)});
			}}
			else
			{{
				result = {instr.DefaultOrNull};
			}}

			return validateResult;
		}}
		/// <summary>
		/// Creates a new instance of <see cref=""{instr.Name}""/>.
		/// </summary>
{PFI(instr, "\r\n", f =>
$@"		/// <param name=""{f.InParamName}"">
		/// The value to assign to the new instances <see cref=""{f.Attribute.Name}""/>.
		/// </param>")}
		/// <returns>
		/// A new instance of <see cref=""{instr.Name}""/> if one could be constructed
		/// using the parameters passed; otherwise, an <see cref=""System.ArgumentException""/> will be thrown.
		/// </returns>
		/// <exception cref=""System.ArgumentException"">
		/// Thrown if the parameters passed are not valid construction values.
		/// </exception>
		public static {instr.Name} Create({PFI(instr, ", ", f => $"{f.Attribute.Type.FullName} {f.InParamName}")})
		{{
			var validateResult = ValidateResult.Valid;
			var validateParameters = new ValidateParameters({PFI(instr, ", ", f => f.InParamName)});
			Validate(validateParameters, ref validateResult);

			if (!validateResult.IsValid)
			{{
				String reasonMessage = null;
				String paramName = null;
				{PFI(instr,
"\r\n				else ", f =>
$@"if (validateResult.{f.Attribute.Name}IsInvalid)
				{{
					reasonMessage = validateResult.{f.Attribute.Name}Error;
					paramName = ""{f.InParamName}"";
				}}")}

				String reason = null;
				if (reasonMessage != null)
				{{
					reason = $"" Reason: {{reasonMessage}}"";
				}}
				throw new ArgumentException($""The {{paramName}} provided for creating an instance of {instr.Name} was not valid.{{reason}}"", paramName);
			}}

			return new {instr.Name}({PFI(instr, ", ", f => f.InParamName)});
		}}
		#endregion
		#region Deconstruction & Transformation
		/// <summary>
		/// Deconstructs this instance into its constituent values.
		/// </summary>
{PFI(instr, "\r\n", f =>
$@"			/// <param name=""{f.OutParamName}"">
			/// The value contained in <see cref=""{f.Attribute.Name}""/>.
			/// </param>")}
		public void Deconstruct({PFI(instr, ", ", f => $"out {f.Attribute.Type.FullName} {f.OutParamName}")})
		{{
{PFI(instr, "\r\n", f =>
$@"			{f.OutParamName} = {f.Attribute.Name};")}
		}}
{PFI(instr, "\r\n", (f, i) =>
$@"		/// <summary>
		/// Constructs a shallow copy of this instance with the <see cref=""{f.Attribute.Name}""/> value replaced.
		/// </summary>
		/// <param name=""{f.InParamName}"">
		/// The value to replace <see cref=""{f.Attribute.Name}""/> with.
		/// </param>
		/// <returns>
		/// A shallow copy of this instance with the <see cref=""{f.Attribute.Name}""/> value replaced by <paramref name=""{f.InParamName}""/>.
		/// </returns>
		{Util.GetString(f.Attribute.Visibility)} {instr.Name} With{f.Attribute.Name}({f.Attribute.Type.FullName} {f.InParamName})
		{{
			var result = Create({PFI(instr, ", ", (g, j) => j == i ? g.InParamName : g.Attribute.Name)});

			return result;
		}}")}
		#endregion
		#region Equality & Hashíng
		/// <inheritdoc/>
		public override System.Boolean Equals(System.Object obj)
		{{
			return obj is {instr.Name} instance && Equals(instance);
		}}
		/// <inheritdoc/>
		public System.Boolean Equals({instr.Name} other)
		{{
			return ({PFI(instr, ", ", f => f.Attribute.Name)}).Equals(
				({PFI(instr, ", ", f => $"other.{f.Attribute.Name}")}));
		}}
		/// <inheritdoc/>
		public override System.Int32 GetHashCode()
		{{
			return ({PFI(instr, ", ", f => f.Attribute.Name)}).GetHashCode();
		}}
		/// <summary>
		/// Indicates whether two instances of <see cref=""{instr.Name}""/> are equal.
		/// </summary>
		/// <param name=""left"">The left operand.</param>
		/// <param name=""left"">The right operand.</param>
		/// <returns>
		/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
		/// equal; otherwise, <see langword=""false""/>.
		/// </returns>
		public static System.Boolean operator ==({instr.Name} left, {instr.Name} right)
		{{
			return left.Equals(right);
		}}
		/// <summary>
		/// Indicates whether two instances of <see cref=""{instr.Name}""/> are <em>not</em> equal.
		/// </summary>
		/// <param name=""left"">The left operand.</param>
		/// <param name=""left"">The right operand.</param>
		/// <returns>
		/// <see langword=""true""/> if <paramref name=""left""/> and <paramref name=""right""/> are
		/// <em>not</em> equal; otherwise, <see langword=""false""/>.
		/// </returns>
		public static System.Boolean operator !=({instr.Name} left, {instr.Name} right)
		{{
			return !(left == right);
		}}
		#endregion
	}}{nsClose}";

            return result;
        }

        private static String GetDebuggerDisplayImpl(ObjectInstructions instr)
        {
            if(!instr.Attribute.GenerateDebugDisplay)
            {
                return String.Empty;
            }

            return $@"#region Debugger Display
private System.String GetDebugDisplayString() =>
	$""{instr.Name} {{{{ {PFI(instr, ", ", f => $"{f.Attribute.Name}:{quotes(f)}{{{f.Attribute.Name}}}{quotes(f)}")} }}}}"";
#endregion";

            String quotes(FieldInstructions f) =>
                f.Attribute.Type == typeof(String) ?
                "\\\"" :
                String.Empty;
        }

        private static String BuildValidateParametersType(ObjectInstructions instr)
        {
            var result =
$@"		/// <summary>
		/// Wrapper type around possible construction parameters.
		/// </summary>
		private readonly struct ValidateParameters : IEquatable<ValidateParameters>
		{{
			#region Constructors & Fields
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
{PFI(instr, "\r\n", f =>
$@"			/// <param name=""{f.InParamName}"">
			/// The value for <see cref=""{f.Attribute.Name}""/> to validate.
			/// </param>")}
			public ValidateParameters({PFI(instr, ", ", f => $"{f.Attribute.Type.FullName} {f.InParamName}")})
			{{
{PFI(instr, "\r\n", f =>
$"				{f.Attribute.Name} = {f.InParamName};")}
			}}
{PFI(instr, "\r\n", f =>
$@"			/// <summary>
			/// The value for <see cref=""{f.Attribute.Name}""/> to validate.
			/// </summary>
			public readonly {f.Attribute.Type.FullName} {f.Attribute.Name};")}
			#endregion
			#region Deconstruction
			/// <summary>
			/// Deconstructs this instance into its constituent values.
			/// </summary>
{PFI(instr, "\r\n", f =>
$@"			/// <param name=""{f.OutParamName}"">
			/// The value contained in <see cref=""{f.Attribute.Name}""/>.
			/// </param>")}
			public void Deconstruct(
{PFI(instr, ",\r\n", f =>
$"				out {f.Attribute.Type.FullName} {f.OutParamName}")})
			{{
{PFI(instr, "\r\n", f =>
$"				{f.OutParamName} = {f.Attribute.Name};")}
			}}
			#endregion
			#region Equality & Hashing
			/// <inheritdoc/>
			public override System.Boolean Equals(System.Object obj)
			{{
				return obj is ValidateParameters address && Equals(address);
			}}
			/// <inheritdoc/>
			public System.Boolean Equals(ValidateParameters other)
			{{
				return ({PFI(instr, ", ", f => f.Attribute.Name)}).Equals(({PFI(instr, ", ", f => $"other.{f.Attribute.Name}")}));
			}}
			/// <inheritdoc/>
			public override System.Int32 GetHashCode()
			{{
				return ({PFI(instr, ", ", f => f.Attribute.Name)}).GetHashCode();
			}}
			/// <summary>
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
			#endregion
		}}";

            return result;
        }
        private static String BuildValidateResultType(ObjectInstructions instr)
        {
            var result =
$@"		/// <summary>
		/// Communicates detailed validation results.
		/// </summary>
		private ref struct ValidateResult
		{{
			#region Fields & Properties
{PFI(instr, "\r\n", f =>
$@"			/// <summary>
			/// Indicates whether the value provided by <see cref=""ValidateParameters.{f.Attribute.Name}""/> is invalid.
			/// </summary>
			public System.Boolean {f.Attribute.Name}IsInvalid;
			/// <summary>
			/// Contains the error message to include in instances of <see cref=""System.ArgumentException""/> thrown 
			/// by <see cref=""Create(System.String, System.String)""/> if <see cref=""{f.Attribute.Name}IsInvalid""/> is set to <see langword=""true""/>.
			/// </summary>
			public System.String {f.Attribute.Name}Error;")}

			/// <summary>
			/// Gets a default (valid) instance.
			/// </summary>
			public static ValidateResult Valid => default;

			/// <summary>
			/// Gets a value indicating whether none of the <c>xxxIsInvalid</c> fields have been set to <see langword=""true""/>.
			/// </summary>
			public Boolean IsValid =>
{PFI(instr, "&&\r\n", f => $"!{f.Attribute.Name}IsInvalid")};
			#endregion
			#region Conversion Operators
			public static implicit operator IsValidResult(ValidateResult result)=>
				new IsValidResult({PFI(instr, ", ", f => $"result.{f.Attribute.Name}IsInvalid, result.{f.Attribute.Name}Error")});
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

            return result;
        }
        private static String BuildIsValidResultType(ObjectInstructions instr)
        {
            var result =
$@"		/// <summary>
		/// Contains validation results for construction parameters.
		/// </summary>
		public readonly struct IsValidResult : IEquatable<IsValidResult>
		{{
			#region Constructor & Fields
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
{PFI(instr, "\r\n", f =>
$@"			/// <param name=""{f.InParamName}IsInvalid"">
			/// Indicates whether the parameter provided for <see cref=""{f.Attribute.Name}""/> was invalid.
			/// </param>")}
			public IsValidResult({PFI(instr, ", ", f => $"System.Boolean {f.InParamName}IsInvalid, System.String {f.InParamName}Error")})
			{{
{PFI(instr, "\r\n\r\n", f =>
$@"				{f.Attribute.Name}IsInvalid = {f.InParamName}IsInvalid;
				{f.Attribute.Name}Error = {f.InParamName}Error;")}
			}}

{PFI(instr, "\r\n", f =>
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
			/// <summary>
			/// Implicitly converts an instance of <see cref=""IsValidResult""/> to <see cref=""System.Boolean""/>.
			/// The result will be true if all validity fields evaluate to <see langword=""false""/>.
			/// </summary>
			/// <param name=""result"">The instance to implicitly convert.</param>
			public static implicit operator Boolean(IsValidResult result)=>
{PFI(instr, " &&\r\n", f =>
$@"				!result.{f.Attribute.Name}IsInvalid")};

			/// <summary>
			/// Deconstructs this instance into its constituent values.
			/// </summary>
{PFI(instr, "\r\n", f =>
$@"			/// <param name=""{f.OutParamName}IsInvalid"">
			/// The value contained in <see cref=""{f.Attribute.Name}IsInvalid""/>.
			/// </param>")}
			public void Deconstruct({PFI(instr, ", ", f => $"System.Boolean {f.OutParamName}IsInvalid")})
			{{
{PFI(instr, "\r\n", f =>
$@"				{f.OutParamName}IsInvalid = {f.Attribute.Name}IsInvalid;")}
			}}
			#endregion
			#region Equality & Hashing
			/// <inheritdoc/>
			public override System.Boolean Equals(System.Object obj)
			{{
				return obj is IsValidResult result && Equals(result);
			}}
			/// <inheritdoc/>
			public System.Boolean Equals(IsValidResult other)
			{{
				return ({PFI(instr, ", ", f => $"{f.Attribute.Name}IsInvalid")}).Equals(
					({PFI(instr, ", ", f => $"other.{f.Attribute.Name}IsInvalid")}));
			}}
			/// <inheritdoc/>
			public override System.Int32 GetHashCode()
			{{
				return ({PFI(instr, ", ", f => $"{f.Attribute.Name}IsInvalid")}).GetHashCode();
			}}
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
    }
}
