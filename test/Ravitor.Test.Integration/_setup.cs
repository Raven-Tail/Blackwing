using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts.Options;

//[assembly:RavitorOptions(DisableInterceptor = true)]

namespace Ravitor.Test.Integration;

public abstract class TestBase
{
    protected IServiceProvider SetupServices(Action<IServiceCollection>? setupAction = null)
    {
        var services = new ServiceCollection();

        services.AddRavitor();
        services.AddRavitorHandlers();
        setupAction?.Invoke(services);

        return services.BuildServiceProvider();
    }
}
