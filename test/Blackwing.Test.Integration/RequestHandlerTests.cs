using Microsoft.Extensions.DependencyInjection;
using Blackwing.Contracts;
using Blackwing.Contracts.Handlers;
using Blackwing.Contracts.Requests;
using Blackwing.Test.Integration.Helpers;
using TUnit.Assertions.AssertConditions.Throws;

namespace Blackwing.Test.Integration;

public sealed class RequestHandlerTests : TestBase
{
    [Test]
    public async Task ShouldReplaceHandler()
    {
        var services = SetupServices();

        var handler = services.GetRequiredService<IRequestHandler<Ping, Pong>>();
        var sender = services.GetRequiredService<ISender>();
        var context = services.GetRequiredService<RequestContext>();

        var pong = await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing();
        var stack = context.CallStack.ToArray();

        await Assert.That(handler.GetType()).IsNotEqualTo(typeof(PingHandler));
        await Assert.That(stack[0]).IsEqualTo(nameof(PingHandler));
        await Assert.That(stack[1]).IsEqualTo(nameof(PingPipeLine));
        await Assert.That(pong?.Num).IsEqualTo(0);
    }

    public readonly struct StructMediator : IMediator
    {
        private readonly IServiceProvider services;

        public StructMediator(IServiceProvider services)
        {
            this.services = services;
        }

        public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : notnull, IRequest<TResponse>
        {
            return services.GetRequiredService<IRequestHandler<TRequest, TResponse>>().Handle(request, cancellationToken);
        }
    }

    [Test]
    public async Task ShouldNotWorkWithStructMediator()
    {
        var services = SetupServices();

        var sender = new StructMediator(services);

        // todo: Implement diagnostic that the interceptor can not work with structs as a Mediator.
        await Assert.That(async () => await sender.Send(new Ping())).Throws<NotImplementedException>();
    }
}
