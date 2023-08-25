using Microsoft.CodeAnalysis;

using RhoMicro.CodeAnalysis;

using System.Linq;

namespace RhoMicro.ValueObjectGenerator
{
    [Generator]
    internal sealed class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if(!(context.SyntaxContextReceiver is SyntaxContextReceiver receiver))
            {
                return;
            }

            var sources = receiver.Results
                .Select(b => b.AddParentType().Build().Source);

            context.AddSources(sources);
        }

        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
    }
}