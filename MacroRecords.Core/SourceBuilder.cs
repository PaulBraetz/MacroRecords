using System;
using System.Collections.Generic;
using System.Text;

namespace RhoMicro.MacroRecords
{
	internal sealed class SourceBuilder : IEquatable<SourceBuilder>
	{
		private readonly StringBuilder _builder = new StringBuilder();
		public SourceBuilder Append(String value)
		{
			_builder.Append(value);
			return this;
		}
		public SourceBuilder AppendLine(String value)
		{
			_builder.AppendLine(value);
			return this;
		}
		public SourceBuilder Append(Char value)
		{
			_builder.Append(value);
			return this;
		}
		public SourceBuilder Append(Int32 value)
		{
			_builder.Append(value);
			return this;
		}
		public SourceBuilder AppendLine(Char value)
		{
			_builder.Append(value).AppendLine();
			return this;
		}
		public SourceBuilder AppendLine()
		{
			_builder.AppendLine();
			return this;
		}
		public SourceBuilder ForEach<T>(IEnumerable<T> values, String separator, Action<SourceBuilder, T> append)
		{
			var appendSeparator = false;
			foreach(var value in values)
			{
				if(appendSeparator)
				{
					_builder.Append(separator);
				}

				append.Invoke(this, value);
				appendSeparator = true;
			}

			return this;
		}
		public SourceBuilder ForEach<T>(IEnumerable<T> values, Char separator, Action<SourceBuilder, T> append)
		{
			var appendSeparator = false;
			foreach(var value in values)
			{
				if(appendSeparator)
				{
					_builder.Append(separator);
				}

				append.Invoke(this, value);
				appendSeparator = true;
			}

			return this;
		}
		public SourceBuilder ForEach<T>(IEnumerable<T> values, Action<SourceBuilder, T> append)
		{
			var appendSeparator = false;
			foreach(var value in values)
			{
				if(appendSeparator)
				{
					_builder.AppendLine();
				}

				append.Invoke(this, value);
				appendSeparator = true;
			}

			return this;
		}

		public override String ToString() => _builder.ToString();
		public override Boolean Equals(Object obj) => Equals(obj as SourceBuilder);
		public Boolean Equals(SourceBuilder other) => !(other is null) && EqualityComparer<StringBuilder>.Default.Equals(_builder, other._builder);
		public override Int32 GetHashCode() => 1106445577 + EqualityComparer<StringBuilder>.Default.GetHashCode(_builder);
	}
}