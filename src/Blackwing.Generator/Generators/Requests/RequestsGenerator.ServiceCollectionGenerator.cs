using Blackwing.Generator.Generators.Requests.Models;
using Blackwing.Generator.Options;
using System.Collections.Immutable;
using System.Text;

namespace Blackwing.Generator.Generators.Requests;

public static partial class RequestsGenerator
{
    private static class ServiceCollectionGenerator
    {
        public static (string HintName, string Content) Generate(in ImmutableArray<RequestHandlerToGenerate> handlersToGenerate, BlackwingOptions options)
        {
            // todo: Register diagnostics for the invalid names.

            var (classNamespace, validNamespace) = options.ServicesNamespace.IsValidNamespace() ? (options.ServicesNamespace, true) :( "Microsoft.Extensions.DependencyInjection", false);
            var classAccess = options.ServicesClassPublic ? "public" : "internal";
            var className = options.ServicesClassName.IsValidClassName() ? options.ServicesClassName : "BlackwingServiceCollectionExtensions";
            var methodName = options.ServicesMethodName.IsValidClassName() ? options.ServicesMethodName : "AddBlackwingHandlers";

            var sb = new StringBuilder();
            sb.AppendLine($$"""
            {{Constants.Generator.Header}}
        
            using {{Constants.Requests.Namespace}};
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

            {{Constants.Generator.Footer}}
            """);

            return ("_ServiceCollectionExtensions.g.cs", sb.ToString());
        }
    }
}