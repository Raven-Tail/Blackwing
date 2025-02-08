using Microsoft.Extensions.DependencyInjection;
using Ravitor.Contracts;
using Ravitor.Contracts.Handlers;
using Ravitor.Contracts.Requests;

namespace Ravitor.Services;

public sealed class MediatorReflection : IMediator
{
    private static readonly Type requestHandlerType = typeof(IRequestHandler<,>);
    private readonly IServiceProvider serviceProvider;

    public MediatorReflection(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var t = requestHandlerType.MakeGenericType(request.GetType(), typeof(TResponse));
        var h = serviceProvider.GetRequiredService(t);

        var m = t.GetMethod("Handle")!;
        var r = m.Invoke(h, [request, cancellationToken]);
        return (ValueTask<TResponse>)r!;
    }

    public ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : notnull, IRequest<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return handler.Handle(request, cancellationToken);
    }
}