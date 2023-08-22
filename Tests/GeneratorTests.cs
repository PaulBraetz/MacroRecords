using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RhoMicro.ValueObjectGenerator;
using System.Collections.Immutable;
using System.ComponentModel.Design.Serialization;
using System.Xml.Linq;

namespace Tests
{
	[TestClass]
	public class GeneratorTests
	{
		//set in Setup
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		private SyntaxContextReceiver _receiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		private static (SyntaxNode, StructDeclarationSyntax, SemanticModel) GetContext(String source)
		{
			var tree = CSharpSyntaxTree.ParseText(source);

			var compilation = CSharpCompilation.Create("TestCompilation")
							  .AddReferences(
								 MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
								 MetadataReference.CreateFromFile(typeof(SyntaxContextReceiver).Assembly.Location))
							  .AddSyntaxTrees(tree);
			var root = tree.GetRoot();
			var declaration = root.DescendantNodes().OfType<StructDeclarationSyntax>().Single();

			return (root, declaration, compilation.GetSemanticModel(tree));
		}

		private static Object?[][] Sources
		{
			get
			{
				var typeVariations = new[]
				{
					typeof(String),
					typeof(Int32)
				};
				return new Object?[][]{
					//Type type, String spec, String visibility, String name, Boolean withToString
					new Object[]
					{
						 "Values must be alphanumeric.", "public", "TestStringValueObject", false
					},
					new Object[]
					{
						 "Values must be alphanumeric.", "public", "TestStringValueObject", true
					},
					new Object[]
					{
						 "Values must be alphanumeric.", "public", "MyVOForTesting", false
					},
					new Object[]
					{
						 "Values must be alphanumeric.", "public", "MyVOForTesting", true
					},
					new Object[]
					{
						"Values must be alphanumeric.", "internal", "TestStringValueObject", false
					},
					new Object[]
					{
						 "Values must be alphanumeric.", "internal", "TestStringValueObject", true
					},
					new Object[]
					{
						"Values must be alphanumeric.", "internal", "MyVOForTesting", false
					},
					new Object[]
					{
						"Values must be alphanumeric.", "internal", "MyVOForTesting", true
					},
					//
					new Object?[]
					{
						null, "public", "TestStringValueObject", false
					},
					new Object?[]
					{
						null, "public", "TestStringValueObject", true
					},
					new Object?[]
					{
						null, "public", "MyVOForTesting", false
					},
					new Object?[]
					{
						null, "public", "MyVOForTesting", true
					},
					new Object?[]
					{
						null, "internal", "TestStringValueObject", false
					},
					new Object?[]
					{
						null, "internal", "TestStringValueObject", true
					},
					new Object?[]
					{
						null, "internal", "MyVOForTesting", false
					},
					new Object?[]
					{
						null, "internal", "MyVOForTesting", true
					}
				}.SelectMany((ps, i) => typeVariations.Select(t => ps.Prepend(t).ToArray()))
				.ToArray();
			}
		}

		[TestInitialize]
		public void Setup()
		{
			_receiver = new();
		}

		[TestMethod]
		[DynamicData(nameof(Sources))]
		public void TestReceiver(Type type, String spec, String visibility, String name, Boolean withToString)
		{
			//Arrange
			var source = Templates.GetSource(type, spec, visibility, name, withToString);
			var (root, structDeclaration, model) = GetContext(source);
			var nodes = root.DescendantNodesAndSelf();

			//Act
			foreach (var node in nodes)
			{
				_receiver.OnVisitSyntaxNode(model, node);
			}

			//Assert
			Assert.AreEqual(1, _receiver.Results.Count);
			var info = _receiver.Results.Single();
			Assert.AreEqual(structDeclaration, info.Declaration);
			Assert.AreEqual(spec, info.Attribute.ValueSpecification);
			Assert.AreEqual(type, info.Attribute.WrappedType);
		}

		[TestMethod]
		[DynamicData(nameof(Sources))]
		public void TestGenerator(Type type, String spec, String visibility, String name, Boolean withToString)
		{
			//Arrange
			var source = Templates.GetSource(type, spec, visibility, name, withToString);
			var (root, structDeclaration, model) = GetContext(source);

			//Act
			_receiver.OnVisitSyntaxNode(model, structDeclaration);
			var sources = _receiver.Results.Select(i => i.GeneratePartial()).ToArray();

			//Assert
			Assert.AreEqual(1, sources.Length);

			var actualText = sources[0].Source.Text;
			actualText = actualText[actualText.IndexOf("namespace")..];
			actualText = actualText[..actualText.IndexOf("#pragma warning restore\r\n")];
			var actualTree = CSharpSyntaxTree.ParseText(actualText);

			var expected = Templates.GetExpected(type, spec, visibility, name, !withToString);
			var expectedTree = CSharpSyntaxTree.ParseText(expected);

			var condition = expectedTree.IsEquivalentTo(actualTree);

			Assert.IsTrue(condition);
		}
	}
}