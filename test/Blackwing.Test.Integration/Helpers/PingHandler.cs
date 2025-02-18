using Blackwing.Contracts.Handlers;

namespace Blackwing.Test.Integration.Helpers;

public sealed class PingHandler(RequestContext context) : IRequestHandler<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping request, CancellationToken cancellationToken = default)
    {
        context.CallStack.Push(GetType().Name);
        return new(new Pong(0));
    }
}
