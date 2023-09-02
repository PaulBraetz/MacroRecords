using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhoMicro.MacroRecords.Tests
{
    internal static class Assertions
    {
        private static readonly CSharpParseOptions _options = new(
                LanguageVersion.CSharp7_3,
                DocumentationMode.None,
                SourceCodeKind.Regular);


        public static void AreEquivalent(String expected, String actual)
        {
            if(expected == actual)
            {
                return;
            }

            var expectedTree = CSharpSyntaxTree.ParseText(expected, _options);
            var actualTree = CSharpSyntaxTree.ParseText(actual, _options);

            var condition = expectedTree.IsEquivalentTo(actualTree);
            Assert.IsTrue(condition);
        }
    }
}
