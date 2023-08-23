namespace Tests
{
	internal static class Templates
	{
		private const String SUPPLIED_SOURCE_WITH_TOSTRING =
@"using {0};
using RhoMicro.ValueObjectGenerator;

namespace TestNamespace
{
	[GeneratedValueObject(typeof({1}), ValueSpecification = {2})]
	{3} readonly partial struct {4}
	{
		public override String ToString()
		{
			return base.ToString();
		}
	}
}";
		private const String SUPPLIED_SOURCE_WITHOUT_TOSTRING =
@"using {0};
using RhoMicro.ValueObjectGenerator;

namespace TestNamespace
{
	[GeneratedValueObject(typeof({1}), ValueSpecification = {2})]
	{3} readonly partial struct {4}
	{
	}
}";
		public static String GetSource(Type type, String spec, String visibility, String name, Boolean withToString) =>
				(withToString ?
					SUPPLIED_SOURCE_WITH_TOSTRING :
					SUPPLIED_SOURCE_WITHOUT_TOSTRING)
			.Replace("{0}", type.Namespace)
			.Replace("{1}", type.Name)
			.Replace("{2}", spec == null ?
					"null" :
					$"\"{spec}\"")
			.Replace("{3}", visibility)
			.Replace("{4}", name);

		private const String EXPECTED_GENERATED_SOURCE_WITH_TOSTRING =
@"namespace TestNamespace
{
    {0} partial struct {1} : IEquatable<{1}>
    {
        private {1}({2} value)
        {
            _value = value;
        }

        private readonly {2} _value;
        public static readonly {1} Empty = new();
        public static partial System.Boolean IsValid({2} value);
        public static System.Boolean TryCreate({2} value, out {1} result)
        {
            System.Boolean isValid = IsValid(value);
            if (isValid)
            {
                result = new {1}(value);
            }
            else
            {
                result = Empty;
            }

            return isValid;
        }

        public static {1} Create({2} value)
        {
            if (!TryCreate(value, out var result))
            {
                throw new System.ArgumentException(""The value provided for creating an instance of {1} was not valid.{3}"", ""value"");
            }

            return result;
        }

        public static implicit operator {2}({1} instance)
        {
            return instance._value;
        }

        public static explicit operator {1}({2} value)
        {
            return Create(value);
        }

        public override System.Boolean Equals(System.Object obj)
        {
            return obj is {1} instance && Equals(instance);
        }

        public System.Boolean Equals({1} other)
        {
            return System.Collections.Generic.EqualityComparer<{2}>.Default.Equals(_value, other._value);
        }

        public override System.Int32 GetHashCode()
        {
            return System.Collections.Generic.EqualityComparer<{2}>.Default.GetHashCode(_value);
        }

        public static System.Boolean operator ==({1} left, {1} right)
        {
            return left.Equals(right);
        }

        public static System.Boolean operator !=({1} left, {1} right)
        {
            return !(left == right);
        }

        public override System.String ToString()
        {
            return _value{4}.ToString();
        }
    }
}";
		private const String EXPECTED_GENERATED_SOURCE_WITHOUT_TOSTRING =
@"namespace TestNamespace
{
    {0} partial struct {1} : IEquatable<{1}>
    {
        private {1}({2} value)
        {
            _value = value;
        }

        private readonly {2} _value;
        public static readonly {1} Empty = new();
        public static partial System.Boolean IsValid({2} value);
        public static System.Boolean TryCreate({2} value, out {1} result)
        {
            System.Boolean isValid = IsValid(value);
            if (isValid)
            {
                result = new {1}(value);
            }
            else
            {
                result = Empty;
            }

            return isValid;
        }

        public static {1} Create({2} value)
        {
            if (!TryCreate(value, out var result))
            {
                throw new System.ArgumentException(""The value provided for creating an instance of {1} was not valid.{3}"", ""value"");
            }

            return result;
        }

        public static implicit operator {2}({1} instance)
        {
            return instance._value;
        }

        public static explicit operator {1}({2} value)
        {
            return Create(value);
        }

        public override System.Boolean Equals(System.Object obj)
        {
            return obj is {1} instance && Equals(instance);
        }

        public System.Boolean Equals({1} other)
        {
            return System.Collections.Generic.EqualityComparer<{2}>.Default.Equals(_value, other._value);
        }

        public override System.Int32 GetHashCode()
        {
            return System.Collections.Generic.EqualityComparer<{2}>.Default.GetHashCode(_value);
        }

        public static System.Boolean operator ==({1} left, {1} right)
        {
            return left.Equals(right);
        }

        public static System.Boolean operator !=({1} left, {1} right)
        {
            return !(left == right);
        }
    }
}";
		public static String GetExpected(Type type, String spec, String visibility, String name, Boolean withToString) =>
			(withToString ?
					EXPECTED_GENERATED_SOURCE_WITH_TOSTRING :
					EXPECTED_GENERATED_SOURCE_WITHOUT_TOSTRING)
			.Replace("{0}", visibility)
			.Replace("{1}", name)
			.Replace("{2}", type.FullName)
			.Replace("{3}", spec == null ?
					String.Empty :
					$" Reason: {spec}")
			.Replace("{4}", type.IsValueType && Nullable.GetUnderlyingType(type) == null ?
					String.Empty :
					"?");
	}
}