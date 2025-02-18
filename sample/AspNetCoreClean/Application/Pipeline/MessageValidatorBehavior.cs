using Blackwing.Contracts.Requests;

namespace AspNetCoreSample.Application;

public sealed class MessageValidatorBehavior<TMessage, TResponse> : IRequestPipeline<TMessage, TResponse>
    where TMessage : IValidate
{
    public ValueTask<TResponse> Handle(TMessage request, IRequestPipelineDelegate<TMessage, TResponse> next, CancellationToken cancellationToken = default)
    {
        if (!request.IsValid(out var validationError))
            throw new ValidationException(validationError);

        return next(request, cancellationToken);
    }
}
