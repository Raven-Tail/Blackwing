using Microsoft.Extensions.DependencyInjection;
using Blackwing.Contracts;
using Blackwing.Contracts.Requests;

namespace Blackwing.Services;

public sealed class Mediator : IMediator
{
    private readonly IServiceProvider serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Mediator doesn't implement this method because the source generator will call the other one.");
    }

    public ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : notnull, IRequest<TResponse>
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return handler.Handle(request, cancellationToken);
    }
}
