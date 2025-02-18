using Blackwing.Contracts.Requests;

namespace Blackwing.Contracts.Handlers;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}
