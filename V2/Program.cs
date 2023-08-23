using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ValueObjects
{
	public abstract class ValueObjectBase
	{
		public ValueObjectBase(Object p) { }
	}
	[GeneratedValueObject(GenerateConstructor = false)]
	[GenerateValueObjectProperty(typeof(String), "City")]
	[GenerateValueObjectProperty(typeof(String), "Street")]
	public partial class Address : ValueObjectBase
	{
		public Address(string city, string street, Object p) :
			base(p)
		{ }
		private Address(string city, string street) :
			this(city, street, null)
		{
			City = city;
			Street = street;
		}

		static partial void Validate(ValidateParameters parameters, ref ValidateResult result)
		{
			var (city, street) = parameters;
			if (String.IsNullOrEmpty(city))
			{
				result.CityIsInvalid = true;
				result.CityError = "A city may not be null or empty.";
			}
			if (String.IsNullOrEmpty(street))
			{
				result.StreetIsInvalid = true;
				result.StreetError = "A street must be provided (may not be null or empty).";
			}
			if (street == "Baker")
			{
				result.StreetIsInvalid = true;
				result.StreetError = "Baker Street is not real.";
			}
		}
	}
	public partial class Address : IEquatable<Address>
	{
		#region Nested Types
		/// <summary>
		/// Wrapper type around possible construction parameters.
		/// </summary>
		private readonly struct ValidateParameters : IEquatable<ValidateParameters>
		{
			#region Constructors & Fields
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			/// <param name="city">
			/// The value for <see cref="City"/> to validate.
			/// </param>
			/// <param name="street">
			/// The value for <see cref="Street"/> to validate.
			/// </param>
			public ValidateParameters(string city, string street)
			{
				City = city;
				Street = street;
			}
			/// <summary>
			/// The value for <see cref="City"/> to validate.
			/// </summary>
			public readonly String City;
			/// <summary>
			/// The value for <see cref="Street"/> to validate.
			/// </summary>
			public readonly String Street;
			#endregion
			#region Deconstruction
			/// <summary>
			/// Deconstructs this instance into its constituent values.
			/// </summary>
			/// <param name="city">
			/// The value contained in <see cref="City"/>.
			/// </param>
			/// <param name="street">
			/// The value contained in <see cref="Street"/>.
			/// </param>
			public void Deconstruct(out String city, out String street)
			{
				city = City;
				street = Street;
			}
			#endregion
			#region Equality & Hashing
			/// <inheritdoc/>
			public override bool Equals(object obj)
			{
				return obj is ValidateParameters address && Equals(address);
			}
			/// <inheritdoc/>
			public bool Equals(ValidateParameters other)
			{
				return (City, Street).Equals((other.City, other.Street));
			}
			/// <inheritdoc/>
			public override int GetHashCode()
			{
				return (City, Street).GetHashCode();
			}
			/// <summary>
			/// Indicates whether two instances of <see cref="ValidateParameters"/> are equal.
			/// </summary>
			/// <param name="left">The left operand.</param>
			/// <param name="left">The right operand.</param>
			/// <returns>
			/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are
			/// equal; otherwise, <see langword="false"/>.
			/// </returns>
			public static bool operator ==(ValidateParameters left, ValidateParameters right)
			{
				return left.Equals(right);
			}
			/// <summary>
			/// Indicates whether two instances of <see cref="ValidateParameters"/> are <em>not</em> equal.
			/// </summary>
			/// <param name="left">The left operand.</param>
			/// <param name="left">The right operand.</param>
			/// <returns>
			/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are
			/// <em>not</em> equal; otherwise, <see langword="false"/>.
			/// </returns>
			public static bool operator !=(ValidateParameters left, ValidateParameters right)
			{
				return !(left == right);
			}
			#endregion
		}
		/// <summary>
		/// Communicates detailed validation results.
		/// </summary>
		private ref struct ValidateResult
		{
			#region Fields & Properties
			/// <summary>
			/// Indicates whether the value provided by <see cref="ValidateParameters.City"/> is invalid.
			/// </summary>
			public Boolean CityIsInvalid;
			/// <summary>
			/// Contains the error message to include in instances of <see cref="ArgumentException"/> thrown 
			/// by <see cref="Create(string, string)"/> if <see cref="CityIsInvalid"/> is set to <see langword="true"/>.
			/// </summary>
			public String CityError;
			/// <summary>
			/// Indicates whether the value provided by <see cref="ValidateParameters.Street"/> is invalid.
			/// </summary>
			public Boolean StreetIsInvalid;
			/// <summary>
			/// Contains the error message to include in instances of <see cref="ArgumentException"/> thrown 
			/// by <see cref="Create(string, string)"/> if <see cref="StreetIsInvalid"/> is set to <see langword="true"/>.
			/// </summary>
			public String StreetError;

			/// <summary>
			/// Gets a default (valid) instance.
			/// </summary>
			public static ValidateResult Valid => default;

			/// <summary>
			/// Gets a value indicating whether all validation results are valid.
			/// </summary>
			public Boolean IsValid =>
				!CityIsInvalid &&
				!StreetIsInvalid;
			#endregion
			#region Equals & Hashing
			/// <summary>
			/// <see cref="ValidateResult"/> may not be boxed due to being a <see langword="ref"/> <see langword="struct"/>. 
			/// Therefore, calling <see cref="Equals(object)"/> is not supported.
			/// </summary>
			/// <exception cref="NotSupportedException"></exception>
			public override bool Equals(object obj)
			{
				throw new NotSupportedException("Address.ValidateResult may not be boxed due to being a ref struct. Therefore, calling Equals(object obj) is not supported.");
			}
			/// <summary>
			/// <see cref="ValidateResult"/> does not support calling <see cref="GetHashCode"/>.
			/// </summary>
			/// <exception cref="NotSupportedException"></exception>
			public override int GetHashCode()
			{
				throw new NotSupportedException();
			}
			#endregion
		}
		/// <summary>
		/// Contains validation results for construction parameters.
		/// </summary>
		public readonly struct IsValidResult : IEquatable<IsValidResult>
		{
			#region Constructor & Fields
			/// <summary>
			/// Initializes a new instance.
			/// </summary>
			/// <param name="cityIsValid">
			/// Indicates whether the parameter provided for <see cref="City"/> was invalid.
			/// </param>
			/// <param name="streetIsValid">
			/// Indicates whether the parameter provided for <see cref="Street"/> was invalid.
			/// </param>
			public IsValidResult(bool cityIsValid, bool streetIsValid)
			{
				CityIsValid = cityIsValid;
				StreetIsValid = streetIsValid;
			}

			/// <summary>
			/// Indicates whether the parameter provided for <see cref="City"/> was invalid.
			/// </summary>
			public readonly Boolean CityIsValid;
			/// <summary>
			/// Indicates whether the parameter provided for <see cref="Street"/> was invalid.
			/// </summary>
			public readonly Boolean StreetIsValid;
			#endregion
			#region Conversion & Deconstruction
			/// <summary>
			/// Implicitly converts an instance of <see cref="IsValidResult"/> to <see cref="Boolean"/>.
			/// The result will be true if all validity fields evaluate to <see langword="true"/>.
			/// </summary>
			/// <param name="result">The instance to implicitly convert.</param>
			public static implicit operator Boolean(IsValidResult result) =>
				result.CityIsValid && result.StreetIsValid;

			/// <summary>
			/// Deconstructs this instance into its constituent values.
			/// </summary>
			/// <param name="cityIsValid">
			/// The value contained in <see cref="CityIsValid"/>.
			/// </param>
			/// <param name="streetIsValid">
			/// The value contained in <see cref="StreetIsValid"/>.
			/// </param>
			public void Deconstruct(out Boolean cityIsValid, out Boolean streetIsValid)
			{
				cityIsValid = CityIsValid;
				streetIsValid = StreetIsValid;
			}
			#endregion
			#region Equality & Hashing
			/// <inheritdoc/>
			public override bool Equals(object obj)
			{
				return obj is IsValidResult result && Equals(result);
			}
			/// <inheritdoc/>
			public bool Equals(IsValidResult other)
			{
				return CityIsValid == other.CityIsValid &&
					   StreetIsValid == other.StreetIsValid;
			}
			/// <inheritdoc/>
			public override int GetHashCode()
			{
				return (CityIsValid, StreetIsValid).GetHashCode();
			}
			/// <summary>
			/// Indicates whether two instances of <see cref="IsValidResult"/> are equal.
			/// </summary>
			/// <param name="left">The left operand.</param>
			/// <param name="left">The right operand.</param>
			/// <returns>
			/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are
			/// equal; otherwise, <see langword="false"/>.
			/// </returns>
			public static bool operator ==(IsValidResult left, IsValidResult right)
			{
				return left.Equals(right);
			}
			/// <summary>
			/// Indicates whether two instances of <see cref="IsValidResult"/> are <em>not</em> equal.
			/// </summary>
			/// <param name="left">The left operand.</param>
			/// <param name="left">The right operand.</param>
			/// <returns>
			/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are
			/// <em>not</em> equal; otherwise, <see langword="false"/>.
			/// </returns>
			public static bool operator !=(IsValidResult left, IsValidResult right)
			{
				return !(left == right);
			}
			#endregion
		}
		#endregion
		#region Constructor & Fields
		public readonly String City;
		public readonly String Street;

		public static readonly Address Empty = default;
		#endregion
		#region Validation & Factories
		/// <summary>
		/// Validates a set of construction parameters.
		/// </summary>
		/// <param name="parameters">The parameters to validate.</param>
		/// <param name="result">The validation result to communicate validation with.</param>
		static partial void Validate(ValidateParameters parameters, ref ValidateResult result);
		/// <summary>
		/// Gets a value indicating the validity of the parameters passed, were they to be used for constructing a new instance of
		/// <see cref="Address"/>.
		/// </summary>
		/// <param name="city">
		/// The potential <see cref="City"/> value whose validity to assert.
		/// </param>
		/// <param name="street">
		/// The potential <see cref="Street"/> value whose validity to assert.
		/// </param>
		/// <returns>
		/// A value indicating the validity of the parameters passed.
		/// </returns>
		public static IsValidResult IsValid(String city, String street)
		{
			var validateResult = ValidateResult.Valid;
			var validateParameters = new ValidateParameters(city, street);
			Validate(validateParameters, ref validateResult);

			var result = new IsValidResult(
				!validateResult.CityIsInvalid,
				!validateResult.StreetIsInvalid);

			return result;
		}
		/// <summary>
		/// Attempts to create a new instance of <see cref="Address"/>.
		/// </summary>
		/// <param name="city">
		/// The value to assign to the new instances <see cref="City"/>.
		/// </param>
		/// <param name="street">
		/// The value to assign to the new instances <see cref="Street"/>.
		/// </param>
		/// <param name="result">
		/// Upon returning, will contain a new instance of <see cref="Address"/> if
		/// one could be constructed using the parameters passed; otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if an instance of <see cref="Address"/> could be constructed
		/// using the parameters passed; otherwise, <see langword="false"/>.
		/// </returns>
		public static Boolean TryCreate(String city, String street, out Address result)
		{
			var validateResult = ValidateResult.Valid;
			var validateParameters = new ValidateParameters(city, street);
			Validate(validateParameters, ref validateResult);
			var isValid = validateResult.IsValid;

			if (isValid)
			{
				result = new Address(street, city);
			}
			else
			{
				result = Empty;
			}

			return isValid;
		}
		/// <summary>
		/// Creates a new instance of <see cref="Address"/>.
		/// </summary>
		/// <param name="city">
		/// The value to assign to the new instances <see cref="City"/>.
		/// </param>
		/// <param name="street">
		/// The value to assign to the new instances <see cref="Street"/>.
		/// </param>
		/// <returns>
		/// A new instance of <see cref="Address"/> if could be constructed
		/// using the parameters passed; otherwise, an <see cref="ArgumentException"/> will be thrown.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if the parameters passed are not valid construction values.
		/// </exception>
		public static Address Create(String city, String street)
		{
			var validateResult = ValidateResult.Valid;
			var validateParameters = new ValidateParameters(city, street);
			Validate(validateParameters, ref validateResult);

			if (!validateResult.IsValid)
			{
				String reasonMessage = null;
				String paramName = null;
				if (validateResult.CityIsInvalid)
				{
					reasonMessage = validateResult.CityError;
					paramName = "city";
				}
				else if (validateResult.StreetIsInvalid)
				{
					reasonMessage = validateResult.StreetError;
					paramName = "street";
				}

				String reason = null;
				if (reasonMessage != null)
				{
					reason = $" Reason: {reasonMessage}";
				}
				throw new ArgumentException($"The {paramName} provided for creating an instance of Address was not valid.{reason}", paramName);
			}

			return new Address(city, street);
		}
		#endregion
		#region Deconstruction & Transformation
		/// <summary>
		/// Deconstructs this instance into its constituent values.
		/// </summary>
		/// <param name="city">The value contained in <see cref="City"/>.</param>
		/// <param name="street">The value contained in <see cref="Street"/>.</param>
		public void Deconstruct(out String city, out String street)
		{
			city = City;
			street = Street;
		}
		/// <summary>
		/// Constructs a clone of this instance with its <see cref="City"/> value replaced.
		/// </summary>
		/// <param name="city">The value to replace <see cref="City"/> with.</param>
		/// <returns>A new clone of this instance with its <see cref="City"/> value replaced by <paramref name="city"/>.</returns>
		public Address WithCity(String city)
		{
			var result = Create(city, Street);

			return result;
		}
		/// <summary>
		/// Constructs a clone of this instance with its <see cref="Street"/> value replaced.
		/// </summary>
		/// <param name="street">The value to replace <see cref="Street"/> with.</param>
		/// <returns>A new clone of this instance with its <see cref="Street"/> value replaced by <paramref name="street"/>.</returns>
		public Address WithStreet(String street)
		{
			var result = Create(City, street);

			return result;
		}
		#endregion
		#region Equality & Hashíng
		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			return obj is Address address && Equals(address);
		}
		/// <inheritdoc/>
		public bool Equals(Address other)
		{
			return (City, Street).Equals((other.City, other.Street));
		}
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return (City, Street).GetHashCode();
		}
		/// <summary>
		/// Indicates whether two instances of <see cref="Address"/> are equal.
		/// </summary>
		/// <param name="left">The left operand.</param>
		/// <param name="left">The right operand.</param>
		/// <returns>
		/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are
		/// equal; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool operator ==(Address left, Address right)
		{
			return left.Equals(right);
		}
		/// <summary>
		/// Indicates whether two instances of <see cref="Address"/> are <em>not</em> equal.
		/// </summary>
		/// <param name="left">The left operand.</param>
		/// <param name="left">The right operand.</param>
		/// <returns>
		/// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are
		/// <em>not</em> equal; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool operator !=(Address left, Address right)
		{
			return !(left == right);
		}
		#endregion
	}
	internal class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine("Expecting Baker ex");
				_ = Address.Create("London", "Baker");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				Console.WriteLine("Expecting city empty ex");
				_ = Address.Create("", "Baker");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				Console.WriteLine("Expecting street empty ex");
				_ = Address.Create("London", "");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				Console.WriteLine("Expecting city empty ex");
				_ = Address.Create(null, "Baker");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				Console.WriteLine("Expecting street empty ex");
				_ = Address.Create("London", null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				Console.WriteLine("Expecting city empty ex");
				_ = Address.Create("", "");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			try
			{
				Console.WriteLine("Expecting city empty ex");
				_ = Address.Create(null, null);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			var a1 = Address.Create("London", "Main");
			Console.WriteLine(a1);
		}
	}
}