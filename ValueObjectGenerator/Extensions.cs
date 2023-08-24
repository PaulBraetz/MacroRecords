using System;
using System.Collections.Generic;
using System.Text;

namespace RhoMicro.ValueObjectGenerator
{
	internal static class Extensions
	{
		public static StringBuilder Nl(this StringBuilder builder) => builder.Append(Environment.NewLine);
		public static StringBuilder ForEach<T>(
			this StringBuilder builder,
			IEnumerable<T> values,
			String separator,
			Action<StringBuilder, T> append)
		{
			Boolean appendSeparator = false;
			foreach (var value in values)
			{
				if (appendSeparator)
				{
					builder.Append(separator);
				}

				append.Invoke(builder, value);
				appendSeparator = true;
			}

			return builder;
		}
		public static StringBuilder ForEach<T>(
			this StringBuilder builder,
			IEnumerable<T> values,
			Action<StringBuilder, T> append) => builder.ForEach(values, Environment.NewLine, append);
		public static StringBuilder ForEach<T>(
			this StringBuilder builder,
			IEnumerable<T> values,
			String separator) =>
			builder.ForEach(values, separator, (b, v) => b.Append(v));
		public static StringBuilder ForEach<T>(
			this StringBuilder builder,
			IEnumerable<T> values) =>
			builder.ForEach(values, (b, v) => b.Append(v));
		public static StringBuilder ForEach(
			this StringBuilder builder,
			IEnumerable<String> values,
			String separator) =>
			builder.ForEach(values, separator, (b, v) => b.Append(v));
		public static StringBuilder ForEach(
			this StringBuilder builder,
			IEnumerable<String> values) =>
			builder.ForEach(values, (b, v) => b.Append(v));
	}
}
