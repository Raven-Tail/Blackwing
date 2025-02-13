using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts.Options;
using Ravitor.Contracts.Pipelines;
using Ravitor.Test.Integration.Helpers;

//[assembly:RavitorOptions(DisableInterceptor = true)]

namespace Ravitor.Test.Integration;

public abstract class TestBase
{
    /// <summary>
    /// Setups a basic service collection, will register the following:
    /// <br /> 1. setupAction
    /// <br /> 2. AddRavitor
    /// <br /> 3. AddRavitorHandlers
    /// <br /> 4. AddScoped{RequestContext}
    /// <br /> 5. AddScoped{IRequestPipeline{Ping, Pong}, PingPipeLine}
    /// </summary>
    protected static IServiceProvider SetupServices(Action<IServiceCollection>? setupAction = null)
    {
        var services = new ServiceCollection();

        setupAction?.Invoke(services);
        services.AddRavitor();
        services.AddRavitorHandlers();

        services.AddScoped<RequestContext>();
        services.AddScoped<IRequestPipeline<Ping, Pong>, PingPipeLine>();

        return services.BuildServiceProvider();
    }
}
