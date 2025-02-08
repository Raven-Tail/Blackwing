using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts;
using Ravitor.Contracts.Handlers;
using Ravitor.Contracts.Pipelines;
using Ravitor.Contracts.Requests;
using TUnit.Assertions.AssertConditions.Throws;

namespace Ravitor.Test.Integration;

public sealed class RequestTests : TestBase
{
    public sealed class RequestContext
    {
        public bool SkipHandler { get; set; }

        public Stack<string> CallStack { get; } = [];
    }

    public sealed class Ping : IRequest<Pong>;

    public sealed record Pong(int Num);

    public sealed class PingHandler(RequestContext context) : IRequestHandler<Ping, Pong>
    {
        public ValueTask<Pong> Handle(Ping request, CancellationToken cancellationToken = default)
        {
            context.CallStack.Push(GetType().Name);
            return new(new Pong(0));
        }
    }

    public sealed class PingPipeLine(RequestContext context) : IRequestPipeline<Ping, Pong>
    {
        public ValueTask<Pong> Handle(Ping request, IRequestPipelineDelegate<Ping, Pong> next, CancellationToken cancellationToken = default)
        {
            context.CallStack.Push(GetType().Name);

            return context.SkipHandler ? new(new Pong(1)) : next(request, cancellationToken);
        }
    }

    [Test]
    public async Task ShouldReplaceHandler()
    {
        var services = SetupServices(services =>
        {
            services.AddScoped<RequestContext>();
            services.AddScoped<IRequestPipeline<Ping, Pong>, PingPipeLine>();
        });

        var handler = services.GetRequiredService<IRequestHandler<Ping, Pong>>();
        var sender = services.GetRequiredService<ISender>();
        var context = services.GetRequiredService<RequestContext>();

        var pong = (Pong)(await Assert.That(async () => await sender.Send(new Ping())).ThrowsNothing())!;
        var stack = context.CallStack.ToArray();

        await Assert.That(handler.GetType()).IsNotEqualTo(typeof(PingHandler));
        await Assert.That(stack[0]).IsEqualTo(nameof(PingHandler));
        await Assert.That(stack[1]).IsEqualTo(nameof(PingPipeLine));
        await Assert.That(pong.Num).IsEqualTo(0);
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
        var services = SetupServices(services =>
        {
            services.AddScoped<RequestContext>();
            services.AddScoped<IRequestPipeline<Ping, Pong>, PingPipeLine>();
        });

        var sender = new StructMediator(services);

        await Assert.That(async () => await sender.Send(new Ping())).Throws<NotImplementedException>();
    }
}
