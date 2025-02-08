using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ravitor.Benchmark;

public sealed class MediatRImpl
{
    private static ServiceProvider SP = null!;

    public static void Setup()
    {
        var services = new ServiceCollection();

        services.AddMediatR(options =>
        {
            options.Lifetime = ServiceLifetime.Scoped;
            options.RegisterServicesFromAssemblyContaining<MediatRImpl>();
            options.AddOpenBehavior(typeof(Pipeline1<,>));
            options.AddOpenBehavior(typeof(Pipeline2<,>));
            options.AddOpenBehavior(typeof(Pipeline3<,>));
        });

        SP = services.BuildServiceProvider();
    }

    public static async Task<Output> Run()
    {
        //Setup();

        var handler = SP.GetRequiredService<ISender>();
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
        public Task<Output> Handle(Input request, CancellationToken cancellationToken)
        {
            var output = new Output();
            return Task.FromResult(output);
        }
    }

    public sealed class Pipeline1<TInput, TOutput> : IPipelineBehavior<TInput, TOutput>
        where TInput : notnull
    {
        public Task<TOutput> Handle(TInput request, RequestHandlerDelegate<TOutput> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public sealed class Pipeline2<TInput, TOutput> : IPipelineBehavior<TInput, TOutput>
        where TInput : notnull, IInput
    {
        public Task<TOutput> Handle(TInput request, RequestHandlerDelegate<TOutput> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }

    public sealed class Pipeline3<TInput, TOutput> : IPipelineBehavior<TInput, TOutput>
        where TInput : notnull
    {
        public Task<TOutput> Handle(TInput request, RequestHandlerDelegate<TOutput> next, CancellationToken cancellationToken = default)
        {
            return next();
        }
    }
}
