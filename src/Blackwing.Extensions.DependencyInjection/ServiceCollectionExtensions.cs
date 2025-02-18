using Microsoft.Extensions.DependencyInjection.Extensions;
using Blackwing.Contracts;
using Blackwing.Services;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Blackwing service collection extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Blackwing Contracts (<see cref="IMediator"/> and <see cref="ISender"/>).
    /// </summary>
    /// <param name="services">The service collection where the contracts will be registered.</param>
    /// <param name="reflectionBased">Whether to use a reflection based mediator as a fallback to disable interceptors.</param>
    /// <returns>The <see cref="IServiceCollection"/> for further configuration.</returns>
    public static IServiceCollection AddBlackwing(this IServiceCollection services, bool reflectionBased = false)
    {
        if (reflectionBased)
        {
            services.TryAddScoped<MediatorReflection>();
            services.TryAddScoped<IMediator>(static sp => sp.GetRequiredService<MediatorReflection>());
            services.TryAddScoped<ISender>(static sp => sp.GetRequiredService<MediatorReflection>());
        }
        else
        {
            services.TryAddScoped<Mediator>();
            services.TryAddScoped<IMediator>(static sp => sp.GetRequiredService<Mediator>());
            services.TryAddScoped<ISender>(static sp => sp.GetRequiredService<Mediator>());
        }

        return services;
    }
}
