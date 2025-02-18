using Microsoft.Extensions.DependencyInjection;
using Blackwing.Contracts.Requests;

namespace AspNetCoreSample.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddBlackwing();
        services.AddBlackwingHandlers();
        services.AddSingleton(typeof(IRequestPipeline<,>), typeof(ErrorLoggingBehavior<,>));
        services.AddSingleton(typeof(IRequestPipeline<,>), typeof(MessageValidatorBehavior<,>));

        return services;
    }
}
