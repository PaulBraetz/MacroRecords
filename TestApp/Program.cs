using RhoMicro.MacroRecords;

using System;

internal partial class Program
{
    private static void Main(string[] args)
    {
    }
    [MacroRecord(Options = RecordOptions.Default)]
    [Field(typeof(int), "Field", Options = FieldOptions.All)]
    partial struct TVO { }
}