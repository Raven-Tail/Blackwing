using Microsoft.Extensions.DependencyInjection;
using Blackwing.Contracts.Options;
using Blackwing.Contracts.Pipelines;
using Blackwing.Test.Integration.Helpers;

//[assembly:BlackwingOptions(DisableInterceptor = true)]

namespace Blackwing.Test.Integration;

public abstract class TestBase
{
    /// <summary>
    /// Setups a basic service collection, will register the following:
    /// <br /> 1. setupAction
    /// <br /> 2. AddBlackwing
    /// <br /> 3. AddBlackwingHandlers
    /// <br /> 4. AddScoped{RequestContext}
    /// <br /> 5. AddScoped{IRequestPipeline{Ping, Pong}, PingPipeLine}
    /// </summary>
    protected static IServiceProvider SetupServices(Action<IServiceCollection>? setupAction = null)
    {
        var services = new ServiceCollection();

        setupAction?.Invoke(services);
        services.AddBlackwing();
        services.AddBlackwingHandlers();

        services.AddScoped<RequestContext>();
        services.AddScoped<IRequestPipeline<Ping, Pong>, PingPipeLine>();

        return services.BuildServiceProvider();
    }
}
