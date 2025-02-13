using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts;
using Ravitor.Contracts.Requests;
using Ravitor.Services;
using Ravitor.Test.Integration.Helpers;
using TUnit.Assertions.AssertConditions.Throws;

namespace Ravitor.Test.Integration;

public sealed class InterceptorTests : TestBase
{
    [Test]
    public async Task ShouldInterceptISender()
    {
        var services = SetupServices();

        var sender = services.GetRequiredService<ISender>();

        _ = await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing();
    }

    [Test]
    public async Task ShouldInterceptIMediator()
    {
        var services = SetupServices();

        var sender = services.GetRequiredService<IMediator>();

        _ = await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing();
    }

    [Test]
    public async Task ShouldInterceptMediator()
    {
        var services = SetupServices();

        var sender = services.GetRequiredService<Mediator>();

        _ = await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing();
    }

    public interface ICustomMediator : IMediator
    {
    }

    public abstract class CustomMediatorBase : ICustomMediator
    {
        public abstract ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

        public abstract ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : notnull, IRequest<TResponse>;
    }

    public class CustomMediator : CustomMediatorBase
    {
        private readonly Mediator mediator;

        public CustomMediator(Mediator Mediator)
        {
            mediator = Mediator;
        }

        public override ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            return mediator.Send(request, cancellationToken);
        }

        public override ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        {
            return mediator.Send<TRequest, TResponse>(request, cancellationToken);
        }
    }

    [Test]
    public async Task ShouldInterceptCustomAbstractClass()
    {
        var services = SetupServices(static services =>
        {
            services.AddScoped<ICustomMediator>(sp => sp.GetRequiredService<CustomMediator>());
            services.AddScoped<CustomMediatorBase>(sp => sp.GetRequiredService<CustomMediator>());
            services.AddScoped<CustomMediator>();
        });

        var sender = services.GetRequiredService<CustomMediatorBase>();

        _ = await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing();
    }

    [Test]
    public async Task ShouldInterceptCustomClass()
    {
        var services = SetupServices(static services =>
        {
            services.AddScoped<ICustomMediator>(sp => sp.GetRequiredService<CustomMediator>());
            services.AddScoped<CustomMediatorBase>(sp => sp.GetRequiredService<CustomMediator>());
            services.AddScoped<CustomMediator>();
        });

        var sender = services.GetRequiredService<CustomMediator>();

        _ = await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing();
    }

    [Test]
    public async Task ShouldInterceptCustomInterface()
    {
        var services = SetupServices(static services =>
        {
            services.AddScoped<ICustomMediator>(sp => sp.GetRequiredService<CustomMediator>());
            services.AddScoped<CustomMediatorBase>(sp => sp.GetRequiredService<CustomMediator>());
            services.AddScoped<CustomMediator>();
        });

        var sender = services.GetRequiredService<ICustomMediator>();

        _ = await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing();
    }

    [Test]
    public async Task ShouldNotInterceptInterfaceRequest()
    {
        var services = SetupServices();

        var sender = services.GetRequiredService<IMediator>();

        IRequest<Pong> request = new Ping();

        // todo: Implement diagnostic that the interceptor can only work with concrete non generic classes.
        _ = await Assert.That(async () => await sender.Send(request)).ThrowsExactly<NotImplementedException>();
    }
}
