using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RhoMicro.MacroRecords.Core
{
    internal sealed class SourceBuilder : IEquatable<SourceBuilder>
    {
        private readonly StringBuilder _builder = new StringBuilder();
        public SourceBuilder Append(ISymbol symbol)
        {
            if(symbol is ITypeSymbol typeSymbol)
            {
                var identifier = TypeIdentifier.Create(typeSymbol).ToString();
                return Append(identifier);
            }

            return Append(symbol?.ToDisplayString() ?? String.Empty);
        }

        public SourceBuilder Append(ITypeSymbol symbol) => Append(symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        public SourceBuilder Append(Visibility visibility) => Append(Util.GetString(visibility));
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