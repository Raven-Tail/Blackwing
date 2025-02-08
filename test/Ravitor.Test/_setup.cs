using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts.Requests;
using Ravitor.Services;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VerifyTests;
using VerifyTUnit;

namespace Ravitor.Test;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}

public abstract class TestBase
{
    private const string Resources = "Resources";
    private const string Snapshots = "Snapshots";

    private static readonly PortableExecutableReference[] References =
    [
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(IRequest).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Mediator).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(ServiceLifetime).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(ServiceProvider).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(MulticastDelegate).Assembly.Location),
        .. AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).Select(x => MetadataReference.CreateFromFile(x.Location))
    ];

    protected async Task Verify(bool ignoreErrors = false, [CallerMemberName] string resourceFileName = null!)
    {
        var executingDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        await Assert.That(executingDir).IsNotNull();

        var resourceFile = Path.Combine(executingDir, Resources, GetType().Name, $"{resourceFileName}.cs");
        var source = await File.ReadAllTextAsync(resourceFile);
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create(
            assemblyName: GetType().Name,
            syntaxTrees: [syntaxTree],
            references: References
        );

        AssertNoErrors(compilation.GetDiagnostics(), ignoreErrors);

        var generator = new RavitorIncrementalGenerator();
        var driver = (GeneratorDriver)CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        AssertNoErrors(driver.GetRunResult().Diagnostics, ignoreErrors);

        await Verifier
            .Verify(driver)
            //.ScrubLinesContaining("InterceptsLocation")
            .UseDirectory(Path.Combine(Snapshots, GetType().Name, resourceFileName))
            .UseFileName("#");
    }

    private static void AssertNoErrors(ImmutableArray<Diagnostic> diagnostics, bool skip)
    {
        if (skip) return;

        var errors = diagnostics.Where(x => x.Severity is DiagnosticSeverity.Error);
        if (errors.Any() is false)
            return;

        var texts = errors.Select(static x => x.ToString());
        var reason = string.Join(Environment.NewLine, texts);
        Assert.Fail(reason);
    }
}
