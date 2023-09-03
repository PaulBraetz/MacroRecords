using RhoMicro.MacroRecords;

using System;
using System.Linq;

internal partial class Program
{
    private static void Main(string[] args)
    {
    }
    [MacroRecord]
    [Field(typeof(int), "Field0")]
    partial struct TVO { }
}