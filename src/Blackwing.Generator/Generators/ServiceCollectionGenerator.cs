using Blackwing.Generator.Models;
using System.Collections.Immutable;
using System.Text;

namespace Blackwing.Generator.Generators;

internal static class ServiceCollectionGenerator
{
    public static (string HintName, string Content) Generate(in ImmutableArray<HandlerToGenerate> handlersToGenerate, BlackwingOptions options)
    {
        // todo: Allow to define if all three should be registered or if the second registration should be removed (add it by default).
        // todo: Register diagnostics for the invalid names.

        var (classNamespace, validNamespace) = options.ServicesNamespace.IsValidNamespace() ? (options.ServicesNamespace, true) :( "Microsoft.Extensions.DependencyInjection", false);
        var classAccess = options.ServicesClassPublic ? "public" : "internal";
        var className = options.ServicesClassName.IsValidClassName() ? options.ServicesClassName : "BlackwingServiceCollectionExtensions";
        var methodName = options.ServicesMethodName.IsValidClassName() ? options.ServicesMethodName : "AddBlackwingHandlers";

        var sb = new StringBuilder();
        sb.AppendLine($$"""
        {{Constants.Header}}
        
        using Blackwing.Contracts.Handlers;
        """);
        if (validNamespace)
        {
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        }
        sb.AppendLine();
        sb.AppendLine($$"""
        namespace {{classNamespace}};
        
        {{classAccess}} static class {{className}}
        {
            public static IServiceCollection {{methodName}}(this IServiceCollection services)
            {
        """);
        foreach (var handlerToGenerate in handlersToGenerate)
        {
            var handler = handlerToGenerate.RequestHandler;
            var wrapper = $"global::{handlerToGenerate.WrapperNamespace}.{handlerToGenerate.WrapperHandler}";
            var request = handlerToGenerate.Request;
            var response = handlerToGenerate.Response;

            sb.AppendLine($$"""
                    services.AddScoped<{{handler}}>();
                    services.AddScoped<{{wrapper}}>();
                    services.AddScoped<IRequestHandler<{{request}}, {{response}}>, {{wrapper}}>(static sp => sp.GetRequiredService<{{wrapper}}>());

            """);
        }
        sb.AppendLine($$"""
                return services;
            }
        }

        {{Constants.Footer}}
        """);

        return ("_ServiceCollectionExtensions.g.cs", sb.ToString());
    }
}
