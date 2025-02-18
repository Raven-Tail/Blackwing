namespace Blackwing.Contracts.Requests;

public delegate ValueTask<TResponse> IRequestPipelineDelegate<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
    where TRequest : notnull;
