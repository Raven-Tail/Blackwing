using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Ravitor.Generator.Generators;
using Ravitor.Generator.Models;

namespace Ravitor;

#pragma warning disable RSEXPERIMENTAL002 // Experimental interceptable location API

[Generator]
public sealed partial class RavitorIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get disable ravitor interceptor property.
        var disableRavitorInterceptorProp = context
            .AnalyzerConfigOptionsProvider
            .Select(static (config, _) =>
                // Get the value, check if it's set to 'true', otherwise return false
                config.GlobalOptions.TryGetValue(Constants.BuildProperties.DisableRavitorInterceptor, out var d) && d.Equals("true", StringComparison.OrdinalIgnoreCase)
            );

        // Get user options.
        var options = context.CompilationProvider
            .Select(static (compilation, ct) => GetOptionsFromCompilation(compilation, ct))
            .Combine(disableRavitorInterceptorProp)
            .Select(static (combined, ct) => combined.Left.WithDisableInterceptor(combined.Right));

        // Generate handler execution code.
        var handlersToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 },
                transform: static (context, ct) => HandlerTransform(context, ct))
            .Where(static handler => handler.HasValue)
            .Select(static (handler, ct) => handler!.Value);

        context.RegisterSourceOutput(handlersToGenerate, static (spc, source) =>
        {
            var (filename, content) = HandlerGenerator.Generate(source);
            spc.AddSource(filename, content);
        });

        // Generate service collection extension.
        var extensionToGenerate = handlersToGenerate.Collect();

        context.RegisterSourceOutput(extensionToGenerate.Combine(options), static (spc, handlersWithOptions) =>
        {
            var (sources, options) = handlersWithOptions;
            var (filename, content) = ServiceCollectionGenerator.Generate(sources, options);
            spc.AddSource(filename, content);
        });

        // Generate interceptors.
        var interceptorsToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.ValueText: Constants.Sender.SendMethod } },
                transform: static (context, ct) => InterceptorTransform(context, ct))
            .Where(static candidate => candidate.HasValue)
            .Select(static (candidate, ct) => candidate!.Value)
            .Collect();

        context.RegisterSourceOutput(interceptorsToGenerate.Combine(options), static (spc, interceptorsWithOptions) =>
        {
            var (sources, options) = interceptorsWithOptions;
            if (options.DisableInterceptor || sources.IsEmpty) return;

            var (filename, content) = InterceptorGenerator.Generate(sources);
            spc.AddSource(filename, content);
        });
    }

    static RavitorOptions GetOptionsFromCompilation(Compilation compilation, in CancellationToken ct)
    {
        var options = new RavitorOptions();
        foreach (var attribute in compilation.Assembly.GetAttributes())
        {
            if (attribute.AttributeClass?.ContainingAssembly.Name is not Constants.Assembly.Name)
                continue;

            // This is the attribute, check all of the named arguments
            foreach (var (key, value) in attribute.NamedArguments)
            {
                options.SetValue(key, value);
            }
            return options;
        }
        return options;
    }

    static HandlerToGenerate? HandlerTransform(in GeneratorSyntaxContext context, in CancellationToken ct)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, ct);

        if (classSymbol is not { IsAbstract: false })
            return null;

        foreach (var interfaceSymbol in classSymbol.AllInterfaces)
        {
            if (ImplementsIRequestHandler(interfaceSymbol) &&
                ImplementsIRequest(interfaceSymbol.TypeArguments[0], interfaceSymbol.TypeArguments[1]))
            {
                return new HandlerToGenerate(classSymbol, interfaceSymbol);
            }
        }
        return null;

        static bool ImplementsIRequestHandler(INamedTypeSymbol handler) => handler is
        {
            Name: Constants.Handlers.IRequestHandler,
            IsGenericType: true, TypeParameters.Length: 2, TypeArguments.Length: 2,
            ContainingAssembly.Name: Constants.Assembly.Name,
            //ContainingNamespace.name:  { ContainingNamespace.Name: Constants.BaseNamespaceName, Name: Constants.ContractsNamespace },
        };

        static bool ImplementsIRequest(ITypeSymbol request, ITypeSymbol response)
        {
            foreach (var interfaceSymbol in request.AllInterfaces)
            {
                if (interfaceSymbol is
                    {
                        Name: Constants.Requests.IRequest,
                        IsGenericType: true, TypeParameters.Length: 1, TypeArguments.Length: 1,
                        ContainingAssembly.Name: Constants.Assembly.Name,
                        //ContainingNamespace: { ContainingNamespace.Name: Constants.BaseNamespaceName, Name: Constants.ContractsNamespace }
                    })
                {
                    return response.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == interfaceSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
            }
            return false;
        }
    }

    static InterceptorToGenerate? InterceptorTransform(in GeneratorSyntaxContext context, in CancellationToken ct)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.ArgumentList.Arguments.Count is 1 or 2
            // Get the semantic definition of the method invocation if the user has an invalid call (no IRequest implementation this will be false)
            && context.SemanticModel.GetOperation(context.Node, ct) is IInvocationOperation targetOperation
            // This is the main check - is the method a Send invocation
            && targetOperation.TargetMethod is { Name: Constants.Sender.SendMethod, TypeArguments.Length: 1, Parameters.Length: 2 } targetMethod
            // Grab the Type of the sender on which this is being invoked. It must be a reference type (class)
            && targetOperation.Instance?.Type is { IsReferenceType: true } type
            // The Type must implement ISender
            && (TypeIsISender(type) || type.AllInterfaces.Any(ImplementsISender))
            // If we get to here, we know we want to generate an interceptor,
            // so use the experimental GetInterceptableLocation() API to get the data
            // we need. This returns null if the location is not interceptable.
            && context.SemanticModel.GetInterceptableLocation(invocation) is { } location)
        {
            var requestNode = invocation.ArgumentList.Arguments[0].ChildNodes().FirstOrDefault();
            var requestInfo = context.SemanticModel.GetTypeInfo(requestNode);
            var request = requestInfo.Type?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            if (request is null)
                return null;

            var sender = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var response = targetMethod.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // Return the location details and the full type details
            return new InterceptorToGenerate(location, sender, request, response);
        }

        // Not an interceptor location we're interested in 
        return null;

        static bool TypeIsISender(ITypeSymbol handler) => handler is
        {
            Name: Constants.Sender.ISender,
            //IsGenericType: false, TypeParameters.Length: 0, TypeArguments.Length: 0,
            ContainingAssembly.Name: Constants.Assembly.Name,
            //ContainingNamespace: { ContainingNamespace.Name: Constants.BaseNamespaceName, Name: Constants.ContractsNamespace },
        };

        static bool ImplementsISender(INamedTypeSymbol handler) => handler is
        {
            Name: Constants.Sender.ISender,
            IsGenericType: false, TypeParameters.Length: 0, TypeArguments.Length: 0,
            ContainingAssembly.Name: Constants.Assembly.Name,
            //ContainingNamespace: { ContainingNamespace.Name: Constants.BaseNamespaceName, Name: Constants.ContractsNamespace },
        };
    }
}
