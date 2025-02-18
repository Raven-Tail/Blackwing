namespace Blackwing.Contracts.Pipelines;

public delegate ValueTask<TResponse> IRequestPipelineDelegate<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
    where TRequest : notnull;

public interface IRequestPipeline<TRequest, TResponse>
    where TRequest : notnull
{
    ValueTask<TResponse> Handle(TRequest request, IRequestPipelineDelegate<TRequest, TResponse> next, CancellationToken cancellationToken = default);
}
