using Ravitor.Contracts.Pipelines;

namespace Ravitor.Test.Integration.Helpers;

public sealed class PingPipeLine(RequestContext context) : IRequestPipeline<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping request, IRequestPipelineDelegate<Ping, Pong> next, CancellationToken cancellationToken = default)
    {
        context.CallStack.Push(GetType().Name);

        return context.SkipHandler ? new(new Pong(1)) : next(request, cancellationToken);
    }
}