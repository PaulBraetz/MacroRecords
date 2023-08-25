using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValueObjectGenerator.Tests
{
    internal static class Assertions
    {
        public static void AreEquivalent(String expected, String actual)
        {
            var options = new CSharpParseOptions(LanguageVersion.CSharp7_2,
                DocumentationMode.None,
                SourceCodeKind.Regular);

            AreEquivalent(
                expected: expected,
                actual: actual,
                options);
        }

        public static void AreEquivalent(String expected, String actual, CSharpParseOptions options)
        {
            if(expected == actual)
            {
                return;
            }

            var expectedTree = CSharpSyntaxTree.ParseText(expected, options);
            var actualTree = CSharpSyntaxTree.ParseText(actual, options);

            var condition = expectedTree.IsEquivalentTo(actualTree);
            Assert.IsTrue(condition);
        }
    }
}
