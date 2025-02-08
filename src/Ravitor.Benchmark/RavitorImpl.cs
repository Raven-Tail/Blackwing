using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts;
using Ravitor.Contracts.Handlers;
using Ravitor.Contracts.Pipelines;
using Ravitor.Contracts.Requests;

namespace Ravitor.Benchmark;

public sealed class RavitorImpl
{
    private static ServiceProvider SP = null!;

    public static void Setup()
    {
        var services = new ServiceCollection();

        services.AddScoped(typeof(IRequestPipeline<,>), typeof(Pipeline1<,>));
        services.AddScoped(typeof(IRequestPipeline<,>), typeof(Pipeline2<,>));
        services.AddScoped(typeof(IRequestPipeline<,>), typeof(Pipeline3<,>));

        services.AddRavitor();
        services.AddRavitorHandlers();

        SP = services.BuildServiceProvider();
    }

    public static async Task<Output> Run()
    {
        var handler = SP.GetRequiredService<ISender>();
        var result = await handler.Send(new Input(), default);
        return result;
    }

    public static async Task<Output> Run_Class()
    {
        var handler = SP.GetRequiredService<Ravitor.Services.Mediator>();
        var result = await handler.Send(new Input(), default);
        return result;
    }

    public interface IInput
    {
        string Type { get; }
    }

    public sealed class Input : IRequest<Output>, IInput
    {
        public string Type => "aaaa";
    }

    public sealed class Output;

    public sealed class RequestHandler : IRequestHandler<Input, Output>
    {
        public ValueTask<Output> Handle(Input request, CancellationToken cancellationToken)
        {
            var output = new Output();
            return new(output);
        }
    }

    public sealed class Pipeline1<TInput, TOutput> : IRequestPipeline<TInput, TOutput>
        where TInput : IRequest
    {
        public ValueTask<TOutput> Handle(TInput request, IRequestPipelineDelegate<TInput, TOutput> next, CancellationToken cancellationToken = default)
        {
            return next(request, cancellationToken);
        }
    }

    public sealed class Pipeline2<TInput, TOutput> : IRequestPipeline<TInput, TOutput>
        where TInput : IRequest, IInput
    {
        public ValueTask<TOutput> Handle(TInput request, IRequestPipelineDelegate<TInput, TOutput> next, CancellationToken cancellationToken = default)
        {
            return next(request, cancellationToken);
        }
    }

    public sealed class Pipeline3<TInput, TOutput> : IRequestPipeline<TInput, TOutput>
        where TInput : IRequest
    {
        public ValueTask<TOutput> Handle(TInput request, IRequestPipelineDelegate<TInput, TOutput> next, CancellationToken cancellationToken = default)
        {
            return next(request, cancellationToken);
        }
    }
}
