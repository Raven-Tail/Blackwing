namespace Blackwing.Contracts.Requests;

public interface IRequestPipeline<TRequest, TResponse>
    where TRequest : IRequest
{
    ValueTask<TResponse> Handle(TRequest request, IRequestPipelineDelegate<TRequest, TResponse> next, CancellationToken cancellationToken = default);
}
