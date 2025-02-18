using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts.Pipelines;

namespace AspNetCoreSample.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddRavitor();
        services.AddRavitorHandlers();
        services.AddSingleton(typeof(IRequestPipeline<,>), typeof(ErrorLoggingBehavior<,>));
        services.AddSingleton(typeof(IRequestPipeline<,>), typeof(MessageValidatorBehavior<,>));

        return services;
    }
}
