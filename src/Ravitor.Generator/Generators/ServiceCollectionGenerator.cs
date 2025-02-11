using Ravitor.Generator.Models;
using System.Collections.Immutable;
using System.Text;

namespace Ravitor.Generator.Generators;

internal static class ServiceCollectionGenerator
{
    public static (string HintName, string Content) Generate(in ImmutableArray<HandlerToGenerate> handlersToGenerate)
    {
        // todo: Allow to define if class should be public or internal (internal default).
        // todo: Allow to define Class Name (ServiceCollectionExtensions default).
        // todo: Allow to define Method name (AddRavitorHandlers default).
        // todo: Allow to define if all three should be registered or if the second registration should be removed (add it by default).

        var sb = new StringBuilder();
        sb.AppendLine($$"""
        {{Constants.Header}}

        using Ravitor.Contracts.Handlers;
        
        namespace Microsoft.Extensions.DependencyInjection;
        
        internal static class ServiceCollectionExtensions
        {
            public static IServiceCollection AddRavitorHandlers(this IServiceCollection services)
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
