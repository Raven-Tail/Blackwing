using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Ravitor.Benchmark;

public sealed class MediatorImpl
{
    private static ServiceProvider SP = null!;

    public static void Setup()
    {
        var services = new ServiceCollection();

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Pipeline1<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Pipeline2<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Pipeline3<,>));

        services.AddMediator(o => o.ServiceLifetime = ServiceLifetime.Scoped);

        SP = services.BuildServiceProvider();
    }

    public static async Task<Output> Run()
    {
        //Setup();

        var handler = SP.GetRequiredService<ISender>();
        var result = await handler.Send(new Input(), default);
        return result;
    }
    
    public static async Task<Output> Run_Class()
    {
        //Setup();

        var handler = SP.GetRequiredService<Mediator.Mediator>();
        var result = await handler.Send(new Input(), default);
        return result;
    }

    public interface IInput
    {
        string Type { get; }
    }

    public sealed class Input : IQuery<Output>, IInput
    {
        public string Type => "aaaa";
    }

    public sealed class Output;

    public sealed class RequestHandler : IQueryHandler<Input, Output>
    {
        public ValueTask<Output> Handle(Input request, CancellationToken cancellationToken)
        {
            var output = new Output();
            return new(output);
        }
    }

    public sealed class Pipeline1<TInput, TOutput> : IPipelineBehavior<TInput, TOutput>
        where TInput : IMessage
    {
        public ValueTask<TOutput> Handle(TInput request, CancellationToken cancellationToken, MessageHandlerDelegate<TInput, TOutput> next)
        {
            return next(request, cancellationToken);
        }
    }

    public sealed class Pipeline2<TInput, TOutput> : IPipelineBehavior<TInput, TOutput>
        where TInput : IMessage, IInput
    {
        public ValueTask<TOutput> Handle(TInput request, CancellationToken cancellationToken, MessageHandlerDelegate<TInput, TOutput> next)
        {
            return next(request, cancellationToken);
        }
    }

    public sealed class Pipeline3<TInput, TOutput> : IPipelineBehavior<TInput, TOutput>
        where TInput : IMessage
    {
        public ValueTask<TOutput> Handle(TInput request, CancellationToken cancellationToken, MessageHandlerDelegate<TInput, TOutput> next)
        {
            return next(request, cancellationToken);
        }
    }
}
