using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace RhoMicro.CodeAnalysis
{
    internal readonly struct Error
    {
        private Error(ImmutableArray<Exception> exceptions)
        {
            Exceptions = exceptions;

            _string =
$@"/*
An error has occured:
{String.Join($"\n{Enumerable.Range(0, 25).Select(i => '*')}\n", exceptions.Select((e, i) => $"{i}:\n{e}\n{e.StackTrace}"))}
*/";
        }

        public ImmutableArray<Exception> Exceptions { get; }
        private readonly String _string;

        public static Error Create() => new Error(ImmutableArray.Create<Exception>());
        public Error With(Exception exception) => new Error(Exceptions.Add(exception));
        public Error With(params Exception[] exceptions) => new Error(Exceptions.AddRange(exceptions));
        public Error WithFlattened(Exception exception) => With(Flatten(exception).ToArray());
        public Error WithFlattened(params Exception[] exceptions) => With(exceptions.SelectMany(Flatten).ToArray());
        private static IEnumerable<Exception> Flatten(Exception exception)
        {
            return exception is AggregateException aggregateException
                ? aggregateException.InnerExceptions.SelectMany(Flatten)
                : (new[] { exception });
        }
        public override String ToString() => _string;

        public static implicit operator String(Error error) => error.ToString();
    }
}
