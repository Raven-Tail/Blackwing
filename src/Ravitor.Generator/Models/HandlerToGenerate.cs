using Microsoft.CodeAnalysis;

namespace Ravitor.Generator.Models;

public readonly record struct HandlerToGenerate
{
    public readonly string WrapperHandler;
    public readonly string RequestHandler;
    public readonly string Request;
    public readonly string Response;
    public readonly string WrapperNamespace;

    public HandlerToGenerate(INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceSymbol)
    {
        WrapperHandler = WrapperName(classSymbol, true);
        RequestHandler = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        Request = interfaceSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        Response = interfaceSymbol.TypeArguments[1].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        WrapperNamespace = classSymbol.ContainingNamespace.IsGlobalNamespace
            ? Constants.Generator.Namespace
            : $"{Constants.Generator.Namespace}.{classSymbol.ContainingNamespace.ToDisplayString()}";
    }

    private static string WrapperName(INamedTypeSymbol symbol, bool wrap)
    {
        if (symbol.ContainingType is null)
            return wrap ? $"{symbol.Name}Wrapper" : symbol.Name;

        return $"{WrapperName(symbol.ContainingType, false)}_{symbol.Name}Wrapper";
    }
}
