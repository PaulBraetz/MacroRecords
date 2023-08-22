using Microsoft.CodeAnalysis;
using RhoMicro.CodeAnalysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace RhoMicro.ValueObjectGenerator
{
	[Generator]
	internal sealed class Generator : ISourceGenerator
	{
		public void Execute(GeneratorExecutionContext context)
		{
			context.AddSource(AttributeUnits.GeneratedValueObject.GeneratedType.Source);

			if (!(context.SyntaxContextReceiver is SyntaxContextReceiver receiver))
			{
				return;
			}

			var sources = receiver.Results
				.Select(i=>i.GeneratePartial())
				.Select(t=>t.Source);

			context.AddSources(sources);
		}

        public void Initialize(GeneratorInitializationContext context) => 
			context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
    }
}