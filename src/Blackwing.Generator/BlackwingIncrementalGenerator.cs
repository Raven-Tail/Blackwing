using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Blackwing.Generator.Generators;
using Blackwing.Generator.Models;

namespace Blackwing;

#pragma warning disable RSEXPERIMENTAL002 // Experimental interceptable location API

[Generator]
public sealed partial class BlackwingIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get disable Blackwing interceptor property.
        var disableBlackwingInterceptorProp = context
            .AnalyzerConfigOptionsProvider
            .Select(static (config, _) =>
                // Get the value, check if it's set to 'true', otherwise return false
                config.GlobalOptions.TryGetValue(Constants.BuildProperties.DisableBlackwingInterceptor, out var d) && d.Equals("true", StringComparison.OrdinalIgnoreCase)
            );

        // Get user options.
        var options = context.CompilationProvider
            .Select(static (compilation, ct) => GetOptionsFromCompilation(compilation, ct))
            .Combine(disableBlackwingInterceptorProp)
            .Select(static (combined, ct) => combined.Left.WithDisableInterceptor(combined.Right));

        // Generate handler execution code.
        var handlersToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } or RecordDeclarationSyntax { BaseList.Types.Count: > 0 },
                transform: static (context, ct) => HandlerTransform(context, ct))
            .Where(static handler => handler.HasValue)
            .Select(static (handler, ct) => handler!.Value);

        context.RegisterSourceOutput(handlersToGenerate.Combine(options), static (spc, handlersWithOptions) =>
        {
            var (source, options) = handlersWithOptions;
            var (filename, content) = HandlerGenerator.Generate(source, options);
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

    static BlackwingOptions GetOptionsFromCompilation(Compilation compilation, in CancellationToken ct)
    {
        var options = new BlackwingOptions();
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
        var symbol = context.Node switch
        {
            ClassDeclarationSyntax classSyntax => context.SemanticModel.GetDeclaredSymbol(classSyntax, ct),
            RecordDeclarationSyntax recordSyntax => context.SemanticModel.GetDeclaredSymbol(recordSyntax, ct),
            _ => null
        };

        if (symbol is not { IsAbstract: false, AllInterfaces.IsEmpty: false })
            return null;

        foreach (var interfaceSymbol in symbol.AllInterfaces)
        {
            if (ImplementsIRequestHandler(interfaceSymbol) &&
                ImplementsIRequest(interfaceSymbol.TypeArguments[0], interfaceSymbol.TypeArguments[1]))
            {
                return new HandlerToGenerate(symbol, interfaceSymbol);
            }
        }
        return null;

        static bool ImplementsIRequestHandler(INamedTypeSymbol handler) => handler is
        {
            Name: Constants.Handlers.IRequestHandler,
            IsGenericType: true, TypeParameters.Length: 2, TypeArguments.Length: 2,
            ContainingAssembly.Name: Constants.Assembly.Name,
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
            if (requestNode is null) return null;

            // We can't intercept abstract types, interfaces and generic types.
            var requestInfo = context.SemanticModel.GetTypeInfo(requestNode).Type as INamedTypeSymbol;
            if (requestInfo is null or {IsAbstract:true } or { IsGenericType: true })
                return null;

            var request = requestInfo.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var response = targetMethod.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var sender = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // Interfaces need to be the root ISender interface to be intercepted.
            if (type.TypeKind is TypeKind.Interface)
                sender = $"global::{Constants.Sender.ISenderFull}";

            // Return the location details and the fully quialified type details.
            return new InterceptorToGenerate(location, sender, request, response);
        }

        // Not an interceptor location we're interested in 
        return null;

        static bool TypeIsISender(ITypeSymbol handler) => handler is INamedTypeSymbol
        {
            Name: Constants.Sender.ISender,
            IsGenericType: false, TypeParameters.Length: 0, TypeArguments.Length: 0,
            ContainingAssembly.Name: Constants.Assembly.Name,
        };

        static bool ImplementsISender(INamedTypeSymbol handler) => handler is
        {
            Name: Constants.Sender.ISender,
            IsGenericType: false, TypeParameters.Length: 0, TypeArguments.Length: 0,
            ContainingAssembly.Name: Constants.Assembly.Name,
        };
    }
}
