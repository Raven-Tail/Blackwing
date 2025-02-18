using Blackwing.Generator.Generators.Requests;
using Microsoft.CodeAnalysis;

namespace Blackwing;

#pragma warning disable RSEXPERIMENTAL002 // Experimental interceptable location API

[Generator]
public sealed partial class BlackwingIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        RequestsGenerator.Initialize(context);
    }
}
