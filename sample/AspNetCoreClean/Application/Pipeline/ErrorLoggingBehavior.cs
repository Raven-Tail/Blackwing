using Microsoft.Extensions.Logging;
using Blackwing.Contracts.Pipelines;
using Blackwing.Contracts.Requests;

namespace AspNetCoreSample.Application;

public sealed class ErrorLoggingBehavior<TMessage, TResponse> : IRequestPipeline<TMessage, TResponse>
    where TMessage : notnull, IRequest
{
    private readonly ILogger<ErrorLoggingBehavior<TMessage, TResponse>> _logger;

    public ErrorLoggingBehavior(ILogger<ErrorLoggingBehavior<TMessage, TResponse>> logger)
    {
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TMessage request, IRequestPipelineDelegate<TMessage, TResponse> next, CancellationToken cancellationToken = default)
    {
        try
        {
            return await next(request, cancellationToken);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error handling message of type {messageType}", request.GetType().Name);
            throw;
        }
    }
}
