using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Ravitor.Contracts;
using Ravitor.Contracts.Handlers;
using Ravitor.Contracts.Pipelines;
using Ravitor.Contracts.Requests;
using Ravitor.Services;
using Microsoft.Extensions.DependencyInjection;

var mediator = new CustomMediator();

var id = Guid.NewGuid();
var request = new Ping(id);

_ = await mediator.Send(request);

//
// Types
//

public sealed record Ping(Guid Id) : IRequest<Pong>;

public sealed record Pong(Guid Id);

public sealed class PingHandler : IRequestHandler<Ping, Pong>
{
    public ValueTask<Pong> Handle(Ping request, CancellationToken cancellationToken)
    {
        return new ValueTask<Pong>(new Pong(request.Id));
    }
}

// Custom implementation to get an IMediator or find the handler and send the request.
public readonly struct CustomMediator : IMediator
{
    public ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) where TRequest : notnull, IRequest<TResponse>
    {
        throw new NotImplementedException();
    }
}
